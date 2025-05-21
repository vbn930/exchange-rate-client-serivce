using System.Text.Json;

using ExchangeRateClientService.Services;
using ExchangeRateClientService.Dtos;
using ExchangeRateClientService.Utils;
using System.Globalization;

namespace ExchangeRateClientService.Clients;

public class ExchangeAPIClient
{
    private readonly Logger _logger;
    private readonly APIRequestWrapper _apiRequestWrapper;
    private readonly string _token;
    private readonly string _apiRequestUrl;
    private readonly List<ExchangeRateData> _dataStack;

    public ExchangeAPIClient(
        IConfiguration configuration,
        APIRequestWrapper apiRequestWrapper,
        string token
    )
    {
        if (null == configuration)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        string serviceName = configuration["Logging:ServiceName"];
        _logger = new Logger(serviceName);

        _apiRequestWrapper = apiRequestWrapper;
        _token = token;
        _apiRequestUrl = $"https://openexchangerates.org/api/latest.json?app_id={_token}&base=USD&prettyprint=true&show_alternative=false";
        _dataStack = new List<ExchangeRateData>();
    }

    private async Task<string?> RequestExchangeRateDataAsync()
    {
        using (var log = _logger.StartMethod(nameof(ExchangeAPIClient)))
        {
            string? res = await _apiRequestWrapper.GetAsync(_apiRequestUrl);
            return res;
        }
    }

    public async Task<ExchangeRateData> GetExchangeRateDataAsync()
    {
        string res = await RequestExchangeRateDataAsync();
        if (string.IsNullOrEmpty(res)) { throw new Exception("API response is null or empty"); }
        using (var log = _logger.StartMethod(nameof(GetExchangeRateDataAsync)))
        {
            log.SetAttribute("response", res);

            ExchangeRateData? data = JsonSerializer.Deserialize<ExchangeRateData>(res);

            if (data == null) { throw new Exception("Failed to serialize response value"); }

            _dataStack.Add(data);
            return data;
        }
    }

    public async Task SaveDataStackIntoDB()
    {
        //TODO: DB에 data stack 저장
        await Task.Delay(0);
        ClearDataStack();
    }

    public void ClearDataStack()
    {
        _dataStack.Clear();
    }
}
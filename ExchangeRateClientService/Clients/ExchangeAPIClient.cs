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
    private readonly List<ConvertedExchangeRateData> _dataStack;

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
        _dataStack = new List<ConvertedExchangeRateData>();
    }

    private async Task<string?> RequestExchangeRateDataAsync()
    {
        using (var log = _logger.StartMethod(nameof(ExchangeAPIClient)))
        {
            string? res = await _apiRequestWrapper.GetAsync(_apiRequestUrl);
            return res;
        }
    }

    public async Task<ConvertedExchangeRateData> GetExchangeRateDataAsync()
    {
        string res = await RequestExchangeRateDataAsync();
        if (string.IsNullOrEmpty(res)) { throw new Exception("API response is null or empty"); }
        using (var log = _logger.StartMethod(nameof(GetExchangeRateDataAsync)))
        {
            log.SetAttribute("response", res);

            ExchangeRateData? data = JsonSerializer.Deserialize<ExchangeRateData>(res);

            if (data == null) { throw new Exception("Failed to serialize response value"); }

            ConvertedExchangeRateData convertedData = new ConvertedExchangeRateData();
            convertedData.Timestamp = data.Timestamp;
            convertedData.Base = "KRW";

            //rate data conversion
            var rateDict = data.Rates;
            var rateKeyList = rateDict.Keys.ToList();
            decimal KRWRate = rateDict["KRW"];

            //convert base currency to KRW
            for (int i = 0; i < rateDict.Count; i++)
            {
                string key = rateKeyList[i];
                if (key != "KRW")
                {
                    rateDict[key] = CurrencyConvertor.ConvertCurrencyRate(KRWRate, rateDict[key]);
                }
            }

            //for japan Yen, times 100
            rateDict["JPY"] *= 100;

            //exchange KWR and USD
            rateDict["USD"] = Math.Round(KRWRate, 2);
            rateDict["KRW"] = 1;
            convertedData.Rates = rateDict;

            _dataStack.Add(convertedData);
            return convertedData;
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
using System.Text.Json;

using ExchangeRateClientService.Services;
using ExchangeRateClientService.Dtos;
using ExchangeRateClientService.Utils;
using System.Globalization;
using ExchangeRateClientService.Azure;

namespace ExchangeRateClientService.Clients;

public class ExchangeAPIClient
{
    private readonly Logger _logger;
    private readonly APIRequestWrapper _apiRequestWrapper;
    private readonly CosmosDbWrapper _cosmosDbWrapper;
    private readonly string _token;
    private readonly string _apiRequestUrl;
    private readonly List<ConvertedExchangeRateData> _dataStack;

    public ExchangeAPIClient(
        IConfiguration configuration,
        APIRequestWrapper apiRequestWrapper,
        CosmosDbWrapper cosmosDbWrapper,
        string token
    )
    {
        if (null == configuration)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(nameof(token));
        }

        string serviceName = configuration["Logging:ServiceName"];
        _logger = new Logger(serviceName);

        using (var log = _logger.StartMethod(nameof(ExchangeAPIClient)))
        {
            log.SetAttribute("token: ", token);
            _apiRequestWrapper = apiRequestWrapper;
            _cosmosDbWrapper = cosmosDbWrapper;
            _token = token;
            _apiRequestUrl = $"https://openexchangerates.org/api/latest.json?app_id={_token}&base=USD&prettyprint=true&show_alternative=false";
            _dataStack = new List<ConvertedExchangeRateData>();
        }
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

    public async Task SaveDataStackIntoDBAsync()
    {
        using (var log = _logger.StartMethod(nameof(SaveDataStackIntoDBAsync)))
        {
            string id = DateTime.Now.ToString("yyyyMMdd");
            log.SetAttribute("id", id);
            var dataDict = new ConvertedExchangeRateDataDict(id, DataStackToDict());
            await _cosmosDbWrapper.AddItemAsync(dataDict, id);
            ClearDataStack();
        }
        
    }

    public Dictionary<string, ConvertedExchangeRateData> DataStackToDict()
    {
        Dictionary<string, ConvertedExchangeRateData> dict = new Dictionary<string, ConvertedExchangeRateData>();

        foreach (var data in _dataStack)
        {
            dict.Add(data.Timestamp.ToString(), data);
        }

        return dict;
    }

    public void ClearDataStack()
    {
        _dataStack.Clear();
    }
}
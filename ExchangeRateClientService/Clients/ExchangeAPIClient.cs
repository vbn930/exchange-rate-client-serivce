using System.Text.Json;

using ExchangeRateClientService.Services;
using ExchangeRateClientService.Dtos;

namespace ExchangeRateClientService.Clients;

public class ExchangeAPIClient
{
    private readonly APIRequestWrapper _apiRequestWrapper;
    private readonly string _token;
    private readonly string _apiRequestUrl;
    private readonly List<ExchangeRateData> _dataStack;

    public ExchangeAPIClient(
        APIRequestWrapper apiRequestWrapper,
        string token
    )
    {
        _apiRequestWrapper = apiRequestWrapper;
        _token = token;
        _apiRequestUrl = $"https://openexchangerates.org/api/latest.json?app_id={_token}&base=USD&prettyprint=true&show_alternative=false";
        _dataStack = new List<ExchangeRateData>();
    }

    private async Task<string> RequestExchangeRateDataAsync()
    {
        string? res = await _apiRequestWrapper.GetAsync(_apiRequestUrl);
        if (res == null) { throw new ArgumentNullException("Failed to API request"); }

        return res;
    }

    public async Task<ExchangeRateData> GetExchangeRateDataAsync()
    {
        string res = await RequestExchangeRateDataAsync();
        Console.WriteLine(res);
        ExchangeRateData? data = JsonSerializer.Deserialize<ExchangeRateData>(res);

        if (data == null) { throw new ArgumentNullException("Failed to get exchange data"); }

        _dataStack.Add(data);
        return data;
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
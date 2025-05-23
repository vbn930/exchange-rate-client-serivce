using ExchangeRateClientService.Dtos;
using ExchangeRateClientService.Utils;

namespace ExchangeRateClientService.Services;

public static class CurrencyConvertor
{
    private readonly static Logger _logger;

    static CurrencyConvertor()
    {
        string serviceName = "ExchangeRateClientService";
        _logger = new Logger(serviceName);
    } 
    public static decimal ConvertCurrencyRate(decimal srcCurrencyRate, decimal baseCurrencyRate)
    {
        var currency = Math.Round(srcCurrencyRate / baseCurrencyRate, 2);
        return currency;
    }
}
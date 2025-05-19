using ExchangeRateClientService.Dtos;

namespace ExchangeRateClientService.Services;

public static class CurrencyConvertor
{
    public static decimal ConvertToKRW(ExchangeRateData data, string baseCurrency)
    {
        var rate = data.Rates[baseCurrency];
        var currency = Math.Round(data.Rates["KRW"] / rate, 2);
        return currency;
    }
}
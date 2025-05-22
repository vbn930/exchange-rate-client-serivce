using ExchangeRateClientService.Dtos;

namespace ExchangeRateClientService.Services;

public static class CurrencyConvertor
{
    public static decimal ConvertCurrencyRate(decimal srcCurrencyRate, decimal baseCurrencyRate)
    {
        var currency = Math.Round(srcCurrencyRate / baseCurrencyRate, 2);
        return currency;
    }
}
using Microsoft.AspNetCore.Mvc;

using ExchangeRateClientService.Utils;
using ExchangeRateClientService.Services;
using ExchangeRateClientService.Clients;

namespace ExchangeRateClientService.Controller;

[ApiController]
[Route("exchange-rate")]
public class ExchangeRateController : ControllerBase
{
    private readonly Logger _logger;
    private readonly ExchangeAPIClient _exchangeAPIClient;
    public ExchangeRateController(IConfiguration configuration, ExchangeAPIClient exchangeAPIClient)
    {
        if (null == configuration)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        string serviceName = configuration["Logging:ServiceName"];
        _logger = new Logger(serviceName);
        _exchangeAPIClient = exchangeAPIClient;
    }

    [HttpGet("rate-data")]
    public async Task<IActionResult> GetExchangeRates()
    {
        using (var log = _logger.StartMethod(nameof(GetExchangeRates)))
        {
            var res = await _exchangeAPIClient.GetExchangeRateDataAsync();
            var rateDict = res.Rates;
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
            res.Rates = rateDict;

            return Ok(res);
        }
    }
}
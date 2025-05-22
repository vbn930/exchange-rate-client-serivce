using Microsoft.AspNetCore.Mvc;

using ExchangeRateClientService.Utils;
using ExchangeRateClientService.Services;
using ExchangeRateClientService.Clients;

namespace ExchangeRateClientService.Controller;

[ApiController]
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

    [HttpGet]
    public async Task<IActionResult> GetExchangeRates()
    {
        var res = await _exchangeAPIClient.GetExchangeRateDataAsync();
        return Ok(res);
    }
}
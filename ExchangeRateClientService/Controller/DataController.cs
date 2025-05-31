using Microsoft.AspNetCore.Mvc;

using ExchangeRateClientService.Azure;
using ExchangeRateClientService.Services;
using ExchangeRateClientService.Clients;
using ExchangeRateClientService.Utils;
using ExchangeRateClientService.Dtos;

[ApiController]
[Route("data")]
public class DataController : ControllerBase
{
    private readonly ExchangeAPIClient _exchangeApiClient;
    private readonly CosmosDbWrapper _cosmosDbWrapper;
    private readonly Logger _logger;
    public DataController(IConfiguration configuration, ExchangeAPIClient exchangeApiClient, CosmosDbWrapper cosmosDbWrapper)
    {
        if (null == configuration)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        string serviceName = configuration["Logging:ServiceName"];
        _logger = new Logger(serviceName);
        
        _exchangeApiClient = exchangeApiClient;
        _cosmosDbWrapper = cosmosDbWrapper;
    }
}

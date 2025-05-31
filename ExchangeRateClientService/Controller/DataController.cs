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

    [HttpGet("data/{date}")]
    public async Task<IActionResult> GetDataAsync(string date)
    {
        using (var log = _logger.StartMethod(nameof(GetDataAsync)))
        {
            log.SetAttribute("Date", date);

            if (string.IsNullOrEmpty(date) || date.Length != 6 || !long.TryParse(date, out _))
            {
                return BadRequest("Date parameter is invaild");
            }

            string id = $"ConvertedExchangeRateDataDict-{date}";
            var data = await _cosmosDbWrapper.GetItemAsync<ConvertedExchangeRateDataDict>(id, date);

            return Ok(data);
        }
    }

    [HttpPost("save-data")]
    public async Task<IActionResult> PostSaveDataAsync()
    {
        using (var log = _logger.StartMethod(nameof(PostSaveDataAsync)))
        {
            await _exchangeApiClient.SaveDataStackIntoDBAsync();
            return Ok();
        }
    }
}

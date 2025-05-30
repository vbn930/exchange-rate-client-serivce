using Microsoft.AspNetCore.Mvc;

using ExchangeRateClientService.Utils;
using ExchangeRateClientService.Services;
using ExchangeRateClientService.Clients;

namespace ExchangeRateClientService.Controller;

[TimerController]
[Route("timer")]
public class TimerController: ControllerBase{
    private readonly Logger _logger;
    private readonly CosmosDbWrapper _cosmosDbWrapper;
    public TimerController(IConfiguration configuration, CosmosDbWrapper cosmosDbWrapper){
        if (null == configuration)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        string serviceName = configuration["Logging:ServiceName"];
        _logger = new Logger(serviceName);
        _cosmosDbWrapper = cosmosDbWrapper;
    }

    [HttpGet("setting/{id}")]
    public async Task<IActionResult> GetTimerSettingAsync(ulong id){
        using (var log = _logger.StartMethod(nameof(GetTimerSettingAsync))){
            string cosmosId = $"ChannelTimerSettings-{id}";
            ulong pk = id;

            log.SetAttribute("id", cosmosId);
            log.SetAttribute("pk", pk);

            var timerSetting = await _cosmosDbWrapper.GetItemAsync<ChannelTimerSetting>(cosmosId, pk);

            if (timerSettinf == default){
                return NotFound();
            }

            return Ok(timerSetting);
        }
    }

    [HttpPost("setting")]
    public async Task<IActionResult<ChannelTimerSetting>> PostTimerSettingAsync(ChannelTimerSetting timerSetting){
        using (var log = _logger.StartMethod(nameof(PostTimerSettingAsync))){
            log.SetAttribute("Channel Id", timerSetting.ChannelId);

            await _cosmosDbWrapper.AddItemAsync<ChannelTimerSetting>(timerSetting, timerSetting.ChannelId);

            return return CreatedAtAction(nameof(GetTimerSettingAsync), new { id = timerSetting.ChannelId }, timerSetting);
        }
    }
}
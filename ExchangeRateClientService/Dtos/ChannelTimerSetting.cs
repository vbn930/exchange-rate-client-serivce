using ExchangeRateClientService.Dtos;

public class ChannelTimerSetting
{
    public string Id { get; set; } // Cosmos DB 저장을 위한 ID
    public ulong ChannelId { get; set; }
    public int IntervalMinutes { get; set; }
    public DateTimeOffset LastSentTime { get; set; }
    public bool IsEnabled { get; set; }

    public ChannelTimerSetting() { }

    public ChannelTimerSetting(ulong channelId, int intervalMinutes)
    {
        ChannelId = channelId;
        IntervalMinutes = intervalMinutes;
        LastSentTime = DateTimeOffset.UtcNow;
        IsEnabled = true;
        // Cosmos DB Id 설정 (예시)
        Id = $"ChannelTimerSettings-{channelId}";
    }
}
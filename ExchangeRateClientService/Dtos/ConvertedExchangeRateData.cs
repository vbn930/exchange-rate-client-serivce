using System.Text.Json.Serialization;

namespace ExchangeRateClientService.Dtos
{
    public class ConvertedExchangeRateData
    {
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("base")]
        public string Base { get; set; }

        [JsonPropertyName("rates")]
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
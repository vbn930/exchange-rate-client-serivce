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

    public class ConvertedExchangeRateDataDict
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, ConvertedExchangeRateData> Data { get; set; }

        public ConvertedExchangeRateDataDict(string date, Dictionary<string, ConvertedExchangeRateData> data)
        {
            Date = date;
            Data = data;
            Id = $"ConvertedExchangeRateDataDict-{date}";
        }
    }
}
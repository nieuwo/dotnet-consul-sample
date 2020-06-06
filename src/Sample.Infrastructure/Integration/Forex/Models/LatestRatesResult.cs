using Newtonsoft.Json;

namespace Sample.Infrastructure.Integration.Forex.Models
{
    public class LatestRatesResult
    {

        [JsonProperty("rates")] 
        public Rates Rates { get; set; }

        [JsonProperty("base")] 
        public string Base { get; set; }

        [JsonProperty("date")] 
        public string Date { get; set; }

    }
}
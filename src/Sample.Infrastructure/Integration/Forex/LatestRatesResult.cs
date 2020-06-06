using Newtonsoft.Json;

namespace Sample.Infrastructure.Integration.Forex
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
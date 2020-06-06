using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sample.Infrastructure.Integration.Forex.Models
{
    public class HistoricRatesResult
    {

        [JsonProperty("rates")]
        public Dictionary<string, RatesData> HistoricRates { get; set; }

        [JsonProperty("start_at")]
        public string StartAt { get; set; }

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("end_at")]
        public string EndAt { get; set; }
    }
}
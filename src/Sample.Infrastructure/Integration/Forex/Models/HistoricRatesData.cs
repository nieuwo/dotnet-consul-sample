using System.Collections.Generic;

namespace Sample.Infrastructure.Integration.Forex.Models
{
    public class HistoricRatesData
    {
        public Dictionary<string, RatesData> HistoricRates { get; set; }
    }
}
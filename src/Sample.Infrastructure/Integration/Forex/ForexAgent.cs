using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Sample.Infrastructure.Integration.Forex
{
    public class ForexAgent : IForexAgent
    {
        private const string _url = "https://api.exchangeratesapi.io";
        private readonly ILogger _logger;
        private IHttpClientFactory _clientFactory;
        public HttpClient _client { get; }
        public ForexAgent(ILogger logger, HttpClient httpClient)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _client = httpClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<LatestRatesResult> GetLatest()
        {
            try
            {
                string apiPath = "/latest";
                var responseString = await _client.GetStringAsync(apiPath).ConfigureAwait(false);
                var rates = JsonConvert.DeserializeObject<LatestRatesResult>(responseString);
                return rates;
            }
            catch (Exception ex)
            {
                _logger.Error(ex,ex.Message);
                return null;
            }
        }

        public async Task<HistroricRatesResult> GetHistoric(DateTime startDate, DateTime endDate, string baseCurrency)
        {
            try
            {
                string apiPath = $"/history?start_at={startDate:yyyy-MM-dd}&end_at={endDate:yyyy-MM-dd}&base={baseCurrency}";
                var responseString = await _client.GetStringAsync(apiPath).ConfigureAwait(false);
                var rates = JsonConvert.DeserializeObject<HistroricRatesResult>(responseString);
                dynamic json = JValue.Parse(responseString);
                rates.HistroricRates = new Dictionary<string, RatesData>();
                foreach (var rate in json.rates)
                {
                    var data = JsonConvert.DeserializeObject<RatesData>(rate.Value.ToString()); ;
                    rates.HistroricRates.Add(rate.Name.ToString(), data);
                }
                
                return rates;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                return null;
            }
        }
    }
}
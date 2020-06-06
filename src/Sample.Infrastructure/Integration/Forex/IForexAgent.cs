using System;
using System.Threading.Tasks;
using Sample.Infrastructure.Integration.Forex.Models;

namespace Sample.Infrastructure.Integration.Forex
{
    public interface IForexAgent
    {
        Task<LatestRatesResult> GetLatest();
        Task<HistoricRatesResult> GetHistoric(DateTime startDate, DateTime endDate, string baseCurrency);
    }
}
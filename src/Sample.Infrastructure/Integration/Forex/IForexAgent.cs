using System;
using System.Threading.Tasks;

namespace Sample.Infrastructure.Integration.Forex
{
    public interface IForexAgent
    {
        Task<LatestRatesResult> GetLatest();
        Task<HistroricRatesResult> GetHistoric(DateTime startDate, DateTime endDate, string baseCurrency);
    }
}
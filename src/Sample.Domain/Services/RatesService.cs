using System;
using Sample.Domain.Contracts;
using Serilog;

namespace Sample.Domain.Services
{
    public class RatesService : IRatesService
    {
        private readonly ILogger _logger;
        public RatesService(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
        }
    }
}
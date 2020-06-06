using Sample.Domain.Contracts;
using Serilog;

namespace Sample.Domain.Services
{
    public class NotifierService : INotifierService
    {
        private readonly ILogger _logger;
        public NotifierService(ILogger logger)
        {
            _logger = logger;
        }
        public void HelloWorld()
        {
            _logger.Information("Hello World.");
        }
    }
}
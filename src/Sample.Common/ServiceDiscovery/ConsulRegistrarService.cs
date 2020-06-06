using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Serilog;

namespace Sample.Common.ServiceDiscovery
{
    public class ConsulRegistrarService: IConsulRegistrarService
    {
        private CancellationTokenSource _cts;
        private string _registrationId;
        private readonly ILogger _logger;
        private readonly IConsulClient _consulClient;
        public ConsulRegistrarService(ILogger logger, IConsulClient consulClient)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _consulClient = consulClient ?? throw new ArgumentException(nameof(consulClient)); ;
        }
        public async Task StartAsync(Models.Consul consulConfig,IFeatureCollection features,CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();

            var uri = new Uri(address);
            _registrationId = $"{consulConfig.ServiceId}-{uri.Port}";

            var registration = new AgentServiceRegistration()
            {
                ID = _registrationId,
                Name = consulConfig.ServiceName,
                Address = consulConfig.ServiceAddress, // TODO: Maybe this shouldn't be config?
                Port = uri.Port,
                Tags = consulConfig.ServiceTags
            };

            _logger.Information("Registering in Consul");
            await _consulClient.Agent.ServiceDeregister(registration.ID, _cts.Token);
            await _consulClient.Agent.ServiceRegister(registration, _cts.Token);
        }

        public async Task StopAsync( CancellationToken cancellationToken)
        {
            _cts.Cancel();
            _logger.Information("De-registering from Consul");
            try
            {
                await _consulClient.Agent.ServiceDeregister(_registrationId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"De-registration failed");
            }
        }
    }
}
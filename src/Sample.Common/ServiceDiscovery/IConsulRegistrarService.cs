using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace Sample.Common.ServiceDiscovery
{
    public interface IConsulRegistrarService
    {
        Task StartAsync(Models.Consul consulConfig, IFeatureCollection features, CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
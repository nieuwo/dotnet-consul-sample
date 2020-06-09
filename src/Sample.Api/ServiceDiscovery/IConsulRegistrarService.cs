using Microsoft.AspNetCore.Http.Features;
using System.Threading;
using System.Threading.Tasks;

namespace Sample.Api.ServiceDiscovery
{
    public interface IConsulRegistrarService
    {
        Task StartAsync(Api.Models.Consul consulConfig, IFeatureCollection features, CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
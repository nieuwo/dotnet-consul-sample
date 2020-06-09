using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sample.Api.ServiceDiscovery;
using Serilog;
using System.Threading;

namespace Sample.Api.Extensions
{
    public static class HostApplicationExtension
    {
        public static void UseApplicationLifetimeTriggers(this IApplicationBuilder app, IHostApplicationLifetime lifetime, IConfiguration configuration, IConsulRegistrarService consulRegistrarService)
        {
            var cancelToken = new CancellationTokenSource();
            var consulConfig = configuration.GetSection(nameof(Models.Consul)).Get<Models.Consul>();
            lifetime.ApplicationStarted.Register(async () => await consulRegistrarService.StartAsync(consulConfig, app.ServerFeatures, cancelToken.Token));
            lifetime.ApplicationStopping.Register(async () =>
            {
                await consulRegistrarService.StopAsync(cancelToken.Token);
                Log.CloseAndFlush();
            });
        }
    }
}
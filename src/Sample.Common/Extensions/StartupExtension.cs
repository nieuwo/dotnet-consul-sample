using System;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sample.Common.Extensions
{
    public static class StartupExtension
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Common.Models.Consul>(configuration.GetSection(nameof(Common.Models.Consul)));
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var options = p.GetService<IOptions<Common.Models.Consul>>();
                consulConfig.Address = new Uri(options.Value.Address);
            }));

            return services;
        }
    }
}
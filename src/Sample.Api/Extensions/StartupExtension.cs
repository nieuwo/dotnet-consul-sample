using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Sample.Api.ServiceDiscovery;
using System;
using System.IO;
using System.Reflection;

namespace Sample.Api.Extensions
{
    public static class StartupExtension
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Api.Models.Consul>(configuration.GetSection(nameof(Api.Models.Consul)));
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
            {
                var options = p.GetService<IOptions<Api.Models.Consul>>();
                consulConfig.Address = new Uri(options.Value.Address);
            }));

            return services;
        }

        public static IServiceCollection AddHostedService(this IServiceCollection services)
        {
            services.AddHostedService<LeaderElectionService>();

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Sample Api", Version = "v1"});
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }
    }
}
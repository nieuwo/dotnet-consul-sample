using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Sample.Common.ServiceDiscovery;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Polly;
using Polly.Extensions.Http;
using Sample.Infrastructure.Integration.Forex;

namespace Sample.Api.Extensions
{
    public static class StartupExtension
    {
        public static IServiceCollection AddHostedService(this IServiceCollection services)
        {
            services.AddHostedService<LeaderElectionService>();

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample Api", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        public static IServiceCollection AddCustomHttpClient(this IServiceCollection services)
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
            ;

            services.AddHttpClient<IForexAgent, ForexAgent>(c =>
                {
                    c.BaseAddress = new Uri("https://api.exchangeratesapi.io/");
                    c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
                })
                .AddPolicyHandler(retryPolicy);
            
            return services;
        }
    }
}
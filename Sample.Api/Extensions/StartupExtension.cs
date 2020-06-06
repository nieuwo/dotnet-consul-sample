using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Sample.Api.Extensions
{
    public static class StartupExtension
    {
        public static IServiceCollection AddCustom(this IServiceCollection services)
        {
            return services;
        }

        public static IConfigurationBuilder AddSerilog(this IConfigurationBuilder config)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config.Build())
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .WriteTo.Console()
                .CreateLogger();

            return config;
        }
    }
}
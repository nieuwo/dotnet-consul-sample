using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sample.Api.DependencyInjection;
using Sample.Api.Extensions;
using Sample.Common.Extensions;
using Sample.Common.ServiceDiscovery;
using Sample.Infrastructure.Integration.Forex;
using Serilog;

namespace Sample.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IConfiguration _configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConsul(_configuration);

            services.AddHostedService();

            services.AddControllers();

            services.AddHealthChecks();

            services.AddCustomSwagger();

            services.AddCustomHttpClient();

            services.AddAutofac();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());
            builder.RegisterModule(new DomainModule());
            builder.RegisterModule(new InfrastructureModule());
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime,
            IConsulRegistrarService consulRegistrarService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample Api V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            app.UseApplicationLifetimeTriggers(lifetime, _configuration, consulRegistrarService);
        }
    }
}
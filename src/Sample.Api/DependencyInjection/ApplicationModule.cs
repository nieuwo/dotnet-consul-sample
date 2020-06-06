using Autofac;
using Consul;
using Microsoft.Extensions.Options;
using Sample.Domain.Contracts;
using Sample.Domain.Services;
using System;
using Sample.Common.ServiceDiscovery;
using Sample.Infrastructure.Integration.Forex;

namespace Sample.Api.DependencyInjection
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ConsulRegistrarService>().As<IConsulRegistrarService>();
        }
    }

    public class DomainModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<RatesService>().As<IRatesService>();
        }
    }

    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
        }
    }
}
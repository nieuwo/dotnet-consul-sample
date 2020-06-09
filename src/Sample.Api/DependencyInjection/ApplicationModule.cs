using Autofac;
using Sample.Api.ServiceDiscovery;

namespace Sample.Api.DependencyInjection
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<ConsulRegistrarService>().As<IConsulRegistrarService>();
        }
    }
}
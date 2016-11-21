using Autofac;
using Report.Logging;
using Report.Services;

namespace Report.Presentation
{
    public static class ApplicationContainer
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Application>().As<IApplication>();
            builder.RegisterType<BasicLogging>().As<ILogging>();
            builder.RegisterType<CacheWrapper>().As<ICacheWrapper>();
            builder.RegisterType<ConfigurationServices>().As<IConfigurationServices>();
            builder.RegisterType<ExternalServiceWrapper>().As<IExternalServiceWrapper>();
            builder.RegisterType<FloodMonitoringServices>().As<IFloodMonitoringServices>();

            return builder.Build();
        }
    }
}

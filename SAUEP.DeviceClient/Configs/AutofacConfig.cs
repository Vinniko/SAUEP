using Autofac;
using SAUEP.DeviceClient.Interfaces;
using SAUEP.DeviceClient.Services;
using SAUEP.DeviceClient.Repositories;
using SAUEP.DeviceClient.Connections;

namespace SAUEP.DeviceClient.Configs
{
    public sealed class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Guardian>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<DataBaseConnection>().As<IConnection>().SingleInstance();
            builder.RegisterType<DeviceRepository>().As<IRepository>().SingleInstance();
            builder.RegisterType<PollRepository>().AsSelf().SingleInstance();
            builder.RegisterType<ConsoleWriter>().As<IWriter>().SingleInstance();
            builder.RegisterType<JSONParser>().As<IParser>().SingleInstance();
            builder.RegisterType<SocketWriter>().AsSelf().SingleInstance();
            builder.RegisterType<ResultGenerator>().AsSelf().SingleInstance();
            builder.RegisterType<ResultGenerator>().AsSelf().SingleInstance();
            builder.RegisterType<FileReader>().As<IReader>().SingleInstance();

            return builder.Build();
        }
    }
}

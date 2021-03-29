using Autofac;
using SAUEP.TCPServer.Interfaces;
using SAUEP.TCPServer.Services;
using SAUEP.TCPServer.Repositories;
using SAUEP.TCPServer.Connections;

namespace SAUEP.TCPServer.Configs
{
    public sealed class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Guardian>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<DataBaseConnection>().As<IConnection>().SingleInstance();
            builder.RegisterType<PollsRepository>().As<IRepository>().SingleInstance();
            builder.RegisterType<ConsoleWriter>().As<IWriter>().SingleInstance();
            builder.RegisterType<JSONParser>().As<IParser>().SingleInstance();
            builder.RegisterType<SocketListener>().As<IListener>().SingleInstance();
            builder.RegisterType<SocketWriter>().As<SocketWriter>().SingleInstance();
            builder.RegisterType<FileReader>().As<IReader>().SingleInstance();
            builder.RegisterType<UserRepository>().AsSelf().SingleInstance();
            builder.RegisterType<DeviceRepository>().AsSelf().SingleInstance();
            builder.RegisterType<EmailSender>().As<ISender>().SingleInstance();

            return builder.Build();
        }
    }
}

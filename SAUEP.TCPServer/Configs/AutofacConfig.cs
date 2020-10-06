using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using SAUEP.TCPServer.Interfaces;
using SAUEP.TCPServer.Services;

namespace SAUEP.TCPServer.Configs
{
    public sealed class AutofacConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Guardian>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

            return builder.Build();
        }
    }
}

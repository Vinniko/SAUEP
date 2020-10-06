using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using System.Web.Mvc;
using Core.Interfaces;
using Core.Services;

namespace SAUEP.WPF.Configs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Autofac;
using Autofac;
using Prism.Events;
using SAUEP.WPF.Views;

namespace SAUEP.WPF.Configs
{
    public sealed class Bootstrapper : AutofacBootstrapper
    {

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<SAUEPWindow>();
        }
        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override ContainerBuilder CreateContainerBuilder()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<Guardian>().SingleInstance();
            //builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            //builder.RegisterType<DataBaseConnection>().As<IConnection>().SingleInstance();
            //builder.RegisterType<Sorter>().As<ISorter>().SingleInstance();
            //builder.RegisterType<Filter>().As<IFilter>().SingleInstance();
            //builder.RegisterType<JewelRepository>().As<JewelRepository>().SingleInstance();
            //builder.RegisterType<OrderRepository>().As<OrderRepository>().SingleInstance();
            //builder.RegisterType<ShopPageViewModel>().AsSelf().SingleInstance();
            //builder.RegisterType<BasketPageViewModel>().AsSelf().SingleInstance();
            //builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();
            //builder.RegisterType<JewelPageViewModel>().AsSelf().SingleInstance();
            //builder.RegisterType<ChangePageViewModel>().AsSelf().SingleInstance();
            //builder.RegisterType<AddPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            return builder;
        }
    }
}

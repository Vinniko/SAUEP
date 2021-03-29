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
using SAUEP.Core.Interfaces;
using SAUEP.Core.Services;
using SAUEP.Core.Repositories;
using SAUEP.Core.Models;
using SAUEP.Core.Connections;
using SAUEP.WPF.ViewModels;
using SAUEP.WPF.Services;

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

            builder.RegisterType<Guardian>().As<IGuardian>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();
            builder.RegisterType<AuthorizationService>().As<IAuthorization>().SingleInstance();
            builder.RegisterType<LoginValidation>().AsSelf().SingleInstance();
            builder.RegisterType<PasswordValidation>().AsSelf().SingleInstance();
            builder.RegisterType<ServerConnection>().As<IConnection>().SingleInstance();
            builder.RegisterType<JsonParser>().As<IParser>().SingleInstance();
            builder.RegisterType<FileReader>().As<IReader>().SingleInstance();
            builder.RegisterType<UserModel>().As<IModel>().SingleInstance().UsingConstructor().ExternallyOwned();
            builder.RegisterType<UserRepository>().AsSelf();
            builder.RegisterType<InternetConnectionChecker>().AsSelf().SingleInstance();
            builder.RegisterType<DeviceGroupRepository>().AsSelf().SingleInstance();
            builder.RegisterType<DeviceRepository>().AsSelf().SingleInstance();
            builder.RegisterType<DispatcherPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<SocketListener>().As<IListener>().SingleInstance();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<DeviceGroupModelViewingSorter>().AsSelf().SingleInstance();
            builder.RegisterType<DeviceGroupModelViewingFilter>().AsSelf().SingleInstance();
            builder.RegisterType<DeviceDispatcherPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<DeviceModelViewingFilter>().AsSelf().SingleInstance();
            builder.RegisterType<DeviceModelViewingSorter>().AsSelf().SingleInstance();
            builder.RegisterType<DevicePageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<DeviceGroupHistoryPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<DeviceHistoryPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<DeviceModelsSorter>().AsSelf().SingleInstance();
            builder.RegisterType<OneDeviceHistoryPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<ExpensesPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<CreateDeviceGroupPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<UpdateDeviceGroupPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<CreateDevicePageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<UpdateDevicePageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<CreateDevicePollPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<UsersPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<UsersSorter>().AsSelf().SingleInstance();
            builder.RegisterType<UsersFilter>().AsSelf().SingleInstance();
            builder.RegisterType<UserPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<RoleRepository>().AsSelf().SingleInstance();
            builder.RegisterType<CreateUserPageViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<UpdateUserPageViewModel>().AsSelf().SingleInstance();

            return builder;
        }
    }
}

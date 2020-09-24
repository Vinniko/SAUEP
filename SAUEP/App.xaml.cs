using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Autofac.Core;
using Core.Services;

namespace SAUEP
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            IContainer container = AutofacConfig.ConfigureContainer();
            //var scenarioReader = new ScenarioReader(container.Resolve<Guardian>());
            //scenarioReader.Read("f");

            //var orfografferView = new OrfografferView(container.Resolve<BaseVM>() as OrfografferViewModel);
            //orfografferView.Show();
        }
    }
}

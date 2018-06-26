using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Practices.Unity;

using Prism.Mvvm;
using Prism.Unity;

using MakePdf.Wpf.Views;
using MakePdf.Wpf.Views.Pages;

namespace MakePdf.Wpf
{
    enum Region
    {
        MenuRegion,
        MainRegion,
    }

    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            //ViewModelLocator.SetAutoWireViewModel(Shell, true); 

            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // Register all Views (to use RegionManager)
            Container.RegisterTypeForNavigation<Menu>(nameof(Menu));
            Container.RegisterTypeForNavigation<Home>(nameof(Home));
            Container.RegisterTypeForNavigation<EasyMode>(nameof(EasyMode));
            Container.RegisterTypeForNavigation<StandardMode>(nameof(StandardMode));

            //    this.Container.RegisterTypes(
            //        AllClasses.FromLoadedAssemblies().Where(t => t.Namespace.EndsWith(".Views")),
            //        getFromTypes: _ => new[] { typeof(object) },
            //        getName: WithName.TypeName);
        }
    }
}

using AirStack.Client.Services.Settings;
using AirStack.Client.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AirStack.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Locator.Initialize();

            MainWindow = Locator.Resolve<MainView>();

            base.OnStartup(e);

            MainWindow.Show();

            MainWindow.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var toDispose = Locator.Resolve<IEnumerable<IDisposable>>();
            foreach (var item in toDispose)
                item.Dispose();
        }
    }
}

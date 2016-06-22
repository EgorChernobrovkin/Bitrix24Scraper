using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using ScraperLogic;

namespace BitrixScraperWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            var controller = new ScraperController();
            var window = new MainWindow(controller);
            window.ShowDialog();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var controller = new ScraperController();
            var window = new MainWindow(controller);
            window.ShowDialog();
        }
    }
}

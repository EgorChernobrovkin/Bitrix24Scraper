using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using ScraperLogic;
using WatiN.Core;
using Task = ScraperLogic.Models.Task;

namespace BitrixScraperWpf
{
    using System;
    using System.Windows;
    using ScraperLogic.Repository;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScraperController _controller;

        public MainWindow(ScraperController controller)
        {
            InitializeComponent();
            _controller = controller;
            DataContext = controller;
        }

        private void TasksListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var taskListView = sender as ListView;
            var task = taskListView?.SelectedItem as Task;
            if (task != null)
            {
                
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            using (var browser = new IE())
            {
                await System.Threading.Tasks.Task.Factory.StartNew(() => { _controller.RefreshTasks(browser); });
            }

            IsEnabled = true;
        }
    }
}
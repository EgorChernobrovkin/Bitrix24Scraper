

namespace BitrixScraperWpf
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using ObjectDumper;

    using ScraperLogic;
    using WatiN.Core;
    using Task = ScraperLogic.Models.Task;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ScraperController _controller;

        public MainWindow(ScraperController controller)
        {
            InitializeComponent();
            _controller = controller;
            DataContext = controller;
        }

        private void TasksListView_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var taskListView = sender as ListView;
            if (taskListView == null)
            {
                return;
            }

            var task = taskListView.SelectedItem as Task;

            if (task != null)
            {
                var window = new TaskInfoWindow(task) {Owner = this};
                window.ShowDialog();
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

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            XmlTaskDatabase.Instance.Save();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            XmlTaskDatabase.Instance.Save();
        }
    }
}
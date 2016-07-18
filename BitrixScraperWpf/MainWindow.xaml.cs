using System;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ScraperLogic;
using WatiN.Core;
using Task = ScraperLogic.Models.Task;

namespace BitrixScraperWpf
{
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
            this.Dispatcher.Invoke(() =>
                {
                    CollectionViewSource.GetDefaultView(this.TasksListView.ItemsSource)
                        .SortDescriptions.Add(new SortDescription("CustomStatus", ListSortDirection.Ascending));
                    CollectionViewSource.GetDefaultView(this.TasksListView.ItemsSource).Refresh();
                });
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            XmlTaskDatabase.Instance.Save();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(this.TasksListView.ItemsSource)
                .SortDescriptions.Add(new SortDescription("CustomStatus", ListSortDirection.Ascending));
            CollectionViewSource.GetDefaultView(this.TasksListView.ItemsSource).Refresh();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            XmlTaskDatabase.Instance.Save();
        }
        
        private void DeleteItem_OnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null)
            {
                return;
            }

            var task = menuItem.DataContext as Task;
            if (task == null)
            {
                return;
            }

            var dialogResult = MessageBox.Show(
                "Удалить задачу из списка?",
                "Предупреждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (dialogResult != MessageBoxResult.Yes)
            {
                return;
            }

            this._controller.DeleteTask(task);
            this.Dispatcher.Invoke(() =>
            {
                CollectionViewSource.GetDefaultView(this.TasksListView.ItemsSource)
                    .SortDescriptions.Add(new SortDescription("CustomStatus", ListSortDirection.Ascending));
                CollectionViewSource.GetDefaultView(this.TasksListView.ItemsSource).Refresh();
            });
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(this.TasksListView.ItemsSource).Refresh();
        }
    }
}
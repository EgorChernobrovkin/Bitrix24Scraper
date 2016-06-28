using System.Windows;

namespace BitrixScraperWpf
{
    using System.Diagnostics;
    using System.Windows.Documents;
    using System.Windows.Navigation;

    using ScraperLogic;

    using WatiN.Core;

    using Task = ScraperLogic.Models.Task;

    /// <summary>
    /// Interaction logic for TaskInfoWindow.xaml
    /// </summary>
    public partial class TaskInfoWindow : Window
    {
        public TaskInfoWindow(Task task)
        {
            InitializeComponent();
            this.DataContext = task;

            
        }

        private async void UpdateInfoButton_Click(object sender, RoutedEventArgs e)
        {
            using (var browser = new IE())
            {
                var task = (Task)this.DataContext;
                await System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    TaskScraper.UpdateTaskInfo(task, browser);
                }); 
            }
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}

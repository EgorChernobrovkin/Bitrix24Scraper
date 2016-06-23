using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BellaCode.Collections.ObjectModel;
using ScraperLogic.Annotations;
using ScraperLogic.Models;
using ScraperLogic.Repository;
using ScraperLogic.Repository.Utility;
using WatiN.Core;

namespace ScraperLogic
{
    public class ScraperController : INotifyPropertyChanged
    {
        public ObservableHashSet<Task> Tasks;

        public event PropertyChangedEventHandler PropertyChanged;

        public ScraperController()
        {
            Tasks = new ObservableHashSet<Task>(XmlTaskDatabase.Instance.Tasks);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RefreshTasks(Browser browser)
        {
            var tasks = new HashSet<Task>();
            
            var taskLinks = TaskScraper.GetAllLinks(browser);
            foreach (var link in taskLinks)
            {
                var task = TaskScraper.GetTaskInfo(link, browser);
                tasks.Add(task);
            }

            browser.Dispose();

            foreach (var task in tasks)
            {
                if (XmlTaskDatabase.Instance.Tasks.Contains(task))
                {
                    XmlTaskDatabase.Instance.Tasks.Remove(task);
                }

                XmlTaskDatabase.Instance.Tasks.Add(task);
            }

            XmlTaskDatabase.Instance.Save();
        }
    }
}

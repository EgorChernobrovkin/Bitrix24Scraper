using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BellaCode.Collections.ObjectModel;
using ScraperLogic.Annotations;
using ScraperLogic.Models;

using WatiN.Core;

namespace ScraperLogic
{
    using System.Linq;
    using System.Windows.Data;

    public sealed class ScraperController : INotifyPropertyChanged
    {
        public ObservableHashSet<Task> Tasks { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ScraperController()
        {
            this.Tasks = new ObservableHashSet<Task>(XmlTaskDatabase.Instance.Tasks);
            this.OnPropertyChanged("Tasks");
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void RefreshTasks(Browser browser)
        {
            var taskSet = new HashSet<Task>();
            
            var taskLinks = TaskScraper.GetAllLinks(browser);

            foreach (var link in taskLinks)
            {
                var task = XmlTaskDatabase.Instance.Tasks.FirstOrDefault(t => string.Equals(t.Link, link));
                
                if (task == null)
                {
                    task = TaskScraper.GetTaskInfo(link, browser);
                }
                else
                {
                    TaskScraper.UpdateTaskInfo(task, browser);
                }

                taskSet.Add(task);
            }
            
            foreach (var task in taskSet)
            {
                if (XmlTaskDatabase.Instance.Tasks.Contains(task))
                {
                    XmlTaskDatabase.Instance.Tasks.Remove(task);
                }

                XmlTaskDatabase.Instance.Tasks.Add(task);
            }

            XmlTaskDatabase.Instance.Save();

            this.Tasks = new ObservableHashSet<Task>(XmlTaskDatabase.Instance.Tasks);
            this.OnPropertyChanged("Tasks");
        }

        public bool DeleteTask(Task task)
        {
            var result = XmlTaskDatabase.Instance.Tasks.Remove(task);
            XmlTaskDatabase.Instance.Save();

            this.Tasks = new ObservableHashSet<Task>(XmlTaskDatabase.Instance.Tasks);
            this.OnPropertyChanged("Tasks");

            return result;
        }
    }
}

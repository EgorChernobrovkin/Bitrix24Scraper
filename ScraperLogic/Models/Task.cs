using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScraperLogic.Annotations;

namespace ScraperLogic.Models
{
    using Enums;
    
    /// <summary>
    /// Информация о задаче
    /// </summary>
    public sealed class Task : INotifyPropertyChanged

    {
        private int _id;
        private string _link;
        private string _title;
        private string _description;
        private string _status;
        private TaskCustomStatus _customStatus = TaskCustomStatus.NotSet;

        public Task()
        {
            
        }

        public Task(Task task)
        {
            Id = task.Id;
            Link = task.Link;
            Title = task.Title;
            Description = task.Description;
            Status = task.Status;
            CustomStatus = task.CustomStatus;
        }

        /// <summary>
        /// Номер задачи
        /// </summary>
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        /// <summary>
        /// Ссылка на задачу
        /// </summary>
        public string Link
        {
            get { return _link; }
            set
            {
                _link = value;
                OnPropertyChanged(nameof(Link));
            }
        }

        /// <summary>
        /// Название задачи
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        /// <summary>
        /// HTML описания задачи
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        /// <summary>
        /// Статус задачи
        /// </summary>
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        /// <summary>
        /// Кастомный статус для задачи
        /// </summary>
        public TaskCustomStatus CustomStatus
        {
            get { return _customStatus; }
            set
            {
                _customStatus = value;
                OnPropertyChanged(nameof(CustomStatus));
            }
        }

        public override bool Equals(object obj)
        {
            var task = obj as Task;
            if (task != null)
            {
                return Id == task.Id;
            }
            return obj.Equals(this);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

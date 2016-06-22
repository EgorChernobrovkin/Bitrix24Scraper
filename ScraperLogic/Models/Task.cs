namespace ScraperLogic.Models
{
    using Enums;
    
    /// <summary>
    /// Информация о задаче
    /// </summary>
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class Task
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
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
        public int Id { get; set; }

        /// <summary>
        /// Ссылка на задачу
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Название задачи
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// HTML описания задачи
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Статус задачи
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Кастомный статус для задачи
        /// </summary>
        public TaskCustomStatus CustomStatus { get; set; } = TaskCustomStatus.NotSet;

        public override bool Equals(object obj)
        {
            var task = obj as Task;
            if (task != null)
            {
                return Id == task.Id;
            }
            return obj.Equals(this);
        }
    }
}

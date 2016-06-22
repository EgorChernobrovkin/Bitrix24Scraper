namespace ScraperLogic.Models
{
    using ScraperLogic.Models.Enums;

    /// <summary>
    /// Информация о задаче
    /// </summary>
    public class Task
    {
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
    }
}

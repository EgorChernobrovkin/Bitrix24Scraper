namespace ScraperLogic.Models.Enums
{
    using System.ComponentModel;

    /// <summary>
    /// Пользовательские статусы для задач
    /// </summary>
    public enum TaskCustomStatus
    {
        /// <summary>
        /// Не задан
        /// </summary>
        [Description("Не задан")]
        NotSet,

        /// <summary>
        /// В процессе
        /// </summary>
        [Description("Выполняю")]
        Doing,

        /// <summary>
        /// Просто наблюдаю
        /// </summary>
        [Description("Наблюдаю")]
        Watching,

        /// <summary>
        /// Задача отложена
        /// </summary>
        [Description("Отложена")]
        Suspended,

        /// <summary>
        /// На тестировании
        /// </summary>
        [Description("На тестировании")]
        OnTesting,

        /// <summary>
        /// Ждет выпуска в обновлении
        /// </summary>
        [Description("Ждет выпуска")]
        WaitingForRelease,

        /// <summary>
        /// Задача завершена
        /// </summary>
        [Description("Завершена")]
        Finished
    }
}

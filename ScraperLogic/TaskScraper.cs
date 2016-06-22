namespace ScraperLogic
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using WatiN.Core;

    /// <summary>
    /// Собирает информацию о задачах из Битрикс24
    /// </summary>
    public static class TaskScraper
    {
        /// <summary>
        /// Время ожидания, пока выполнятся ajax запросы на странице
        /// </summary>
        private const int PageloadTimeout = 2000;

        /// <summary>
        /// Собрать всю информацию о задачах
        /// </summary>
        public static void Scrape()
        {
            using (var browser = new IE("https://tns.bitrix24.ru/") { AutoClose = true })
            {
                // Ждем загрузки главной страницы и переходим на страницу с задачами
                browser.WaitForComplete();
                Thread.Sleep(PageloadTimeout);
                LoadTasksPage(browser);

                var taskLinks = CollectTaskLinks(browser);
                
#if DEBUG
                foreach (var link in taskLinks)
                {
                    Debug.WriteLine(link);
                }
#endif
            }
        }
        
        /// <summary>
        /// Собирает все ссылки на задачи со всех разделов
        /// </summary>
        /// <param name="browser">Браузер, в котором выполняется анализ</param>
        /// <returns>Список ссылок на страницы с задачами</returns>
        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        private static HashSet<string> CollectTaskLinks(DomContainer browser)
        {
            var taskLinks = new HashSet<string>();

            // Проходимся по всем разделам
            foreach (var buttonText in new[] { "Делаю", "Помогаю", "Поручил", "Наблюдаю" })
            {
                LoadTasksSection(buttonText, browser);

                // Находим нужные строки в таблице
                var taskTableRows = browser.TableRows.Where(
                    row =>
                        row.ClassName != null
                        && row.ClassName.Contains("task-list-item")
                        && !row.ClassName.Contains("task-list-project-item")
                        && row.Id != "task-list-no-tasks");

                // Из каждой строки забираем ссылку
                // ReSharper disable once LoopCanBePartlyConvertedToQuery
                foreach (var taskRow in taskTableRows)
                {
                    var linkElem = taskRow.Link(Find.ByClass("task-title-link"));
                    if (linkElem == null)
                    {
                        continue;
                    }

                    var link = linkElem.GetAttributeValue("href");
                    if (!string.IsNullOrWhiteSpace(link))
                    {
                        taskLinks.Add(link);
                    }
                }
            }

            return taskLinks;
        }
        
        /// <summary>
        /// Ищет кнопку "Задачи" в левом меню и переходит на страницу с задачами
        /// </summary>
        /// <param name="browser">Браузер, в котором выполняется анализ</param>
        private static void LoadTasksPage(DomContainer browser)
        {
            // Ищем кнопку "Задачи"
            var tasksLink = browser.Links.FirstOrDefault(link =>
                link.ClassName.Contains("menu-item-link")
                && link.Children().FirstOrDefault(element =>
                    element != null
                    && !string.IsNullOrEmpty(element.Text)
                    && element.Text.Contains("Задачи"))
                != null);
            Debug.Assert(tasksLink != null, "Кнопка Задачи не найдена");
            tasksLink.Click();
            browser.WaitForComplete();
            Thread.Sleep(PageloadTimeout);
        }

        /// <summary>
        /// Переход на указанный раздел страницы с задачами
        /// </summary>
        /// <param name="buttonText">Нужный раздел</param>
        /// <param name="browser">Браузер, в котором выполняется анализ</param>
        /// <remarks>buttonText - текст на кнопке с разделом. Например, "Делаю" или "Помогаю"</remarks>
        private static void LoadTasksSection(string buttonText, DomContainer browser)
        {
            var sectionLink = browser.Links.FirstOrDefault(link =>
                link.ClassName.Contains("tasks-top-item")
                && link.Children().FirstOrDefault(elem =>
                    elem != null
                    && elem.Text != null
                    && elem.Text == buttonText) != null);
            Debug.Assert(sectionLink != null, "Кнопка " + buttonText + " не найдена");
            sectionLink.Click();
            browser.WaitForComplete();
            Thread.Sleep(PageloadTimeout);
        }
    }
}
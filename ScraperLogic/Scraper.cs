using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using WatiN.Core;

namespace ScraperLogic
{
    public static class TaskScraper
    {
        private const int PageloadTimeout = 2000;

        public static void Scrape()
        {
            using (var browser = new IE("https://tns.bitrix24.ru/") {AutoClose = true})
            {
                // Ждем загрузки главной страницы и переходим на страницу с задачами
                Thread.Sleep(PageloadTimeout);
                loadTasksPage(browser);

                var taskLinks = collectTaskLinks(browser);
                
#if DEBUG
                foreach (var link in taskLinks)
                {
                    Debug.WriteLine(link);
                }
#endif
            }
        }
        
        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        /// <summary>
        /// Собирает все ссылки на задачи со всех разделов
        /// </summary>
        /// <param name="browser"></param>
        /// <returns></returns>
        private static HashSet<string> collectTaskLinks(IElementContainer browser)
        {
            var taskLinks = new HashSet<string>();
            // Проходимся по всем разделам
            foreach (var buttonText in new[] { "Делаю", "Помогаю", "Поручил", "Наблюдаю" })
            {
                loadTasksSection(buttonText, browser);

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
                    if (linkElem == null) continue;

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
        /// <param name="browser"></param>
        private static void loadTasksPage(IElementContainer browser)
        {
            // Ищем кнопку "Задачи"
            var tasksLink = browser.Links.FirstOrDefault(link =>
                link.ClassName.Contains("menu-item-link")
                && link.Children().FirstOrDefault(element =>
                    element != null
                    && !string.IsNullOrEmpty(element.Text)
                    && element.Text.Contains("Задачи"))
                != null);
            Debug.Assert(tasksLink != null);

            tasksLink.Click();
            Thread.Sleep(PageloadTimeout);
        }

        /// <summary>
        /// Переход на указанный раздел страницы с задачами
        /// </summary>
        /// <param name="buttonText">Нужный раздел</param>
        /// <param name="browser"></param>
        /// <remarks>buttonText - текст на кнопке с разделом. Например, "Делаю" или "Помогаю"</remarks>
        private static void loadTasksSection(string buttonText, IElementContainer browser)
        {
            
            var doingLink = browser.Links.FirstOrDefault(link =>
                link.ClassName.Contains("tasks-top-item")
                && link.Children().FirstOrDefault(elem =>
                    elem != null
                    && elem.Text != null
                    && elem.Text == buttonText) != null);
            Debug.Assert(doingLink != null);
            doingLink.Click();
            Thread.Sleep(PageloadTimeout);
        }

    }
}

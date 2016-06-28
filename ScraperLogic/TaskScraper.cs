namespace ScraperLogic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;

    using ScraperLogic.Models;
    using ScraperLogic.Models.Enums;

    using WatiN.Core;
    using WatiN.Core.Exceptions;

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
        /// Собрать список ссылок на все задачи
        /// </summary>
        /// <param name="browser">Браузер, в котором выполняется анализ</param>
        /// <returns>Список ссылок на страницы с задачами</returns>
        public static HashSet<string> GetAllLinks(Browser browser)
        {
            browser.GoTo("https://tns.bitrix24.ru/");
            browser.WaitForComplete();
            
            // Ждем загрузки главной страницы и переходим на страницу с задачами
            browser.WaitForComplete();
            Thread.Sleep(PageloadTimeout);
            LoadTasksPage(browser);

            var taskLinks = CollectTaskLinks(browser);

            return taskLinks;
        }

        /// <summary>
        /// Получить информацию о задаче
        /// </summary>
        /// <param name="link">Ссылка на задачу</param>
        /// <param name="browser">Браузер, в котором выполняется анализ</param>
        /// <returns>Объект с информацией о задаче</returns>
        public static Task GetTaskInfo(string link, Browser browser)
        {
            var task = new Task
            {
                Link = link,
                CustomStatus = TaskCustomStatus.NotSet
            };

            UpdateTaskInfo(task, browser);

            return task;
        }

        /// <summary>
        /// Обновить информацию о задаче
        /// </summary>
        /// <param name="task">Задача для обновления</param>
        /// <param name="browser">Браузер, в котором выполняется анализ</param>
        public static void UpdateTaskInfo(Task task, Browser browser)
        {
            var link = task.Link;

            browser.GoTo(link);
            browser.WaitForComplete();
            Thread.Sleep(PageloadTimeout);

            var titleSpan = browser.Span(Find.ByClass("pagetitle-inner"));
            Debug.Assert(titleSpan != null, "Не найден заголовок задачи");

            var titleRegex = new Regex(@"^(?<title>.+) \(задача №(?<id>\d+)\)$");
            var match = titleRegex.Match(titleSpan.Text);
            Debug.Assert(match.Success, "Не удалось распарсить заголовок");

            int id;
            var parseResult = int.TryParse(match.Groups["id"].Value, out id);
            Debug.Assert(parseResult, "Не удалось распарсить номер задачи");
            var title = match.Groups["title"].Value;

            var descriptionDiv = browser.Div(Find.ById("task-detail-description"));
            Debug.Assert(descriptionDiv != null, "Не найдено описание задачи");
            string descriptionHtml;
            try
            {
                descriptionHtml = descriptionDiv.InnerHtml;
            }
            catch (ElementNotFoundException)
            {
                descriptionHtml = string.Empty;
            }

            var statusSpan = browser.Span(Find.ById("task-detail-status-name"));
            Debug.Assert(statusSpan != null, "Не удалось найти статус");
            var status = statusSpan.Text;

            task.Id = id;
            task.Title = title;
            task.Description = descriptionHtml;
            task.Status = status;
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
            Link tasksLink;
            do
            {
                browser.WaitForComplete();
                tasksLink = browser.Links.FirstOrDefault(link =>
                    link.ClassName != null
                    && link.ClassName.Contains("menu-item-link")
                    && link.Children().FirstOrDefault(element =>
                        element != null 
                        && !string.IsNullOrEmpty(element.Text)
                        && element.Text.Contains("Задачи"))
                    != null);
            } while (tasksLink == null);
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
                    && elem.Text == buttonText)
                != null);
            Debug.Assert(sectionLink != null, "Кнопка " + buttonText + " не найдена");
            sectionLink.Click();
            browser.WaitForComplete();
            Thread.Sleep(PageloadTimeout);
        }
    }
}
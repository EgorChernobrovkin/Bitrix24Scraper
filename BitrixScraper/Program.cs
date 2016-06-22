using System;
using ScraperLogic;

namespace BitrixScraper
{
    using System.Diagnostics;

    using WatiN.Core;
    using WatiN.Core.Exceptions;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        [STAThread]
        // ReSharper disable once InconsistentNaming
        private static void Main(string[] args)
        {
            using (var browser = new IE())
            {
                var taskLinks = TaskScraper.GetAllLinks(browser);
                foreach (var link in taskLinks)
                {
                    var task = TaskScraper.GetTaskInfo(link, browser);
                    Debug.WriteLine(task.Link);
                    Debug.WriteLine(task.Id + ": " + task.Title);
                    Debug.WriteLine(task.Status);
                    Debug.WriteLine(task.Description);
                }
            }
        }
    }
}

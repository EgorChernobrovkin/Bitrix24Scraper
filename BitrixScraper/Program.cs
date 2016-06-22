using System;
using System.Collections.Generic;
using ScraperLogic;
using ScraperLogic.Models;
using ScraperLogic.Repository;
using ScraperLogic.Repository.Utility;

namespace BitrixScraper
{
    using WatiN.Core;

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        [STAThread]
        // ReSharper disable once InconsistentNaming
        private static void Main(string[] args)
        {
            var tasks = new HashSet<Task>();
            using (var browser = new IE())
            {
                var taskLinks = TaskScraper.GetAllLinks(browser);
                foreach (var link in taskLinks)
                {
                    var task = TaskScraper.GetTaskInfo(link, browser);
                    tasks.Add(task);
                }
            }

            foreach (var task in tasks)
            {
                if (XmlTaskDatabase.Instance.Tasks.Contains(task))
                {
                    XmlTaskDatabase.Instance.Tasks.Remove(task);
                }

                XmlTaskDatabase.Instance.Tasks.Add(task);
            }
            
        }
    }
}

using System;
using ScraperLogic;

namespace BitrixScraper
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        [STAThread]
        // ReSharper disable once InconsistentNaming
        private static void Main(string[] args)
        {
            TaskScraper.Scrape();
        }
    }
}

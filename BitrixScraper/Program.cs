using System;
using ScraperLogic;

namespace BitrixScraper
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            TaskScraper.Scrape();
        }
    }
}

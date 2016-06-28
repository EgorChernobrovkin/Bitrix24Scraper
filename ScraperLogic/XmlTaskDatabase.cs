namespace ScraperLogic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Xml.Serialization;

    using ScraperLogic.Models;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class XmlTaskDatabase
    {
        private const string SaveFileName = "tasksdb.xml";

        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<XmlTaskDatabase> _instance= new Lazy<XmlTaskDatabase>(() => new XmlTaskDatabase(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static XmlTaskDatabase Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public HashSet<Task> Tasks { get; private set; }

        private XmlTaskDatabase()
        {
            if (!File.Exists(SaveFileName))
            {
                this.Tasks = new HashSet<Task>();
            }
            else
            {
                using (var dbStream = new FileStream(SaveFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var dbReader = new StreamReader(dbStream, Encoding.UTF8);
                    var xmlSerializer = new XmlSerializer(typeof (HashSet<Task>));

                    try
                    {
                        this.Tasks = (HashSet<Task>) xmlSerializer.Deserialize(dbReader);
                    }
                    catch
                    {
                        this.Tasks = new HashSet<Task>();
                    }
                }
            }
        }

        public void Save()
        {
            using (var dbStream = new FileStream(SaveFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var tasks = new HashSet<Task>(this.Tasks);
                var dbWriter = new StreamWriter(dbStream, Encoding.UTF8);
                var xmlSerializer = new XmlSerializer(typeof(HashSet<Task>));
                xmlSerializer.Serialize(dbWriter, tasks);
            }
        }
    }
}

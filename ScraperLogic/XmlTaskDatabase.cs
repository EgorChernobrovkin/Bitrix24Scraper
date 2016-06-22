using BellaCode.Collections.ObjectModel;

namespace ScraperLogic.Repository.Utility
{
    
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Xml.Serialization;

    using Models;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class XmlTaskDatabase
    {
        private const string SaveFileName = "tasksdb.xml";

        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<XmlTaskDatabase> _instance= new Lazy<XmlTaskDatabase>(() => new XmlTaskDatabase(), LazyThreadSafetyMode.ExecutionAndPublication);

        public static XmlTaskDatabase Instance => _instance.Value;

        public ObservableHashSet<Task> Tasks { get; }

        private XmlTaskDatabase()
        {
            if (!File.Exists(SaveFileName))
            {
                Tasks = new ObservableHashSet<Task>();
            }
            else
            {
                using (var dbStream = new FileStream(SaveFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var dbReader = new StreamReader(dbStream, Encoding.UTF8);
                    var xmlSerializer = new XmlSerializer(typeof (ObservableHashSet<Task>));
                    try
                    {
                        Tasks = (ObservableHashSet<Task>) xmlSerializer.Deserialize(dbReader);
                    }
                    catch
                    {
                        Tasks = new ObservableHashSet<Task>();
                    }
                }
            }
        }

        public void Save()
        {
            using (var dbStream = new FileStream(SaveFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var tasks = new ObservableHashSet<Task>(Tasks);
                var dbWriter = new StreamWriter(dbStream, Encoding.UTF8);
                var xmlSerializer = new XmlSerializer(typeof(ObservableHashSet<Task>));
                xmlSerializer.Serialize(dbWriter, tasks);
            }
        }
    }
}

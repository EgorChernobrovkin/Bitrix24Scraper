using System.ComponentModel;
using System.Runtime.CompilerServices;
using BellaCode.Collections.ObjectModel;
using ScraperLogic.Annotations;
using ScraperLogic.Models;
using ScraperLogic.Repository;
using ScraperLogic.Repository.Utility;

namespace ScraperLogic
{
    public class ScraperController : INotifyPropertyChanged
    {
        public ObservableHashSet<Task> Tasks => XmlTaskDatabase.Instance.Tasks;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

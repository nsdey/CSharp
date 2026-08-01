using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace QA_Master.Utitities
{
    public class ObservableCollectionEachItem<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (e.NewItems != null)
            {
                foreach (T each in e.NewItems)
                {
                    each.PropertyChanged -= each_PropertyChanged;
                    each.PropertyChanged += each_PropertyChanged;
                }
            }
        }

        private void each_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            base.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Go8_SQL_Deployment
{
    public class ObservableCollectionEachItem<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public ObservableCollectionEachItem() : base() { }
        public ObservableCollectionEachItem(IEnumerable<T> items) : base(items) { SetUp_PropertyChanged(items); }
        public ObservableCollectionEachItem(List<T> list) : base(list) { SetUp_PropertyChanged(list); }

        private void SetUp_PropertyChanged(IEnumerable<T>? items)
        {
            if (items != null)
            {
                foreach(T each in items)
                {
                    each.PropertyChanged -= each_PropertyChanged;
                    each.PropertyChanged += each_PropertyChanged;
                }
            }
        }

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

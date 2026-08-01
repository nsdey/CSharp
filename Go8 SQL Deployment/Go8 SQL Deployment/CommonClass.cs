using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Go8_SQL_Deployment
{
    public class CommonClass : INotifyPropertyChanged
    {
        public static List<FilterDate> DateOnlys = [new FilterDate("[No filter]", DateOnly.MaxValue)];
        public static List<FilterDate> DateOnlysSorted => DateOnlys.OrderByDescending(X => X.Value).ToList();

        public static FilterDate Filter = new("", DateOnly.MaxValue);

        public string? Name { get; set; }
        public string? FullPath { get; set; }

        public DateOnly LastWriteDate { get; set; }

        private int? rowsAffected;
        public int? RowsAffected { get { return rowsAffected; } set { rowsAffected = value; OnPropertyChanged(); } }

        private RunStatus? runStatus = Go8_SQL_Deployment.RunStatus.None;
        public RunStatus? RunStatus { get { return runStatus; } set { runStatus = value; OnPropertyChanged(); } }

        public string? GetRowsAffected => RowsAffected == null || rowsAffected < 0 ? null : $"{rowsAffected} rows affected";

        public IList<CommonClass> Items { get; set; } = [];

        internal ObservableCollection<CommonClass> children = [];
        public virtual ObservableCollection<CommonClass> Children { get; set; } = [];

        public virtual string? ImagePath { get; set; }

        public DirectoryClass? ParentDirectory { get; set; }

        public bool Selected { get; set; }

        public override string? ToString()
        {
            if (string.IsNullOrWhiteSpace(Name) == false)
                return Name;
            else
                return base.ToString();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

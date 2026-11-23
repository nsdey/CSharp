using System.IO;

namespace Go8_SQL_Deployment
{
    public class FileClass : CommonClass
    {
        public string? DirectoryName { get; set; }
        public string? FileContent { get; set; } = null;

        public override string? ImagePath => "pack:SQLFile.png";

        public FileClass() { }

        public FileClass(string? fullPath, DirectoryClass? parent = null)
        {
            FullPath = fullPath;
            Name = Path.GetFileName(FullPath);
            DirectoryName = parent?.Name;

            if (FullPath != null)
            {
                FileInfo fileInfo = new(FullPath);
                LastWriteDate = DateOnly.FromDateTime(fileInfo.LastWriteTime);
                if (DateOnlys.Where(X => X != null).Select(X => X.Value).Contains(LastWriteDate) == false) DateOnlys.Add(new FilterDate(LastWriteDate.ToString("dd-MMM-yyyy"), LastWriteDate));

                //PropertyChanged += delegate (object? sender, System.ComponentModel.PropertyChangedEventArgs e)
                //{
                //    parent?.OnPropertyChanged();
                //};
            }

            ParentDirectory = parent;
        }

        public bool Read(bool forceRead = false)
        {
            if (forceRead == true) FileContent = null;

            if (FullPath != null && FileContent == null)
            {
                FileContent = File.ReadAllText(FullPath);
                return true;
            }

            return false;
        }
    }
}

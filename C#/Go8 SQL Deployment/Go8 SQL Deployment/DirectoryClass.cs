using System.IO;

namespace Go8_SQL_Deployment
{
    public class DirectoryClass : CommonClass
    {
        public string FileSearchPattern { get; set; } = "*.*";
        public override string? ImagePath => "pack:Folder.png";

        public override ObservableCollectionEachItem<CommonClass> Children
            => new(
                Items.Where
                (
                    X => (X is DirectoryClass && X.Children.Count > 0)
                    ||
                    (Filter?.Value == DateOnly.MaxValue)
                    ||
                    (Filter?.Value != null && X.LastWriteDate == Filter.Value)
                )
            );

        public DirectoryClass() { }

        public DirectoryClass(string? fullPath, string? fileSearchPattern, DirectoryClass? parent = null)
        {
            FullPath = fullPath;
            Name = FullPath?.Split("\\").Last();

            ParentDirectory = parent;

            if (fileSearchPattern != null) FileSearchPattern = fileSearchPattern;

            if (FullPath != null)
            {
                foreach (string eachDir in Directory.GetDirectories(FullPath))
                {
                    AddDirectory(eachDir);
                }
                foreach (string eachFile in Directory.GetFiles(FullPath, FileSearchPattern))
                {
                    AddFile(eachFile);
                }
            }
        }

        public DirectoryClass AddDirectory(string? fullPath)
        {
            DirectoryClass directoryClass = new(fullPath, FileSearchPattern, this);

            if (directoryClass.Items.Count > 0) Items.Add(directoryClass);

            return directoryClass;
        }

        public FileClass AddFile(string? fullPath)
        {
            FileClass fileClass = new(fullPath, this);
            Items.Add(fileClass);

            return fileClass;
        }
    }
}

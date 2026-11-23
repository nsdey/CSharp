using QA_Master.Models.Core;
using System.Reflection;
using System.IO;
using System.ComponentModel;

namespace QA_Master.Forms
{
    public class Session : _PropertyChanged
    {
        private static Session? instance = null;
        public static Session Instance { get { instance??= new Session(); return instance; } }

        public MainWindow? MainWindow;

        private const string baseTitle = "QA Master";
        private string windowTitle = baseTitle;
        public string WindowTitle
        {
            get { return windowTitle; }
            private set { windowTitle = value; OnPropertyChanged(); }
        }

        public string? ExeDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public const string projectFileFilter = "JSON project files|*.json";

        public string? ProjectFile { get; set; } = null;
        public string? ProjectFileDirectory => Path.GetDirectoryName(ProjectFile);
        public string? ProjectFileName => Path.GetFileName(ProjectFile);
        public string? ProjectSavedJsonString { get; private set; } = null;
        public string? ProjectCurrentJsonString { get; private set; } = null;

        public string? projectFormTitle { get; set; } = null;

        private Project? project = null;
        public Project? Project
        {
            get { if (project == null) Project = new Project(); return project; }
            private set
            {
                if (value == null) project = new Project();
                else project = value;

                project.PropertyChanged -= project_PropertyChanged;
                project.PropertyChanged += project_PropertyChanged;

                OnPropertyChanged();
            }
        }

        private void project_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SetupWindowTitle();
        }

        public void OpenProject(string? fileName = null)
        {
            ProjectFile = fileName;

            if (ProjectFile != null) ProjectSavedJsonString = File.ReadAllText(ProjectFile);
            else ProjectSavedJsonString = null;

            Project = Project.ReadFromJson(ProjectSavedJsonString);

            ProjectCurrentJsonString = Project?.WriteToJson();
            ProjectSavedJsonString = ProjectCurrentJsonString;

            SetupWindowTitle();

            if (Project != null) Project.PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
            {
                SetupWindowTitle();
            };
        }

        public void SetupWindowTitle()
        {
            string? projectName = "[" + Project?.Name + "]";
            string? fileName = "[" + (ProjectFileName ?? "new") + $"{(Project != null && Project.ProjectChanged ? "*" : "")}]";

            WindowTitle = $"{baseTitle} {projectName} {fileName}";
        }
    }
}

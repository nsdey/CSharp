using Microsoft.Win32;
using QA_Master.Models.Extensions;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace QA_Master.Forms.Pages
{
    /// <summary>
    /// Interaction logic for Page_PerformUploadFile.xaml
    /// </summary>
    public partial class Page_PerformUploadFile : Page
    {
        public void _BaseConstructor()
        {
            InitializeComponent();
        }

        public Page_PerformUploadFile() => _BaseConstructor();

        public Page_PerformUploadFile(object dataContext)
        {
            _BaseConstructor();

            DataContext = dataContext;
        }

        private void btnBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            string? fileName = null;
            string? filePath = null;

            if (string.IsNullOrWhiteSpace(txtValue.Text) == false)
            {
                fileName = txtValue.Text.GetAbsolutePath(Session.Instance.ProjectFileDirectory);
                filePath = Path.GetDirectoryName(fileName);
                fileName = Path.GetFileName(fileName);
            }
            else
            {
                filePath = Session.Instance.ProjectFileDirectory;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "All files|*.*"};

            if (fileName != null) openFileDialog.FileName = fileName;
            if (filePath != null) openFileDialog.InitialDirectory = filePath;

            if (openFileDialog.ShowDialog(Window.GetWindow(this)) == true)
            {
                txtValue.Text = openFileDialog.FileName.GetRelativePath(Session.Instance.ProjectFileDirectory);                
            }
        }
    }
}

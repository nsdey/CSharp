using System.ComponentModel;
using QA_Master.Forms;
using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Upload File")]
    public class PerformUploadFile : Perform
    {
        private string? filePath = null;
        [Description("File")]
        public string? FilePath
        {
            get { return filePath; }
            set { filePath = value; OnPropertyChanged(); }
        }

        public string? FilePathAbsolute => FilePath?.GetAbsolutePath(Session.Instance?.ProjectFileDirectory);

        public PerformUploadFile() { }

        public PerformUploadFile(string text)
        {
            FilePath = text;
        }

        public override Type? AttachedEditor => typeof(Page_PerformUploadFile);

        public override string ToString()
        {
            string returnValue = this.GetDescription();

            if (FilePath != null)
            {
                returnValue = $"{returnValue} [{FilePath.GetShorter(IgnoreShorterDescription)}]";
            }

            return returnValue;
        }
    }
}

using System.ComponentModel;
using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Enter Text")]
    public class PerformEnterText : Perform
    {
        private string? text = null;
        public string? Text
        {
            get { return text; }
            set { text = value; OnPropertyChanged(); }
        }

        public override Type? AttachedEditor => typeof(Page_PerformEnterText);

        public override string ToString()
        {
            string returnValue = this.GetDescription();

            if (string.IsNullOrWhiteSpace(Text) == false)
            {
                returnValue = $"{returnValue} [{Text}]";
            }

            return returnValue;
        }
    }
}

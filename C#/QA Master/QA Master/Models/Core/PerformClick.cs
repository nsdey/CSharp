using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;
using System.ComponentModel;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Click")]
    public class PerformClick : Perform
    {
        private bool checkElementExists;
        [Description("If element exists, then do above perform")]
        public bool CheckElementExists
        {
            get { return checkElementExists; }
            set { if (checkElementExists.Equals(value)) return; checkElementExists = value; OnPropertyChanged(); }
        }


        private string? seconds = null;
        [Description("Then wait in Seconds")]
        public string? Seconds
        {
            get { return seconds; }
            set { if (seconds != null && seconds.Equals(value)) return; seconds = value; OnPropertyChanged(); }
        }

        public override Type? AttachedEditor => typeof(Page_PerformClick);

        public override string ToString()
        {
            string returnValue = this.GetDescription();

            if (CheckElementExists == true)
                returnValue = $"if exists then {returnValue}";

            if (string.IsNullOrWhiteSpace(Seconds) == false)
                returnValue = $"{returnValue} then Wait {Seconds} second(s)";

            return returnValue;
        }
    }
}

using System.ComponentModel;
using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Match UI Text")]
    public class PerformMatchUIText : Perform
    {
        private string? uIText = null;
        [Description("UI Text")]
        public string? UIText
        {
            get { return uIText; }
            set { uIText = value; OnPropertyChanged(); }
        }

        //public PerformMatchUIText() { }
        //
        //public PerformMatchUIText(string uiText)
        //{
        //    UIText = uiText;
        //}

        public override Type? AttachedEditor => typeof(Page_PerformCommon);

        public override string ToString()
        {
            string returnValue = this.GetDescription();

            if (string.IsNullOrWhiteSpace(UIText) == false)
            {
                returnValue = $"{returnValue} [{UIText}]";
            }

            return returnValue;
        }
    }
}

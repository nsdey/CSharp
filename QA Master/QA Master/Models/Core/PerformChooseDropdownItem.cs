using System.ComponentModel;
using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Choose Dropdown Item")]
    public class PerformChooseDropdownItem : Perform
    {
        private string? byText = null;
        [Description("By Text")]
        public string? ByText
        {
            get { return byText; }
            set { byText = value; OnPropertyChanged(); }
        }

        public PerformChooseDropdownItem() { }

        public PerformChooseDropdownItem(string uiText)
        {
            ByText = uiText;
        }

        public override Type? AttachedEditor => typeof(Page_PerformCommon);

        public override string ToString()
        {
            string returnValue = this.GetDescription();

            if (ByText != null)
            {
                if (string.IsNullOrWhiteSpace(ByText) == true)
                {
                    returnValue = $"Choose any Dropdown Item";
                }
                else
                {
                    returnValue = $"{returnValue} by text [{ByText}]";
                }
            }           

            return returnValue;
        }
    }
}

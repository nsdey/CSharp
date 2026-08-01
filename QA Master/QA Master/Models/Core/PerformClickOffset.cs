using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;
using System.ComponentModel;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Offset Click")]
    public class PerformClickOffset : Perform
    {
        private string? offset = null;
        [Description("Offset (x,y)")]
        public string? Offset
        {
            get { return offset; }
            set { offset = value; OnPropertyChanged(); }
        }

        public override Type? AttachedEditor => typeof(Page_PerformCommon);

        public override string ToString()
        {
            string returnValue = this.GetDescription();

            if (string.IsNullOrWhiteSpace(Offset) == false)
            {
                returnValue = $"Offset by {Offset} then Click";
            }

            return returnValue;
        }
    }
}

using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;
using System.ComponentModel;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Wait")]
    public class ActionWait : Action
    {
        private string? seconds;
        public string? Seconds
        {
            get { return seconds; }
            set { seconds = value; OnPropertyChanged(); }
        }

        public override Type? AttachedEditor => typeof(Page_ActionCommon);

        public override string ToString()
        {
            string? returnValue = $"{this.GetDescription()}";

            if (string.IsNullOrWhiteSpace(Seconds) == false)
            {
                returnValue = $"{returnValue} [{Seconds} seconds]";
            }
            else
            {
                returnValue = $"{returnValue}...";
            }


            return returnValue ?? base.ToString();
        }
    }
}

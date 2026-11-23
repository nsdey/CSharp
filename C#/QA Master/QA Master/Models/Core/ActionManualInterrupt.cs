using QA_Master.Forms.Pages;
using QA_Master.Models.Enums;
using QA_Master.Models.Extensions;
using System.ComponentModel;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Manual Interrupt...")]
    public class ActionManualInterrupt : Action
    {
        private string? message;

        [Description("Message")]
        public string? Message
        {
            get { return message; }
            set { message = value; OnPropertyChanged(); }
        }

        public override Type? AttachedEditor => typeof(Page_ActionCommon);

        public override string ToString()
        {
            string? returnValue = $"{this.GetDescription()}";

            if (string.IsNullOrWhiteSpace(Message) == false)
            {
                returnValue = $"{returnValue} User Message [{Message}]";
            }

            return returnValue ?? base.ToString();
        }
    }
}

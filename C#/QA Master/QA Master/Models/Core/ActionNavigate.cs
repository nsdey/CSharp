using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;
using System.ComponentModel;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Navigate to URL")]
    public class ActionNavigate : Action
    {
        private string? _URL = null;
        public string? URL
        {
            get { return string.IsNullOrWhiteSpace(_URL) ? _BaseURL : _URL; }
            set { _URL = value; OnPropertyChanged(); }
        }

        public override Type? AttachedEditor => typeof(Page_ActionCommon);

        public override string ToString()
        {
            string? returnValue = $"{this.GetDescription()}";

            if (string.IsNullOrWhiteSpace(URL) == false)
            {
                returnValue = $"{returnValue} [{URL}]";
            }
            else
            {
                returnValue = $"{returnValue} [Environment URL]";
            }

            return returnValue ?? base.ToString();
        }
    }
}

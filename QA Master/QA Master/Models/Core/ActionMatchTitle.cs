using System.ComponentModel;
using System.Xml.Serialization;
using System.Text.Json.Serialization;
using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Match Browser Title")]
    public class ActionMatchTitle : Action
    {
        [XmlIgnore, JsonIgnore]
        public string? Title
        {
            get { return ((PerformMatchTitle?)Perform)?.Title; }
            set { if (Perform != null) ((PerformMatchTitle)Perform).Title = value; }
        }

        public ActionMatchTitle()
        {
            Perform = new PerformMatchTitle();
        }

        public override Type? AttachedEditor => typeof(Page_ActionCommon);

        public override string ToString()
        {
            string? returnValue;

            if (Title != null)
            {
                returnValue = Perform?.ToString();
            }
            else
            {
                returnValue = this.GetDescription();
            }

            return returnValue?? base.ToString();
        }
    }
}

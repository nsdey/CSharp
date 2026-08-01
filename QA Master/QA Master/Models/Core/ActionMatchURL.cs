using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Match Browser URL")]
    public class ActionMatchURL : Action
    {
        [XmlIgnore, JsonIgnore]
        public string? URL
        {
            get
            {
                return ((PerformMatchURL?)Perform)?.URL;
            }
            set
            {
                string? url = value;

                if (url != null && url.StartsWith("http", StringComparison.OrdinalIgnoreCase) == false)
                {
                    if (url.StartsWith("/") == false)
                    {
                        url = $"http://{url.Trim('/')}";
                    }
                    else
                    {
                        url = $"/{url.Trim('/')}";
                    }
                }

                if (Perform != null) ((PerformMatchURL)Perform).URL = url;
            }
        }

        public ActionMatchURL()
        {
            Perform = new PerformMatchURL();
        }

        public override Type? AttachedEditor => typeof(Page_ActionCommon);

        public override string ToString()
        {
            string? returnValue;
        
            if (URL != null)
            {
                returnValue = Perform?.ToString();
            }
            else
            {
                returnValue = this.GetDescription();
            }

            return returnValue ?? base.ToString();
        }
    }
}

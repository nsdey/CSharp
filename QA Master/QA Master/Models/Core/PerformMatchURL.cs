using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;
using System.ComponentModel;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Match Browser URL")]
    public class PerformMatchURL : Perform
    {
        private string? uRL = null;
        public string? URL
        {
            get { return uRL; }
            set { uRL = value; OnPropertyChanged(); }
        }

        public string? FullyQualifiedURL
        {
            get
            {
                if (string.IsNullOrWhiteSpace(URL) == true) return _BaseURL;

                if (URL.StartsWith("/") && string.IsNullOrWhiteSpace(_BaseURL) == false)
                {
                    Uri uri = new (_BaseURL);
                    return $"{uri.Scheme}://{uri.Host}/{URL.TrimStart('/')}";
                }

                return URL;
            }
        }

        //public PerformMatchURL() { }
        //
        //public PerformMatchURL(string url)
        //{
        //    URL = url;
        //}
        
        public override Type? AttachedEditor => typeof(Page_PerformCommon);

        public override string ToString()
        {
            string returnValue = this.GetDescription();

            if (string.IsNullOrWhiteSpace(URL) == false)
            {
                returnValue = $"{returnValue} [{URL.GetShorter(IgnoreShorterDescription)}]";
            }

            return returnValue;
        }
    }
}

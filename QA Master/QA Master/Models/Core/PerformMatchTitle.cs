using System.ComponentModel;
using QA_Master.Forms.Pages;
using QA_Master.Models.Extensions;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Match Browser Title")]
    public class PerformMatchTitle : Perform
    {
        private string? title = null;
        public string? Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        //public PerformMatchTitle() { }
        //
        //public PerformMatchTitle(string title)
        //{
        //    Title = title;
        //}

        public override Type? AttachedEditor => typeof(Page_PerformCommon);

        public override string ToString()
        {
            string returnValue = this.GetDescription();

            if (string.IsNullOrWhiteSpace(Title) == false)
            {
                returnValue = $"{returnValue} [{Title.GetShorter(IgnoreShorterDescription)}]";
            }

            return returnValue;
        }
    }
}

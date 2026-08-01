using QA_Master.Models.Extensions;
using System.Xml.Serialization;

namespace QA_Master.Models.Core
{
    [XmlInclude(typeof(ActionAdCloser))]
    [XmlInclude(typeof(ActionManualInterrupt))]
    [XmlInclude(typeof(ActionFindHtmlElement))]
    [XmlInclude(typeof(ActionMatchTitle))]
    [XmlInclude(typeof(ActionMatchURL))]
    [XmlInclude(typeof(ActionNavigate))]
    [XmlInclude(typeof(ActionWait))]   
    [Serializable]
    public class Action : Common
    {
        [XmlIgnore]
        public Workflow? Workflow { get; set; } = null;

        private Perform? perform = null;
        public Perform? Perform
        {
            get { return perform; }
            set
            {
                perform = value;
                
                if (perform!= null) perform.PropertyChanged += delegate (object? sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    OnPropertyChanged();
                };
                
                OnPropertyChanged();
            }
        }

        public Action() { }
        public Action(string defaultValue) { DefaultValue = defaultValue; }

        private byte[]? _Screenshot = null;
        [XmlIgnore]
        public byte[]? Screenshot { get { return Perform == null ? _Screenshot : Perform.Screenshot; } set { _Screenshot = value; } }

        public string GetFullDescription()
        {
            IgnoreShorterDescription = true;
            string returnValue = this.ToString();
            IgnoreShorterDescription = false;
            return returnValue;
        }
        public override dynamic? CopyTo(dynamic? destination = null)
        {
            Action? dest = destination;

            //if (dest == null || dest.GetType() != GetType())
            dest ??= (Action?)Activator.CreateInstance(GetType());

            if (dest == null) return null;

            this.CopyFirstLevel(dest);

            if (Perform != null)
            {
                dest.Perform = (Perform?)Activator.CreateInstance(Perform.GetType());
                Perform.CopyFirstLevel(dest.Perform);
            }
            else
            {
                dest.Perform = null;
            }

            return dest;
        }

        public override string ToString()
        {
            string? returnValue;

            if (string.IsNullOrWhiteSpace(DefaultValue) == false)
            {
                returnValue = DefaultValue;
            }
            else
            {
                returnValue = this.GetDescription();
            }

            return returnValue ?? base.ToString();
        }
    }
}

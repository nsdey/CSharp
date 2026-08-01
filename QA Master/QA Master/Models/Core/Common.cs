using QA_Master.Models.Enums;
using QA_Master.Models.Extensions;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace QA_Master.Models.Core
{
    [Serializable]
    public abstract class Common : _PropertyChanged
    {
        [XmlIgnore]
        internal static string? _BaseURL { get; set; } = null;

        [XmlIgnore]
        public virtual Type? AttachedEditor { get; private set; } = null;

        public Page? GetAttachedPage (object? dataContext)
        {
            if (AttachedEditor == null) return null;
        
            Page? page = (Page?)Activator.CreateInstance(AttachedEditor, [dataContext]);

            return page;
        }

        [XmlIgnore]
        public string? DefaultValue { get; set; }

        private EnumRunStatus runStatus = EnumRunStatus.None;
        [XmlIgnore]
        public EnumRunStatus RunStatus { get { return runStatus; } set { if (runStatus == value) return; runStatus = value; OnPropertyChanged(); } }

        [XmlIgnore]
        public string? GetRunStatus => RunStatus.GetDescription();

        [XmlIgnore]
        public static bool IgnoreShorterDescription { get; set; }

        [XmlIgnore]
        public virtual Common? EditModel { get; set; } = null;

        public override string ToString() => this.GetDescription();

        public object Clone()
        {
            return MemberwiseClone();
        }

        public virtual dynamic? CopyTo(dynamic? destination = null)
        {
            throw new NotImplementedException();
        }
    }
}

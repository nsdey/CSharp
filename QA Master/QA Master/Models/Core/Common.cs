using QA_Master.Models.Enums;
using QA_Master.Models.Extensions;
using System.Windows.Controls;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace QA_Master.Models.Core
{
    [Serializable]
    public abstract class Common : _PropertyChanged
    {
        [XmlIgnore, JsonIgnore]
        internal static string? _BaseURL { get; set; } = null;

        [XmlIgnore, JsonIgnore]
        public virtual Type? AttachedEditor { get; private set; } = null;

        public Page? GetAttachedPage (object? dataContext)
        {
            if (AttachedEditor == null) return null;
        
            Page? page = (Page?)Activator.CreateInstance(AttachedEditor, [dataContext]);

            return page;
        }

        [XmlIgnore, JsonIgnore]
        public string? DefaultValue { get; set; }

        private EnumRunStatus runStatus = EnumRunStatus.None;
        [XmlIgnore, JsonIgnore]
        public EnumRunStatus RunStatus { get { return runStatus; } set { if (runStatus == value) return; runStatus = value; OnPropertyChanged(); } }

        [XmlIgnore, JsonIgnore]
        public string? GetRunStatus => RunStatus.GetDescription();

        [XmlIgnore, JsonIgnore]
        public static bool IgnoreShorterDescription { get; set; }

        [XmlIgnore, JsonIgnore]
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

using QA_Master.Models.Extensions;
using System.Xml.Serialization;

namespace QA_Master.Models.Core
{
    [XmlInclude(typeof(PerformClick))]
    [XmlInclude(typeof(PerformClickOffset))]
    [XmlInclude(typeof(PerformElementDoesnotExists))]
    [XmlInclude(typeof(PerformCheckBoxOff))]
    [XmlInclude(typeof(PerformCheckBoxOn))]
    [XmlInclude(typeof(PerformCheckBoxToggle))]
    [XmlInclude(typeof(PerformChooseDropdownItem))]
    [XmlInclude(typeof(PerformEnterText))]
    [XmlInclude(typeof(PerformMatchTitle))]
    [XmlInclude(typeof(PerformMatchUIText))]
    [XmlInclude(typeof(PerformMatchURL))]
    [XmlInclude(typeof(PerformUploadFile))]
    [Serializable]
    public abstract class Perform : Common
    {
        [XmlIgnore]
        public Action? Action { get; set; }

        [XmlIgnore]
        public byte[]? Screenshot { get; set; }

        public override dynamic? CopyTo(dynamic? destination = null)
        {
            Perform? dest;

            if (destination != null)
            {
                dest = destination;
            }
            else
            {
                dest = (Perform?)Activator.CreateInstance(GetType());
            }

            this.CopyFirstLevel(dest);

            return dest;
        }
    }
}

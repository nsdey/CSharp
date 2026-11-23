using QA_Master.Models.Extensions;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

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
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(PerformClick), "click")]
    [JsonDerivedType(typeof(PerformClickOffset), "clickOffset")]
    [JsonDerivedType(typeof(PerformElementDoesnotExists), "elementDoesNotExist")]
    [JsonDerivedType(typeof(PerformCheckBoxOff), "checkBoxOff")]
    [JsonDerivedType(typeof(PerformCheckBoxOn), "checkBoxOn")]
    [JsonDerivedType(typeof(PerformCheckBoxToggle), "checkBoxToggle")]
    [JsonDerivedType(typeof(PerformChooseDropdownItem), "chooseDropdownItem")]
    [JsonDerivedType(typeof(PerformEnterText), "enterText")]
    [JsonDerivedType(typeof(PerformMatchTitle), "matchTitle")]
    [JsonDerivedType(typeof(PerformMatchUIText), "matchUiText")]
    [JsonDerivedType(typeof(PerformMatchURL), "matchUrl")]
    [JsonDerivedType(typeof(PerformUploadFile), "uploadFile")]
    public abstract class Perform : Common
    {
        [XmlIgnore, JsonIgnore]
        public Action? Action { get; set; }

        [XmlIgnore, JsonIgnore]
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

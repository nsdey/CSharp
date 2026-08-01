using QA_Master.Forms.Pages;
using QA_Master.Models.Enums;
using QA_Master.Models.Extensions;
using System.ComponentModel;
using System.Xml.Serialization;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Find element")]
    public class ActionFindHtmlElement : Action
    {
        [XmlIgnore]
        public string[] GetAllFindHtmlElementBy = Extension.GetAllEnumNames(typeof(EnumActionFindHtmlElementBy));

        private EnumActionFindHtmlElementBy enumActionFindHtmlElementBy = EnumActionFindHtmlElementBy.None;
        public EnumActionFindHtmlElementBy EnumActionFindHtmlElementBy
        {
            get { return enumActionFindHtmlElementBy; }
            set { enumActionFindHtmlElementBy = value; OnPropertyChanged(); }
        }

        private string findHtmlElementValue = string.Empty;
        public string FindHtmlElementValue
        {
            get { return findHtmlElementValue; }
            set { findHtmlElementValue = value; OnPropertyChanged(); }
        }

        [XmlIgnore]
        public override Type? AttachedEditor => typeof(Page_ActionFindHtmlElement);

        [XmlIgnore]
        private List<Perform> getPerformClasses =
        [
            new PerformClick(),
            new PerformClickOffset(),
            new PerformElementDoesnotExists(),
            new PerformEnterText(),
            new PerformMatchUIText(),
            new PerformCheckBoxOn(),
            new PerformCheckBoxOff(),
            new PerformCheckBoxToggle(),
            new PerformChooseDropdownItem(),
            new PerformUploadFile(),
        ];

        [XmlIgnore]
        public List<Perform>? GetPerformClasses
        {
            get
            {
                getPerformClasses.ForEach(X => { X.EditModel ??= X.CopyTo(); });

                if (Perform != null)
                {
                    Perform? getSelected = getPerformClasses?.FirstOrDefault(X => X.GetType() == Perform.GetType());
                    Perform.CopyFirstLevel(getSelected?.EditModel);
                }

                return getPerformClasses;
            }
        }

        public Perform? GetPerformClassesItem
        {
            get
            {
                if (Perform == null) return null;

                Perform? getPerform = GetPerformClasses?.FirstOrDefault(X => X.GetType() == Perform.GetType());
                return getPerform;
            }
            set { }
        }

        public override string ToString()
        {
            string? returnValue = $"{this.GetDescription()}";

            if (EnumActionFindHtmlElementBy != EnumActionFindHtmlElementBy.None)
            {
                returnValue = $"{returnValue} by {EnumActionFindHtmlElementBy.GetDescription()} [{FindHtmlElementValue.GetShorter(IgnoreShorterDescription)}]";
            }

            if (Perform != null)
            {
                returnValue = $"{returnValue} then {Perform}";
            }

            return returnValue ?? base.ToString();
        }
    }
}

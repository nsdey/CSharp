using System.ComponentModel;

namespace QA_Master.Models.Enums
{
    public enum EnumActionFindHtmlElementBy
    {
        None,
        Id,
        [Description("Partial Id")]
        PartialId,
        XPath,
        [Description("Class Name")]
        ClassName,
        Name,
        [Description("Tag Name")]
        TagName,
        [Description("Link Text")]
        LinkText,
        [Description("Partial Link Text")]
        PartialLinkText,
        [Description("UI Text")]
        UIText,
        [Description("Partial UI Text")]
        PartialUIText,
        [Description("CSS Selector")]
        CSSSelector,
    }
}

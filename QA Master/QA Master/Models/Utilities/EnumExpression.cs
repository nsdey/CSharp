
using System.ComponentModel;

namespace QA_Master.Models.Utilities
{
    public enum EnumExpression
    {
        Guid,
        Date,
        Time,
        [Description("Date time")]
        DateTime,
        [Description("Range(Minimum Value, Maximum Value)")]
        Range,
    }
}

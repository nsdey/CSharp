using System.ComponentModel;

namespace QA_Master.Models.Enums
{
    public enum EnumRunStatus
    {
        None,
        Running,
        Passed,
        [Description("Partially Passed")]
        PartiallyPassed,
        Failed,
    }
}

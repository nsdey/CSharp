
using System.ComponentModel;

namespace QA_Master.Models.Core
{
    [Description("Auto Ad Closer")]
    public class ActionAdCloser : Action
    {
        public override string ToString()
        {
            return base.ToString() + " [Google]";
        }
    }
}

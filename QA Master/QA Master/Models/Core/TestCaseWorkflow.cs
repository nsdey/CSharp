using QA_Master.Models.Enums;
using QA_Master.Utitities;
using System.Xml.Serialization;

namespace QA_Master.Models.Core
{
    public class TestCaseWorkflow : _PropertyChanged
    {
        [XmlIgnore]
        public string? VariableKey { get { return Guid.NewGuid().ToString(); } set { } }

        private Workflow? workflow = null;
        [XmlIgnore]
        public Workflow? Workflow
        {
            get { return workflow; }
            set
            {
                workflow = value;

                if (workflow != null) workflow.PropertyChanged += delegate (object? sender, System.ComponentModel.PropertyChangedEventArgs e)
                {
                    OnPropertyChanged();
                };

                OnPropertyChanged();
            }
        }

        [XmlIgnore]
        public ObservableCollectionEachItem<Action>? Actions => Workflow?.Actions;

        [XmlIgnore]
        public string? Id => Workflow?.Id;

        [XmlIgnore]
        public EnumRunStatus? RunStatus => Workflow?.RunStatus;

        [XmlIgnore]
        public string? GetRunStatus => Workflow?.GetRunStatus;

        public TestCaseWorkflow() { }

        public TestCaseWorkflow(Workflow? workflow) { Workflow = workflow; }

        public dynamic? CopyTo(dynamic? destination = null)
        {
            TestCaseWorkflow? dest = destination;

            dest ??= new TestCaseWorkflow();

            dest.Workflow = Workflow?.CopyTo();

            return dest;
        }

        public override string? ToString()
        {
            if (Workflow != null)
                return Workflow?.ToString();
            else
                return base.ToString();
        }
    }
}

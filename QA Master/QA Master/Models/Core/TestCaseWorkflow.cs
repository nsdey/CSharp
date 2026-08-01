using QA_Master.Models.Enums;
using QA_Master.Utitities;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace QA_Master.Models.Core
{
    public class TestCaseWorkflow : _PropertyChanged
    {
        [XmlIgnore, JsonIgnore]
        public string? VariableKey { get { return Guid.NewGuid().ToString(); } set { } }

        private Workflow? workflow = null;
        [XmlIgnore, JsonIgnore]
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

        [XmlIgnore, JsonIgnore]
        public ObservableCollectionEachItem<Action>? Actions => Workflow?.Actions;

        [XmlIgnore, JsonIgnore]
        public string? Id => Workflow?.Id;

        [XmlIgnore, JsonIgnore]
        public EnumRunStatus? RunStatus => Workflow?.RunStatus;

        [XmlIgnore, JsonIgnore]
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

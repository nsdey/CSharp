using System.ComponentModel;
using System.Xml.Serialization;
using QA_Master.Forms;
using QA_Master.Models.Extensions;
using QA_Master.Utitities;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Test Case")]
    public class TestCase : Common
    {
        private string? name = null;
        public string? Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        public List<string?>? WorkflowId_FK { get; set; } = [];

        private ObservableCollectionEachItem<TestCaseWorkflow>? workflows = [];
        [XmlIgnore]
        public ObservableCollectionEachItem<TestCaseWorkflow>? Workflows
        {
            get { if (workflows == null) Workflows = []; return workflows; }
            set { workflows = value; OnPropertyChanged(); }
        }

        public TestCase()
        {
            if (workflows != null)
            {
                workflows.CollectionChanged += delegate (object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
                {
                    if (updatingWorkflow == false)
                    {
                        WorkflowId_FK = Workflows?.Select(X => X.Id)?.ToList();
                        OnPropertyChanged();
                    }
                };
            }
        }

        [XmlIgnore]
        public string? BaseURL { get { return _BaseURL; } set { _BaseURL = value; } }

        [XmlIgnore]
        public int WorkflowSelectedIndex { get; set; }

        [XmlIgnore]
        public int ActionSelectedIndex { get; set; }

        public override Type? AttachedEditor => typeof(CommonEditor);

        private bool updatingWorkflow = false;
        public void Update_Workflows(Project project)
        {
            updatingWorkflow = true;

            Workflows?.Clear();
            Workflow? workflow = null;
            if (WorkflowId_FK != null)
            {
                foreach (string? eachKey in WorkflowId_FK)
                {
                    workflow = project.Workflows?.FirstOrDefault(X => X.Id == eachKey);

                    if (workflow != null) Workflows?.Add(new TestCaseWorkflow(workflow));
                }
            }

            updatingWorkflow = false;
        }

        public override dynamic? CopyTo(dynamic? destination = null)
        {
            TestCase? testCase = destination;
            testCase ??= (TestCase?)Activator.CreateInstance(GetType());

            if (testCase == null) return null;

            this.CopyFirstLevel(testCase);

            testCase.Workflows = [];

            if (Workflows != null)
            {
                foreach(TestCaseWorkflow? eachWorkflow in Workflows)
                {
                    testCase.Workflows.Add(eachWorkflow.CopyTo());
                }
            }

            return testCase;
        }

        public override string ToString()
        {
            string returnValue = this.GetDescription();

            if (string.IsNullOrEmpty(Name) == false)
            {
                returnValue = $"{returnValue} - [{Name}]";
            }

            return returnValue;
        }
    }
}

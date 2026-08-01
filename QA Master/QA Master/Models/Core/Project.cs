using System.ComponentModel;
using QA_Master.Models.Extensions;
using System.IO;
using System.Xml.Serialization;
using QA_Master.Utitities;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Project")]
    public class Project : Common
    {
        public Project()
        {
            PropertyChanged += delegate (object? sender, PropertyChangedEventArgs e)
            {
                WriteToXML();
            };

            if (Workflows != null) Workflows.CollectionChanged += OnCollectionChanged;
            if (TestCases != null) TestCases.CollectionChanged += OnCollectionChanged;
        }

        private string? name = "New Project";
        public string? Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        private string? baseURLs;
        public string? BaseURLs
        {
            get { return baseURLs; }
            set { baseURLs = value; OnPropertyChanged(); }
        }

        private ObservableCollectionEachItem<Workflow>? workflows = [];
        public ObservableCollectionEachItem<Workflow>? Workflows
        {
            get { if (workflows == null) Workflows = []; return workflows; }
            set { workflows = value; OnPropertyChanged(); }
        }

        private ObservableCollectionEachItem<TestCase>? testCases = [];
        public ObservableCollectionEachItem<TestCase>? TestCases
        {
            get { if (testCases == null) TestCases = []; return testCases; }
            set { testCases = value; if (testCases != null) testCases.CollectionChanged += OnCollectionChanged; OnPropertyChanged(); }
        }

        [XmlIgnore]
        public string? XMLSavedString = null, XMLCurrentString = null;
        [XmlIgnore]
        public bool ProjectChanged => XMLSavedString != XMLCurrentString;

        public static Project? ReadFromXML(string? xmlString)
        {
            Project? project = null;

            if (xmlString == null)
            {
                project = new Project();
            }
            else
            {
                StringReader stringReader = new StringReader(xmlString);
                XmlSerializer serializer = new XmlSerializer(typeof(Project));
                project = (Project?)serializer.Deserialize(stringReader);
            }

            //if (project?.BaseURLs != null) project.BaseURLs = project.BaseURLs.Replace("\n", "\r\n");

            project?.TestCases?.ForEach(X => X.Update_Workflows(project));

            if (project != null) project.XMLSavedString = project.WriteToXML(); // xmlString;

            return project;
        }

        public string? WriteToXML()
        {
            //project.TestCases?.ForEach(X => X.Update_WorkflowId_FK());

            //if (BaseURLs != null) BaseURLs = BaseURLs.Replace("\r", "");

            StringWriter stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(Project));
            serializer.Serialize(stringwriter, this);

            XMLCurrentString = stringwriter.ToString();

            return XMLCurrentString;
        }

        //public Project Setup_AddWorkflow(Workflow workflow)
        //{
        //    Workflows.Add(workflow);
        //
        //    return this;
        //}

        public override string ToString()
        {
            string returnValue = this.GetDescription();

            if (string.IsNullOrWhiteSpace(Name) == false)
            {
                returnValue = $"{returnValue} [{Name}]";
            }

            return returnValue;
        }
    }
}

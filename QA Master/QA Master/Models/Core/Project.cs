using System.ComponentModel;
using QA_Master.Models.Extensions;
using System.IO;
using System.Xml.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
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
                WriteToJson();
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

        [XmlIgnore, JsonIgnore]
        public string? JsonSavedString = null, JsonCurrentString = null;
        [XmlIgnore, JsonIgnore]
        public bool ProjectChanged => JsonSavedString != JsonCurrentString;

        public static Project? ReadFromJson(string? jsonString)
        {
            Project? project = null;

            if (jsonString == null)
            {
                project = new Project();
            }
            else
            {
                project = JsonSerializer.Deserialize<Project>(jsonString, JsonOptions);
            }

            //if (project?.BaseURLs != null) project.BaseURLs = project.BaseURLs.Replace("\n", "\r\n");

            project?.TestCases?.ForEach(X => X.Update_Workflows(project));

            if (project != null) project.JsonSavedString = project.WriteToJson();

            return project;
        }

        public string? WriteToJson()
        {
            //project.TestCases?.ForEach(X => X.Update_WorkflowId_FK());

            //if (BaseURLs != null) BaseURLs = BaseURLs.Replace("\r", "");

            JsonCurrentString = JsonSerializer.Serialize(this, JsonOptions);

            return JsonCurrentString;
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() },
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    typeInfo =>
                    {
                        if (!typeof(Common).IsAssignableFrom(typeInfo.Type)) return;

                        JsonPropertyInfo? attachedEditor = typeInfo.Properties
                            .FirstOrDefault(property => property.Name == nameof(Common.AttachedEditor));

                        if (attachedEditor != null) typeInfo.Properties.Remove(attachedEditor);
                    }
                }
            }
        };

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

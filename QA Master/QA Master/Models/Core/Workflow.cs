using QA_Master.Forms;
using QA_Master.Models.Enums;
using QA_Master.Models.Extensions;
using QA_Master.Utitities;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace QA_Master.Models.Core
{
    [Serializable]
    [Description("Workflow")]
    public class Workflow : Common
    {
        public Workflow()
        {
            if (Actions != null) Actions.CollectionChanged += OnCollectionChanged;
        }

        [XmlIgnore, JsonIgnore]
        public string? VariableKey => Guid.NewGuid().ToString();

        private string? _Id = Guid.NewGuid().ToString();
        public string? Id
        {
            get { return _Id; }
            set { if (value == null) _Id = Guid.NewGuid().ToString(); else _Id = value; OnPropertyChanged(); }
        }

        private string? name = null;
        public string? Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(); }
        }

        private ObservableCollectionEachItem<Action> actions = [];
        public ObservableCollectionEachItem<Action> Actions
        {
            get { return actions; }
            set
            {
                actions = value;

                if (actions != null)
                {
                    actions.CollectionChanged += delegate (object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
                    {
                        OnPropertyChanged();

                        //try run status
                        //if (actions.Any(X => X.RunStatus == EnumRunStatus.Running || (X.Perform != null && X.Perform.RunStatus == EnumRunStatus.Running)))
                        //{
                        //    RunStatus = EnumRunStatus.Running;
                        //}
                        //else if (actions.Any(X => X.RunStatus == EnumRunStatus.Failed || (X.Perform != null && X.Perform.RunStatus == EnumRunStatus.Failed)))
                        //{
                        //    RunStatus = EnumRunStatus.Failed;
                        //}
                        //else if (actions.Count(X => X.RunStatus == EnumRunStatus.Passed && ((X.Perform == null) || (X.Perform != null && X.Perform.RunStatus == EnumRunStatus.Passed))) == actions.Count)
                        //{
                        //    RunStatus = EnumRunStatus.Passed;
                        //}
                        //else if (actions.Any(X => X.RunStatus != EnumRunStatus.None))
                        //{
                        //    RunStatus = EnumRunStatus.PartiallyPassed;
                        //}
                    };
                }

                OnPropertyChanged();
            }
        }

        public string? BaseURL { get { return _BaseURL; } set { _BaseURL = value; } }

        public override Type? AttachedEditor => typeof(CommonEditor);

        public Workflow(string defaultValue) { DefaultValue = defaultValue; }

        public Workflow Add_Action(Action action)
        {
            action.Workflow = this;
            Actions.Add(action);
            return this;
        }

        public override dynamic? CopyTo(dynamic? destination = null)
        {
            Workflow? dest = destination;
            dest ??= (Workflow?)Activator.CreateInstance(GetType());

            if (dest == null) return null;

            this.CopyFirstLevel(dest);

            dest.Actions = [];

            foreach (Action? eachAction in Actions)
            {
                dest.Actions.Add(eachAction.CopyTo());
            }

            return dest;
        }

        public void Setup_RunStatus(EnumRunStatus enumRunStatus = EnumRunStatus.None)
        {
            RunStatus = enumRunStatus;
            Actions.ForEach(X =>
            {
                X.RunStatus = enumRunStatus;
                if (X.Perform != null) X.Perform.RunStatus = enumRunStatus;
            });
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(DefaultValue) == false)
            {
                return DefaultValue;
            }
            else
            {
                string returnValue = $"{this.GetDescription()}";

                if (Name != null)
                {
                    returnValue = $"{returnValue} - [{Name}]";
                }

                return returnValue;
            }
        }
    }
}

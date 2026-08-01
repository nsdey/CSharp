using Microsoft.Win32;
using QA_Master.Models.Core;
using QA_Master.Models.Extensions;
using QA_Master.Services.Core;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace QA_Master.Forms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Session.Instance.MainWindow = this;

            InitializeComponent();

            DataContext = Session.Instance;
            OpenOrNewProject();
        }

        private void btnNewProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessSaveIfRequired() != null) OpenOrNewProject();
        }

        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessSaveIfRequired() == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = Session.projectFileFilter, InitialDirectory = Session.Instance.ExeDirectory };
            if (openFileDialog.ShowDialog(this) == true)
            {
                try
                {
                    OpenOrNewProject(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    throw new Exception("File open error", ex);
                }
            }
        }

        private void OpenOrNewProject(string? fileName = null)
        {
            Session.Instance.OpenProject(fileName);

            alcWorkflow.ListCollection = Session.Instance.Project?.Workflows;
            alcTestCase.ListCollection = Session.Instance.Project?.TestCases;
        }

        private void btnSaveProject_Click(object sender, RoutedEventArgs e)
        {
            SaveProject(Session.Instance.ProjectFile);
        }

        private void btnSaveProjectAs_Click(object sender, RoutedEventArgs e)
        {
            SaveProject();
        }

        private bool? ProcessSaveIfRequired()
        {
            if (Session.Instance.Project != null && Session.Instance.Project.ProjectChanged)
            {
                switch (MessageBox.Show
                    (
                        this,
                        $"The {Session.Instance.Project} has changed, do you want to save ?",
                        "Project changed",                        
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question
                    ))
                {
                    case MessageBoxResult.Yes:
                        return SaveProject(Session.Instance.ProjectFile);
                    case MessageBoxResult.No:
                        return false;
                    case MessageBoxResult.Cancel:
                        return null;
                }
            }

            return true;
        }

        private bool? SaveProject(string? fileName = null)
        {
            string? saveFile = fileName;

            if (saveFile == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog { Filter = Session.projectFileFilter, FileName = Session.Instance.Project?.Name };

                bool? saveFileDialogResult = saveFileDialog.ShowDialog(this);

                if (saveFileDialogResult == true)
                {
                    saveFile = saveFileDialog.FileName;
                }
                else
                {
                    return null;
                }
            }

            try
            {
                string? projectSave = Session.Instance.Project?.XMLCurrentString;
                
                if (saveFile != null) File.WriteAllText(saveFile, projectSave);

                Session.Instance.ProjectFile = saveFile;
                if (Session.Instance.Project != null) Session.Instance.Project.XMLSavedString = Session.Instance.Project?.XMLCurrentString;
                Session.Instance.SetupWindowTitle();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("File saving error", ex);
            }
        }

        private void alcWorkflow_ButtonClick(object sender, UserControls.AdvancedListControl.ButtonClickEventArgs e)
        {
            CommonEditor? commonEditor; 
            switch (e.ButtonType)
            {
                case UserControls.AdvancedListControl.EnumButtonType.Add:
                    commonEditor = new (new Workflow(), Enums.EnumFormType.Add) { Owner = this };

                    if (commonEditor.ShowDialog() == true) alcWorkflow.AddNewItem(commonEditor.DataContext);

                    break;

                case UserControls.AdvancedListControl.EnumButtonType.AddSimilar:
                    commonEditor = new(((Workflow?)e.SelectedItem)?.CopyTo(), Enums.EnumFormType.Add) { Owner = this };

                    if (commonEditor.ShowDialog() == true) alcWorkflow.AddNewItem(commonEditor.DataContext);

                    break;

                case UserControls.AdvancedListControl.EnumButtonType.Edit:
                    commonEditor = new(e.SelectedItem.CloneFirstLevel(), Enums.EnumFormType.Edit) { Owner = this };

                    if (commonEditor.ShowDialog() == true) ((Workflow)commonEditor.DataContext).CopyFirstLevel(e.SelectedItem);

                    break;
            }
        }

        private void alcAction_ButtonClick(object sender, UserControls.AdvancedListControl.ButtonClickEventArgs e)
        {
            ActionEditor? actionEditor = null;

            switch (e.ButtonType)
            {
                case UserControls.AdvancedListControl.EnumButtonType.Add:
                    actionEditor = new (null, Enums.EnumFormType.Add) { Owner = this };
                    
                    if (actionEditor.ShowDialog() == true) alcAction.AddNewItem(actionEditor.DataContext);
                    
                    break;

                case UserControls.AdvancedListControl.EnumButtonType.AddSimilar:
                    actionEditor = new(e.SelectedItem, Enums.EnumFormType.Add) { Owner = this };

                    if (actionEditor.ShowDialog() == true) alcAction.AddNewItem(actionEditor.DataContext);

                    break;

                case UserControls.AdvancedListControl.EnumButtonType.Edit:
                    actionEditor = new(e.SelectedItem, Enums.EnumFormType.Edit) { Owner = this };

                    if (actionEditor.ShowDialog() == true)
                    {
                        Models.Core.Action action = (Models.Core.Action)actionEditor.DataContext;

                        if (action.GetType() != e.SelectedItem?.GetType())
                        {
                            alcAction.SelectedItem = action;
                        }
                        else
                        {
                            action.CopyTo(e.SelectedItem);
                        }

                        //action = null;

                        //((Models.Core.Action)actionEditor.DataContext).CopyTo(e.SelectedItem);
                    }

                    break;

                case UserControls.AdvancedListControl.EnumButtonType.Run:
                    if (Session.Instance.Project?.Workflows?.Sum(X => X.Actions.Count) > 0)
                    {
                        TestCase testCase = new TestCase
                        {
                            //Workflows = Session.Instance.Project.Workflows,
                            WorkflowSelectedIndex = alcWorkflow.lstBox.SelectedIndex,
                            ActionSelectedIndex = alcAction.lstBox.SelectedIndex,
                        };

                        Session.Instance.Project.Workflows.ForEach(X => testCase?.Workflows?.Add(new TestCaseWorkflow(X)));

                        TestCaseRun testCaseRun = new (testCase, false)
                        { Owner = this };
                        
                        testCaseRun.ShowDialog();
                    }

                    break;
            }

            actionEditor?.Close();
        }

        private void alcTestCase_ButtonClick(object sender, UserControls.AdvancedListControl.ButtonClickEventArgs e)
        {
            CommonEditor? commonEditor;
            switch(e.ButtonType)
            {
                case UserControls.AdvancedListControl.EnumButtonType.Add:
                    commonEditor = new (new TestCase(), Enums.EnumFormType.Add) { Owner = this };

                    if (commonEditor.ShowDialog() == true) alcTestCase.AddNewItem(commonEditor.DataContext);

                    break;

                case UserControls.AdvancedListControl.EnumButtonType.AddSimilar:
                    commonEditor = new(((TestCase?)e.SelectedItem)?.CopyTo(), Enums.EnumFormType.Add) { Owner = this };

                    if (commonEditor.ShowDialog() == true) alcTestCase.AddNewItem(commonEditor.DataContext);

                    break;

                case UserControls.AdvancedListControl.EnumButtonType.Edit:
                    commonEditor = new(e.SelectedItem.CloneFirstLevel(), Enums.EnumFormType.Edit) { Owner = this };

                    if (commonEditor.ShowDialog() == true) ((TestCase)commonEditor.DataContext).CopyFirstLevel(e.SelectedItem);

                    break;

                case UserControls.AdvancedListControl.EnumButtonType.Run:
                    if (Session.Instance.Project?.Workflows?.Sum(X => X.Actions.Count) > 0)
                    {
                        TestCaseRun testCaseRun = new ((TestCase?)alcTestCase.SelectedItem, true) { Owner = this };

                        testCaseRun.ShowDialog();
                    }

                    break;
            }
        }

        private void alcTestCaseWorkflow_ButtonClick(object sender, UserControls.AdvancedListControl.ButtonClickEventArgs e)
        {
            switch (e.ButtonType)
            {
                case UserControls.AdvancedListControl.EnumButtonType.Add:
                    TestCaseAddWorkFlow testCaseAddWorkFlow = new (Session.Instance.Project) { Owner = this };
                    if (testCaseAddWorkFlow.ShowDialog() == true)
                    {
                        foreach (Workflow each in testCaseAddWorkFlow.lstWorkflow.SelectedItems)
                        {
                            alcTestCaseWorkflow.AddNewItem(new TestCaseWorkflow(each));
                        }

                        //alcTestCaseWorkflow.AddNewItems(testCaseAddWorkFlow.lstWorkflow.SelectedItems);
                    }

                    break;
            }
        }

        private void alcWorkflow_SelectedItemChanged(object sender, UserControls.AdvancedListControl.SelectedItemChangedEventArgs e)
        {
            alcAction.ListCollection = ((Workflow?)e.SelectedItem)?.Actions;
        }

        private void alcAction_SelectedItemChanged(object sender, UserControls.AdvancedListControl.SelectedItemChangedEventArgs e)
        {

        }

        private void alcTestCase_SelectedItemChanged(object sender, UserControls.AdvancedListControl.SelectedItemChangedEventArgs e)
        {
            alcTestCaseWorkflow.ListCollection = ((TestCase?)e.SelectedItem)?.Workflows;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ProcessSaveIfRequired() == null)
            {
                e.Cancel = true;
            }
            else
            {
                Hide();
                Run.seleniumService.WebDriver?.Quit();
            }
        }
    }
}
#pragma warning disable CS8602 // Dereference of a possibly null reference.

using QA_Master.Models.Core;
using QA_Master.Models.Extensions;
using QA_Master.Services.Core;
using QA_Master.Services.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace QA_Master.Forms
{
    /// <summary>
    /// Interaction logic for TestCaseRun.xaml
    /// </summary>
    public partial class TestCaseRun : Window
    {
        public TestCaseRun() => InitializeComponent();

        private TestCase? TestCase = null;
        public bool TestCaseRunning = false;
        public bool RunAllAction = false;

        private bool seleniumServiceRunning = false;
        private bool pdfServiceRunning = false;
        private bool serviceRunning { get { return seleniumServiceRunning || pdfServiceRunning; } }

        public TestCaseRun(TestCase? testCase, bool testCaseRunnning = false)
        {
            InitializeComponent();

            TestCase = testCase.CopyTo();
            TestCaseRunning = testCaseRunnning;

            cmbBrowser.ItemsSource = typeof(EnumBrowser).GetAllEnumNames();
            cmbBrowser.SelectedIndex = 0;

            cmbBaseURL.ItemsSource = Session.Instance.Project?.BaseURLs?.Replace("\r", "").Split('\n');
            cmbBaseURL.SelectedIndex = 0;

            if (string.IsNullOrWhiteSpace(TestCase?.Name) == false) Title = $"{Title} [{TestCase}]";

            TestCase.AddDataBinding(X => X.Workflows, lstWorkflows);

            chkRunAllAction.Visibility = TestCaseRunning ? Visibility.Collapsed : Visibility.Visible;
            btnGenerateReport.Visibility = Visibility.Hidden;

            pBar.Visibility = Visibility.Hidden;
            if (TestCase.Workflows != null) pBar.Maximum = TestCase.Workflows.Sum(X => X.Workflow.Actions.Count);

            if (TestCaseRunning == false)
            {
                lstWorkflows.SelectedIndex = TestCase.WorkflowSelectedIndex;
                lstActions.SelectedIndex = TestCase.ActionSelectedIndex;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (seleniumServiceRunning) e.Cancel = true;
        }

        int previousActionsCount = 0;
        private void lstWorkflows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BindActionList();

            previousActionsCount = 0;
            for (int i = lstWorkflows.SelectedIndex - 1; i >= 0; i--)
            {
                previousActionsCount += TestCase.Workflows[i].Workflow.Actions.Count;
            }
        }

        private void lstActions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pBar.Value = previousActionsCount + lstActions.SelectedIndex + 1;
        }

        private readonly Run Run = new ();
        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            Run.Setup_Run(this);

            TestCase.BaseURL = cmbBaseURL.Text;

            TestCaseWorkflow? workflow = null;
            if (TestCaseRunning == true)
            {
                workflow = (TestCaseWorkflow)lstWorkflows.Items[lstWorkflows.SelectedIndex];
                for (int i = lstActions.SelectedIndex; i < lstActions.Items.Count; i++)
                {
                    workflow.Actions[i].RunStatus = Models.Enums.EnumRunStatus.None;
                }

                for (int i = lstWorkflows.SelectedIndex + 1; i < lstWorkflows.Items.Count; i++)
                {
                    workflow = (TestCaseWorkflow)lstWorkflows.Items[i];
                    workflow.Workflow.Setup_RunStatus();
                }
            }

            int workflowSelectedIndex = lstWorkflows.SelectedIndex;
            int actionSelectionIndex = lstActions.SelectedIndex;

            if (workflowSelectedIndex < 0) workflowSelectedIndex = 0;
            if (actionSelectionIndex < 0) actionSelectionIndex = 0;

            seleniumServiceRunning = true;
            RunAllAction = chkRunAllAction.IsChecked == true;
            pBar.Visibility = TestCaseRunning ? Visibility.Visible : Visibility.Hidden;
            Enable_Disable_Controls();

            try
            {
                await Task.Run(async ()=> await Run.StartSeleniumService(workflowSelectedIndex, actionSelectionIndex));
                //await Task.Run(Run.StartSeleniumService(workflowSelectedIndex, actionSelectionIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            seleniumServiceRunning = false;
            Enable_Disable_Controls();
            pBar.Visibility = Visibility.Hidden;

            TestCase.BaseURL = null;
        }

        private void Enable_Disable_Controls()
        {
            float opacity;
            if (seleniumServiceRunning && TestCaseRunning)
            {
                opacity = 1f;// 0.7f;

                Session.Instance.MainWindow.Opacity = opacity;
                //Session.Instance.MainWindow.WindowStyle = WindowStyle.None;
                //Session.Instance.MainWindow.AllowsTransparency = true;
                Session.Instance.MainWindow.Topmost = true;
                
                Opacity = opacity;
                //WindowStyle = WindowStyle.None;
                //AllowsTransparency = true;
            }
            else
            {
                opacity = 1f;

                Session.Instance.MainWindow.Opacity = opacity;
                ///Session.Instance.MainWindow.AllowsTransparency = false;
                //Session.Instance.MainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                Session.Instance.MainWindow.Topmost = false;

                Opacity = opacity;
                //WindowStyle = WindowStyle.ToolWindow;
                //AllowsTransparency = true;
            }

            cmbBrowser.IsEnabled = !serviceRunning;
            cmbBaseURL.IsEnabled = !serviceRunning;

            btnRun.IsEnabled = !serviceRunning;
            btnGenerateReport.IsEnabled = !serviceRunning;
            btnClose.IsEnabled = !serviceRunning;

            lstWorkflows.IsEnabled = !serviceRunning;
            chkRunAllAction.IsEnabled = !serviceRunning;
            lstActions.IsEnabled = !serviceRunning;

            pBar.Visibility = seleniumServiceRunning ? Visibility.Visible : Visibility.Hidden;

            int index = lstActions.SelectedIndex;
            BindingOperations.ClearBinding(lstActions, ItemsControl.ItemsSourceProperty);

            if (seleniumServiceRunning)
            {
                lstActions.ItemsSource = null;
                lstActions.Items.Add(new Models.Core.Action($"Selenium Service initializing with {cmbBrowser.SelectedItem} Browser..."));
                lstActions.SelectedIndex = 0;
                Thread.Sleep(1000);
            }
            else if (pdfServiceRunning)
            {
                lstActions.ItemsSource = null;
                lstActions.Items.Add($"PDF Service initializing...");
                lstActions.SelectedIndex = 0;
            }
            else
            {
                BindActionList();

                lstActions.SelectedIndex = index;
                lstActions.BringIntoView();
            }

            btnGenerateReport.Visibility = TestCaseRunning ? Visibility.Visible : Visibility.Hidden;
        }

        public void BindActionList(int? selectedIndex = null)
        {            
            if (selectedIndex != null && selectedIndex < 0) selectedIndex = 0;

            if (lstWorkflows.SelectedItem != null)
                ((TestCaseWorkflow)lstWorkflows.SelectedItem).AddDataBinding(X => X.Actions, lstActions);

            if (selectedIndex != null && lstActions.Items.Count > selectedIndex) lstActions.SelectedIndex = selectedIndex.Value;
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

#pragma warning disable CS8602 // Dereference of a possibly null reference.

using QA_Master.Forms;
using QA_Master.Models.Core;
using QA_Master.Models.Enums;
using QA_Master.Services.Enums;
using System.Windows;
using System.Windows.Controls;

namespace QA_Master.Services.Core
{
    public class Run
    {
        public static SeleniumService seleniumService { get; private set; } = new SeleniumService();
        private TestCaseRun? testCaseRun;

        public void Setup_Run(TestCaseRun testCaseRun)
        {
            this.testCaseRun = testCaseRun;

            seleniumService.Setup_SeleniumService((EnumBrowser)this.testCaseRun.cmbBrowser.SelectedIndex, 6, this);
        }

        public async Task<bool> StartSeleniumService(int workflowSelectedIndex, int actionSelectedIndex)
        {
            if (testCaseRun == null) { return false; }

            seleniumService.StartService();

            TestCaseWorkflow? workflow = null;

            int workflowStart, workflowStop, actionStart, actionStop;

            if (testCaseRun?.TestCaseRunning == true)
            {
                workflowStart = 0;
                workflowStart = workflowSelectedIndex;
                workflowStop = GetListBoxItemCount(testCaseRun.lstWorkflows) - 1;
            }
            else
            {
                workflowStart = workflowSelectedIndex;
                workflowStop = workflowStart;
            }

            //init Action Binding
            testCaseRun.Dispatcher.Invoke(() => { testCaseRun.BindActionList(); return; });

            for (int w = workflowStart; w <= workflowStop; w++)
            {
                workflow = (TestCaseWorkflow?)GetListBoxItem(testCaseRun?.lstWorkflows, w);
                if (workflow != null) SetRunStatus(workflow.Workflow, EnumRunStatus.Running);

                SetListBoxListIndex(testCaseRun?.lstWorkflows, w);

                if (testCaseRun != null && (testCaseRun.TestCaseRunning == true || testCaseRun.RunAllAction == true))
                {
                    //SetListBoxListIndex(frmTestCaseRun.lstWorkflows, w);
                    actionStart = 0;
                    actionStart = actionSelectedIndex;
                    actionStop = GetListBoxItemCount(testCaseRun.lstActions) - 1;
                }
                else
                {
                    actionStart = actionSelectedIndex;
                    actionStop = actionStart;
                }

                bool actionStatus = false;
                for (int i = actionStart; i <= actionStop; i++)
                {
                    SetListBoxListIndex(testCaseRun?.lstActions, i);

                    Models.Core.Action? action = (Models.Core.Action?)GetListBoxItem(testCaseRun?.lstActions, i);

                    actionStatus = await seleniumService.Start(action);
                    if (actionStatus == false) break;
                }

                if (workflow != null)
                {
                    int failedCount = 0;
                
                    if (workflow.Actions != null)
                        failedCount = workflow.Actions.Count(X => (GetRunStatus(X) == EnumRunStatus.Failed) || (GetRunStatus(X.Perform) == EnumRunStatus.Failed));
                
                    SetRunStatus(workflow.Workflow, failedCount == 0 ? EnumRunStatus.Passed : EnumRunStatus.PartiallyPassed);
                }

                if (actionStatus == false) break;

                actionSelectedIndex = 0;
            }

            return true;
        }

        public async Task<bool> StartPDFService(string fileName, string headerPrefix)
        {
            bool result = true;

            Workflow? workflow = null;
            PDFService pDFService = new PDFService(fileName);
            for (int w = 0; w < testCaseRun?.lstWorkflows.Items.Count; w++)
            {
                workflow = (Workflow)testCaseRun.lstWorkflows.Items[w];
                //?
                SetListBoxListIndex(testCaseRun.lstWorkflows, w);

                for (int i = 0; i < testCaseRun.lstActions.Items.Count; i++)
                {
                    SetListBoxListIndex(testCaseRun.lstActions, i);

                    Models.Core.Action action = (Models.Core.Action)testCaseRun.lstActions.Items[i];

                    result = await pDFService.Start(headerPrefix, workflow, action);

                    if (result == false) break;
                }
            }

            pDFService.End();

            return result;
        }

        #region Helper Events
        public EnumRunStatus GetRunStatus(Common? common)
        {
            EnumRunStatus status = EnumRunStatus.None;

            testCaseRun.Dispatcher.Invoke(() => { if (common != null) status = common.RunStatus; return; });

            return status;
        }

        public void SetRunStatus (Common? common, EnumRunStatus status)
        {
            testCaseRun.Dispatcher.Invoke(() => { if (common != null) common.RunStatus = status; return; });
        }

        private object? GetListBoxItem(ListBox? listBox, int index)
        {
            object? item = null;

            testCaseRun.Dispatcher.Invoke(() => { if (listBox != null) item = listBox.Items[index]; return; });

            return item;
        }

        private int GetListBoxItemCount(ListBox? listBox)
        {
            int count = 0;

            testCaseRun.Dispatcher.Invoke(() => { if (listBox != null) count = listBox.Items.Count; return; });

            return count;
        }

        private int GetListBoxSelectedIndex(ListBox listBox)
        {
            int selectedIndex = 0;

            testCaseRun.Dispatcher.Invoke(() => { selectedIndex = listBox.SelectedIndex; return; });

            return selectedIndex;
        }

        private void SetListBoxListIndex(ListBox? listBox, int i)
        {
            testCaseRun.Dispatcher.Invoke(() =>
            {
                if (listBox != null)
                {
                    if (listBox.SelectedIndex != i)
                    {
                        listBox.SelectedIndex = i;
                        //listBox.BringIntoView();

                        if (listBox.Items.Count - 2 >= i )
                            listBox.ScrollIntoView(listBox.Items[i + 1]);
                        else
                            listBox.ScrollIntoView(listBox.SelectedItem);

                        if (listBox == testCaseRun?.lstWorkflows) testCaseRun.BindActionList();
                    }
                }

                return;
            });
        }

        public void ManualInterrupt(string? message = null)
        {
            if (string.IsNullOrWhiteSpace(message) == true) message = "We are waiting for your interaction with Web Browser.\n\n" +
                "Once you completed, please click 'Continue' to move on.";

            testCaseRun.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(testCaseRun, message, "Manual Interrupt", MessageBoxButton.OK);
                return;
            });
        }
        #endregion
    }
}

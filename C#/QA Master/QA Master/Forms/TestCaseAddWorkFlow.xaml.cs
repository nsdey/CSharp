using QA_Master.Models.Core;
using System.Windows;

namespace QA_Master.Forms
{
    /// <summary>
    /// Interaction logic for TestCaseAddWorkFlow.xaml
    /// </summary>
    public partial class TestCaseAddWorkFlow : Window
    {
        public TestCaseAddWorkFlow() => InitializeComponent();

        public TestCaseAddWorkFlow(Project? dataContext)
        {
            InitializeComponent();

            DataContext = dataContext;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

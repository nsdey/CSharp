#pragma warning disable CS8602 // Dereference of a possibly null reference.

using QA_Master.Forms.Enums;
using QA_Master.Models.Core;
using QA_Master.Models.Extensions;
using System.Windows;

namespace QA_Master.Forms
{
    /// <summary>
    /// Interaction logic for CommonEditor.xaml
    /// </summary>
    public partial class CommonEditor : Window
    {
        public CommonEditor() => InitializeComponent();
        public CommonEditor(object? dataContext, EnumFormType enumFormType)
        {
            InitializeComponent();

            btnSave.Content = enumFormType == EnumFormType.Add ? "Add" : enumFormType == EnumFormType.Edit ? "Save" : btnSave.Content;

            DataContext = dataContext;

            if (dataContext != null )
            {
                switch (dataContext)
                {
                    case Workflow workflow:
                        workflow.AddDataBinding(X => X.Name, txtTitle, lblTitle);
                        break;
                    case TestCase testCase:
                        testCase.AddDataBinding(X => X.Name, txtTitle, lblTitle);
                        break;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
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

using QA_Master.Forms.Enums;
using QA_Master.Models.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace QA_Master.Forms
{
    /// <summary>
    /// Interaction logic for ActionFindElementEditor.xaml
    /// </summary>
    public partial class ActionEditor : Window
    {
        List<Models.Core.Action?>? actions = null;
        public void _BaseConstructor()
        {
            InitializeComponent();
        }

        public ActionEditor() => _BaseConstructor();

        public ActionEditor(object? dataContext, EnumFormType enumFormType)
        {
            _BaseConstructor();

            btnSave.Content = enumFormType == EnumFormType.Add ? "Add" : enumFormType == EnumFormType.Edit ? "Save" : btnSave.Content;

            Models.Core.Action? selectedAction = null;
            actions = Extension.GetAllActionClasses(out selectedAction, "Select Action...", (Models.Core.Action?)dataContext);
            cmbAction.ItemsSource = actions;

            if (selectedAction != null)
            {
                cmbAction.SelectedItem = selectedAction;
            }
            else
            {
                cmbAction.SelectedIndex = 0;
            }
        }

        private void cmbAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Models.Core.Action? selectedAction = (Models.Core.Action?)e.AddedItems[0];

            DataContext = selectedAction?.EditModel;
            frmAction.Content = selectedAction?.GetAttachedPage(selectedAction.EditModel);
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

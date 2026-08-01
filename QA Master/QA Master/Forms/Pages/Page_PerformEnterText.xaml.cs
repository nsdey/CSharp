#pragma warning disable CS8602 // Dereference of a possibly null reference.

using QA_Master.Models.Core;
using QA_Master.Models.Extensions;
using QA_Master.Models.Utilities;
using System.Windows;
using System.Windows.Controls;

namespace QA_Master.Forms.Pages
{
    /// <summary>
    /// Interaction logic for PerformCommon.xaml
    /// </summary>
    public partial class Page_PerformEnterText : Page
    {
        public void _BaseConstructor()
        {
            InitializeComponent();
        }

        public Page_PerformEnterText() => _BaseConstructor();

        public Page_PerformEnterText(object dataContext)
        {
            _BaseConstructor();

            DataContext = dataContext;

            switch ((Perform)dataContext)
            {
                case PerformEnterText performEnterText:
                    performEnterText.AddDataBinding(X => X.Text, txtValue, lblValue);
                    break;
            }
        }

        private void btnInsertExpression_Click(object sender, RoutedEventArgs e)
        {
            btnInsertExpression_ContextMenu.PlacementTarget = (UIElement)sender;
            btnInsertExpression_ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;

            if (btnInsertExpression_ContextMenu.Items.Count == 0 || true)
            {
                btnInsertExpression_ContextMenu.ItemsSource = new ExpressionEvaluator().DataSource(txtValue);
            }

            btnInsertExpression_ContextMenu.IsOpen = true;
        }

        private void txtValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtPreview.Text = ExpressionEvaluator.Evaluate(txtValue.Text);
        }
    }
}

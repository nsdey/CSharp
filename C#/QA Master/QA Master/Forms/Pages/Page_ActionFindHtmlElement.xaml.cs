using QA_Master.Models.Core;
using System.Windows.Controls;

namespace QA_Master.Forms.Pages
{
    /// <summary>
    /// Interaction logic for ActionFindElement.xaml
    /// </summary>
    public partial class Page_ActionFindHtmlElement : Page
    {
        private void _BaseConstructor()
        {
            InitializeComponent();
        }

        public Page_ActionFindHtmlElement() => _BaseConstructor();

        public Page_ActionFindHtmlElement(object dataContext)
        {
            _BaseConstructor();

            DataContext = dataContext;
        }

        private void cmbPerform_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Perform? perform = (Perform?)(e.AddedItems.Count > 0 ? e.AddedItems[0] : e.RemovedItems[0]);

            ((Models.Core.Action)DataContext).Perform = (Perform?)perform?.EditModel;

            frmPerform.Content = perform?.GetAttachedPage(perform.EditModel);
        }
    }
}

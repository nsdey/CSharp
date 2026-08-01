#pragma warning disable CS8602 // Dereference of a possibly null reference.

using QA_Master.Models.Core;
using QA_Master.Models.Extensions;
using System.Windows.Controls;

namespace QA_Master.Forms.Pages
{
    /// <summary>
    /// Interaction logic for PerformCommon.xaml
    /// </summary>
    public partial class Page_PerformCommon : Page
    {
        public void _BaseConstructor()
        {
            InitializeComponent();
        }

        public Page_PerformCommon() => _BaseConstructor();

        public Page_PerformCommon(object dataContext)
        {
            _BaseConstructor();

            DataContext = dataContext;

            switch ((Perform)dataContext)
            {
                case PerformClick performClick:
                    performClick.AddDataBinding(X => X.Seconds, txtValue, lblValue);
                    break;

                case PerformChooseDropdownItem performChooseDropdownItem:
                    performChooseDropdownItem.AddDataBinding(X => X.ByText, txtValue, lblValue);
                    break;

                case PerformClickOffset performClickOffset:
                    performClickOffset.AddDataBinding(X => X.Offset, txtValue, lblValue);
                    break;

                case PerformMatchTitle performMatchTitle:
                    performMatchTitle.AddDataBinding(X => X.Title, txtValue, lblValue);
                    break;

                case PerformMatchUIText performMatchUIText:
                    performMatchUIText.AddDataBinding(X => X.UIText, txtValue, lblValue);
                    break;
            }
        }
    }
}

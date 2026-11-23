#pragma warning disable CS8602 // Dereference of a possibly null reference.

using QA_Master.Models.Core;
using QA_Master.Models.Extensions;
using System.Windows.Controls;

namespace QA_Master.Forms.Pages
{
    /// <summary>
    /// Interaction logic for Page_ActionCommon.xaml
    /// </summary>
    public partial class Page_ActionCommon : Page
    {
        private void _BaseConstructor()
        {
            InitializeComponent();
        }

        public Page_ActionCommon() => _BaseConstructor();

        public Page_ActionCommon(object dataContext)
        {
            InitializeComponent();

            DataContext = dataContext;

            if (DataContext != null)
            {
                switch ((Models.Core.Action?)DataContext)
                {
                    case ActionManualInterrupt actionManualInterrupt:
                        actionManualInterrupt.AddDataBinding(X => X.Message, txtValue, lblValue);
                        break;
                    case ActionMatchTitle actionMatchTitle:
                        actionMatchTitle.AddDataBinding(X => X.Title, txtValue, lblValue);
                        break;
                    case ActionMatchURL actionMatchURL:
                        actionMatchURL.AddDataBinding(X => X.URL, txtValue, lblValue);
                        break;
                    case ActionNavigate actionNavigate:
                        actionNavigate.AddDataBinding(X => X.URL, txtValue, lblValue);
                        break;
                    case ActionWait actionWait:
                        actionWait.AddDataBinding(X => X.Seconds, txtValue, lblValue);
                        break;
                }
            }
        }
    }
}

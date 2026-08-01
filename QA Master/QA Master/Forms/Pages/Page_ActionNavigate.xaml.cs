#pragma warning disable CS8602 // Dereference of a possibly null reference.

using QA_Master.Models.Core;
using QA_Master.Models.Extensions;
using System.Windows.Controls;

namespace QA_Master.Forms.Pages
{
    /// <summary>
    /// Interaction logic for Page_ActionCommon.xaml
    /// </summary>
    public partial class Page_ActionNavigate : Page
    {
        public Page_ActionNavigate() => InitializeComponent();

        public Page_ActionNavigate(object dataContext)
        {
            InitializeComponent();

            DataContext = dataContext;

            if (DataContext != null)
            {
                if ((Models.Core.Action?)DataContext is ActionNavigate actionNavigate)
                {
                    actionNavigate.AddDataBinding(X => X.URL, txtValue, lblValue);
                }
            }
        }
    }
}

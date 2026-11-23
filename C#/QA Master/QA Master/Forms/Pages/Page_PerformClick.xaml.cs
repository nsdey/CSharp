#pragma warning disable CS8602 // Dereference of a possibly null reference.

using QA_Master.Models.Core;
using QA_Master.Models.Extensions;
using System.Windows.Controls;

namespace QA_Master.Forms.Pages
{
    /// <summary>
    /// Interaction logic for PerformCommon.xaml
    /// </summary>
    public partial class Page_PerformClick : Page
    {
        public void _BaseConstructor()
        {
            InitializeComponent();
        }

        public Page_PerformClick() => _BaseConstructor();

        public Page_PerformClick(object dataContext)
        {
            _BaseConstructor();

            DataContext = dataContext;

            switch ((Perform)dataContext)
            {
                case PerformClick performClick:
                    performClick.AddDataBinding(X => X.Seconds, txtValue, lblValue);
                    performClick.AddDataBinding(X => X.CheckElementExists, chkCheckElementExists, lblCheck, false);
                    break;
            }
        }
    }
}

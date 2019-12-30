using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(VaultReportViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VaultReportView : PageNavigationAwareView
    {

        #region constructor / destructor

        public VaultReportView()
		{
            InitializeComponent();
        }

        #endregion

    }
}
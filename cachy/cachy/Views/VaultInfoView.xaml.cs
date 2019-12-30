using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(VaultInfoViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VaultInfoView : PageNavigationAwareView
    {

        #region constructor / destructor

        public VaultInfoView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
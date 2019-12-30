using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{


    [ViewModelMapping(typeof(VaultViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VaultView : PageNavigationAwareView
    {

        #region constructor / destructor

        public VaultView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
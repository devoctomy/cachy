using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(AddExistingVaultViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddExistingVaultView : PageNavigationAwareView
    {

        #region constructor / destructor

        public AddExistingVaultView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
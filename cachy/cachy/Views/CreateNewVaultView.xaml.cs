using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(CreateVaultViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateNewVaultView : PageNavigationAwareView
    {

        #region constructor / destructor

        public CreateNewVaultView ()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
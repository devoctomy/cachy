using cachy.Navigation.BurgerMenu;
using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(VaultListViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VaultListView : PageNavigationAwareView
    {
        
        #region constructor / destructor

        public VaultListView ()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
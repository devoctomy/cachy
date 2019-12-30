using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(AddCloudProviderViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddCloudProviderView : PageNavigationAwareView
    {

        #region constructor / destructor

        public AddCloudProviderView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
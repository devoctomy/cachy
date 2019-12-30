using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(CloudProviderFileSelectViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CloudProviderFileSelectView : PageNavigationAwareView
    {

        #region constructor / destructor

        public CloudProviderFileSelectView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
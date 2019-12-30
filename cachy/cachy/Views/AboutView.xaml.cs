using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(AboutViewModel), ViewModelMappingAttribute.InstancingType.Single)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AboutView : PageNavigationAwareView
    {

        #region constructor / destructor

        public AboutView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
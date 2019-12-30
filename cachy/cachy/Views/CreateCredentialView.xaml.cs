using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(CreateCredentialViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateCredentialView : PageNavigationAwareView
    {

        #region constructor / destructor

        public CreateCredentialView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
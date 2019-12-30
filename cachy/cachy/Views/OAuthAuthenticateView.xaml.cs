using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(OAuthAuthenticateViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OAuthAuthenticateView : PageNavigationAwareView
    {

        #region constructor / destructor

        public OAuthAuthenticateView()
        {
            InitializeComponent();
        }

        #endregion

    }

}
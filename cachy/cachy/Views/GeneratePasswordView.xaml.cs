using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(GeneratePasswordViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GeneratePasswordView : PageNavigationAwareView
    {

        #region constructor / destructor

        public GeneratePasswordView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
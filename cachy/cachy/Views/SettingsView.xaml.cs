using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(SettingsViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsView : PageNavigationAwareView
    {

        #region constructor / destructor

        public SettingsView ()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
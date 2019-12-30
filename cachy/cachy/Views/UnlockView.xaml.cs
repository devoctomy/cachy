using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(UnlockViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UnlockView : PageNavigationAwareView
    {

        #region constructor / destructor

        public UnlockView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
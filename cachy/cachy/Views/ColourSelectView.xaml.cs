using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(ColourSelectViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ColourSelectView : PageNavigationAwareView
    {

        #region constructor / destructor

        public ColourSelectView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
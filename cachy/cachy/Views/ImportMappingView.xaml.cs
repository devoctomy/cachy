using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(ImportMappingViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ImportMappingView : PageNavigationAwareView
    {

        #region constructor / destructor

        public ImportMappingView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
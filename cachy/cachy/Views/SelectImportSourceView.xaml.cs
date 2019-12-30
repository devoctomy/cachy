using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(SelectImportSourceViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectImportSourceView : PageNavigationAwareView
    {

        #region constructor / destructor

        public SelectImportSourceView()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
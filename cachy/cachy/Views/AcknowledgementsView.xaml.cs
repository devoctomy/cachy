using cachy.Fonts;
using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(AcknowledgementsViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AcknowledgementsView : PageNavigationAwareView
    {

        #region constructor / destructor

        public AcknowledgementsView()
		{
            InitializeComponent();
        }

        #endregion

    }
}
using cachy.Fonts;
using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(ChangeLogViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChangeLogView : PageNavigationAwareView
    {

        #region constructor / destructor

        public ChangeLogView()
		{
            InitializeComponent();
        }

        #endregion

    }
}
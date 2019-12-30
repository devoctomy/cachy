using cachy.Fonts;
using cachy.ViewModels;
using cachy.ViewModels.Attributes;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Views
{

    [ViewModelMapping(typeof(AmazonS3SetupViewViewModel), ViewModelMappingAttribute.InstancingType.Multiple)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AmazonS3SetupView : PageNavigationAwareView
    {

        #region constructor / destructor

        public AmazonS3SetupView()
		{
            InitializeComponent();
        }

        #endregion

    }
}
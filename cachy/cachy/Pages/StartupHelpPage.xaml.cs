using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Pages
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StartupHelpPage : ContentPage
	{

        #region constructor / destructor

        public StartupHelpPage ()
		{
			InitializeComponent ();
		}

        #endregion

        #region object events

        private void Back_Clicked(object sender, EventArgs e)
        {
            ((App)App.Current).DisplayLandingPage();
        }

        private void HelpCenterButton_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(AppConstants.DEVOCTOMY_SUPPORT_URL));
        }

        private async void ResetButton_Clicked(object sender, EventArgs e)
        {
            if(await ((App)App.Current).ConfirmReset(
                false,
                this))
            {
                ((App)App.Current).DisplayLandingPage();
            }
        }

        #endregion

    }

}
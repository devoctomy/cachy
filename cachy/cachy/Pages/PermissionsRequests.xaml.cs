using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Pages
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PermissionsRequests : CarouselPage
	{

        #region private objects

        public PermissionsRequests()
		{
			InitializeComponent ();
		}

        #endregion

        #region base class overrides

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        #endregion

        #region object events

        private void Button_Clicked(object sender, EventArgs e)
        {
            ((App)App.Current).RequestPermissions();
        }

        #endregion

    }

}
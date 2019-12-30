using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Pages
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LandingPage : ContentPage
	{

        #region constructor / destructor

        public LandingPage()
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

        private void LogoButton_Clicked(object sender, EventArgs e)
        {
            ((App)App.Current).PerformStartup();
        }

        private void StartupProblemsButton_Clicked(object sender, EventArgs e)
        {
            ((App)App.Current).StartupProblems();
        }

        #endregion

    }

}
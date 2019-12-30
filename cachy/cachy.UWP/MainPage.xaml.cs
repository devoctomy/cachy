using cachy.UWP.Cryptography;
using cachy.UWP.IO;
using cachy.UWP.Web;
using Windows.UI.Xaml.Navigation;

namespace cachy.UWP
{

    public sealed partial class MainPage
    {

        #region private objects

        private cachy.App _app;

        #endregion

        #region constructor / destructor

        public MainPage()
        {
            this.InitializeComponent();

            devoctomy.cachy.Framework.Native.Native.FileHandler = new NativeFileHandler();
            devoctomy.cachy.Framework.Native.Native.PasswordVault = new NativePasswordVault();
            devoctomy.cachy.Framework.Native.Native.WebUtility = new NativeWebUtility();

            _app = new cachy.App(
                CheckPermissions, 
                RequestPermissions,
                null);

            LoadApplication(_app);
        }

        #endregion

        #region base class overrides

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _app.ProcessArguments(e.Parameter);
        }

        #endregion

        #region private methods

        private bool CheckPermissions()
        {
            return (true);
        }

        private void RequestPermissions()
        {

        }

        #endregion

    }

}

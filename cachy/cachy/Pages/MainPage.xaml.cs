using cachy.Navigation.BurgerMenu;
using devoctomy.cachy.Framework.Data;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Pages
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : BurgerMenuHostPage
    {

        #region private objects

        private bool _checkPasswordVault;

        #endregion

        #region public properties

        public string VersionText
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
#if DEBUG
                return (string.Format("v{0}D", assembly.GetName().Version.ToString()));
#else
                return (string.Format("v{0}", assembly.GetName().Version.ToString()));
#endif
            }
        }

#endregion

        #region constructor / destructor

        public MainPage ()
		{
			InitializeComponent ();
		}

        #endregion

        #region base class events

        private async void Root_Appearing(object sender, System.EventArgs e)
        {
            if (!_checkPasswordVault)
            {
                Dictionary<string, string> passwordVaultCheck = devoctomy.cachy.Framework.Native.Native.PasswordVault.GetCredential("cachy.continuity");
                if (passwordVaultCheck == null)
                {
                    if (VaultIndexFile.Instance.Indexes.Count > 0)
                    {
                        IEnumerable<VaultIndex> cloudSynced = VaultIndexFile.Instance.Indexes.Where(vi => vi.SyncMode == Common.SyncMode.CloudProvider);
                        if (cloudSynced.Any() && !App.NoVaultsOnStartup)
                        {
                            await DisplayAlert("Warning!",
                                "cachy appears to have been uninstalled and then reinstalled manually, you will need to re-authenticate with your cloud providers within the app settings 'SYNC' tab.",
                                "OK");
                        }
                    }
                    devoctomy.cachy.Framework.Native.Native.PasswordVault.StoreCredential(
                        "cachy.continuity",
                        "-",
                        "-",
                        true);
                }

                _checkPasswordVault = true;
            }
        }

        #endregion

    }

}
using cachy.Config;
using cachy.Navigation.BurgerMenu;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Cryptography.KeyDerivation;
using devoctomy.cachy.Framework.Cryptography.OAuth;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Data.Cloud;
using devoctomy.DFramework.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class SettingsViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private SettingsView _view;
        private String _selectedTabPage = String.Empty;
        private CloudProvider _selectedCloudProvider;
        private bool _loggingEnabled = false;
        private string _testSettingsButtonText = "Test Settings";

        private ICommand _addCloudProvider;
        private ICommand _resetCommand;
        private ICommand _testDerivationSettingsCommand;

        #endregion

        #region public properties

        public SettingsView View
        {
            get
            {
                return (_view);
            }
        }

        public String SelectedTabPage
        {
            get
            {
                return (_selectedTabPage);
            }
            set
            {
                if (_selectedTabPage != value)
                {
                    _selectedTabPage = value;
                    NotifyPropertyChanged("SelectedTabPage");
                }
            }
        }

        public IEnumerable<VaultIndex> Vaults
        {
            get
            {
                return (VaultIndexFile.Instance.Indexes);
            }
        }

        public bool AutoOpenVault
        {
            get
            {
                return (AppConfig.Instance.AutoOpenVault);
            }
            set
            {
                if(AppConfig.Instance.AutoOpenVault != value)
                {
                    if(VaultIndexFile.Instance.Indexes.Count == 0 & value)
                    {
                        value = false;
                    }
                    AppConfig.Instance.AutoOpenVault = value;
                    DefaultVault = DefaultVault;
                    NotifyPropertyChanged("AutoOpenVault");
                }
            }
        }

        public bool AutoSave
        {
            get
            {
                return (AppConfig.Instance.AutoSave);
            }
            set
            {
                if(AppConfig.Instance.AutoSave != value)
                {
                    AppConfig.Instance.AutoSave = value;
                    NotifyPropertyChanged("AutoSave");
                }
            }
        }

        public bool AutoSaveOnAcceptCredChanges
        {
            get
            {
                return (AppConfig.Instance.AutoSaveOnAcceptCredChanges);
            }
            set
            {
                if(AppConfig.Instance.AutoSaveOnAcceptCredChanges != value)
                {
                    AppConfig.Instance.AutoSaveOnAcceptCredChanges = value;
                    NotifyPropertyChanged("AutoSaveOnAcceptCredChanges");
                }
            }
        }

        public bool AutoSaveOnDuplicatingCred
        {
            get
            {
                return (AppConfig.Instance.AutoSaveOnDuplicatingCred);
            }
            set
            {
                if(AppConfig.Instance.AutoSaveOnDuplicatingCred != value)
                {
                    AppConfig.Instance.AutoSaveOnDuplicatingCred = value;
                    NotifyPropertyChanged("AutoSaveOnDuplicatingCred");
                }
            }
        }

        public bool AutoSaveOnDeletingCred
        {
            get
            {
                return (AppConfig.Instance.AutoSaveOnDeletingCred);
            }
            set
            {
                if (AppConfig.Instance.AutoSaveOnDeletingCred != value)
                {
                    AppConfig.Instance.AutoSaveOnDeletingCred = value;
                    NotifyPropertyChanged("AutoSaveOnDeletingCred");
                }
            }
        }

        public VaultIndex DefaultVault
        {
            get
            {
                IEnumerable<VaultIndex> defaultVaultMatches = VaultIndexFile.Instance.Indexes.Where(vi => vi.ID == AppConfig.Instance.DefaultVaultID);
                if (defaultVaultMatches.Any())
                {
                    return(defaultVaultMatches.First());
                }
                else
                {
                    return (null);
                }
            }
            set
            {
                if(value != null && AppConfig.Instance.DefaultVaultID != value.ID)
                {
                    AppConfig.Instance.DefaultVaultID = value.ID;
                    NotifyPropertyChanged("DefaultVault");
                }
            }
        }

        public bool AutoCloseVault
        {
            get
            {
                return (AppConfig.Instance.AutoCloseVault);
            }
            set
            {
                if(AppConfig.Instance.AutoCloseVault != value)
                {
                    AppConfig.Instance.AutoCloseVault = value;
                    NotifyPropertyChanged("AutoCloseVault");
                }
            }
        }

        public int AutoCloseVaultMinutes
        {
            get
            {
                return (AppConfig.Instance.AutoCloseTimeSpan.Minutes);
            }
            set
            {
                if(AppConfig.Instance.AutoCloseTimeSpan.TotalMinutes != value)
                {
                    AppConfig.Instance.AutoCloseTimeSpan = new TimeSpan(0, value, 0);
                    NotifyPropertyChanged("AutoCloseVaultMinutes");
                }
            }
        }

        public ObservableCollection<CloudProvider> ConfiguredCloudProviders
        {
            get
            {
                return (CloudProviders.Instance.Providers);
            }
        }

        public CloudProvider SelectedCloudProvider
        {
            get
            {
                return (_selectedCloudProvider);
            }
            set
            {
                if(_selectedCloudProvider != value)
                {
                    _selectedCloudProvider = value;
                    if (_selectedCloudProvider != null && _selectedCloudProvider.CredentialError)
                    {
                        ReauthenticateCloudProvider();
                    }
                }
            }
        }

        public string DefaultBrowseProtocol
        {
            get
            {
                return (AppConfig.Instance.DefaultBrowseProtocol);
            }
            set
            {
                if(AppConfig.Instance.DefaultBrowseProtocol != value)
                {
                    AppConfig.Instance.DefaultBrowseProtocol = value;
                    NotifyPropertyChanged("DefaultBrowseProtocol");
                }
            }
        }

        public bool ShowPasswordAge
        {
            get
            {
                return (AppConfig.Instance.ShowPasswordAge);
            }
            set
            {
                if(AppConfig.Instance.ShowPasswordAge != value)
                {
                    AppConfig.Instance.ShowPasswordAge = value;
                    NotifyPropertyChanged("ShowPasswordAge");
                }
            }
        }

        public int DaysForOld
        {
            get
            {
                return (AppConfig.Instance.DaysForOld);
            }
            set
            {
                if (AppConfig.Instance.DaysForOld != value)
                {
                    AppConfig.Instance.DaysForOld = value;
                    NotifyPropertyChanged("DaysForOld");
                }
            }
        }

        public bool PBKDF2Enabled
        {
            get
            {
                return (AppConfig.Instance.KeyDerivationFunction == AppConfig.CACHY_KEYDERIVATIONFUNCTION_PBKDF2);
            }
            set
            {
                if(value && AppConfig.Instance.KeyDerivationFunction != AppConfig.CACHY_KEYDERIVATIONFUNCTION_PBKDF2)
                {
                    AppConfig.Instance.KeyDerivationFunction = AppConfig.CACHY_KEYDERIVATIONFUNCTION_PBKDF2;
                }
                NotifyPropertyChanged("PBKDF2Enabled");
                NotifyPropertyChanged("SCryptEnabled");
            }
        }

        public string PBKDF2IterationCount
        {
            get
            {
                return (AppConfig.Instance.PBKDF2IterationCount.ToString());
            }
            set
            {
                try
                {
                    if (AppConfig.Instance.PBKDF2IterationCount != int.Parse(value))
                    {
                        AppConfig.Instance.PBKDF2IterationCount = int.Parse(value);
                        NotifyPropertyChanged("PBKDF2IterationCount");
                    }
                }
                catch (FormatException)
                {
                    App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception, "Invalid value entered for '{0}'.", "PBKDF2IterationCount");
                    NotifyPropertyChanged("PBKDF2IterationCount");
                }
            }
        }

        public bool SCryptEnabled
        {
            get
            {
                return (AppConfig.Instance.KeyDerivationFunction == AppConfig.CACHY_KEYDERIVATIONFUNCTION_SCRYPT);
            }
            set
            {
                if (value && AppConfig.Instance.KeyDerivationFunction != AppConfig.CACHY_KEYDERIVATIONFUNCTION_SCRYPT)
                {
                    AppConfig.Instance.KeyDerivationFunction = AppConfig.CACHY_KEYDERIVATIONFUNCTION_SCRYPT;
                }
                NotifyPropertyChanged("PBKDF2Enabled");
                NotifyPropertyChanged("SCryptEnabled");
            }
        }

        public string SCryptIterationCount
        {
            get
            {
                return (AppConfig.Instance.SCryptIterationCount.ToString());
            }
            set
            {
                try
                {
                    if (AppConfig.Instance.SCryptIterationCount != int.Parse(value))
                    {
                        AppConfig.Instance.SCryptIterationCount = int.Parse(value);
                        NotifyPropertyChanged("SCryptIterationCount");
                    }
                }
                catch (FormatException)
                {
                    App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception, "Invalid value entered for '{0}'.", "SCryptIterationCount");
                    NotifyPropertyChanged("SCryptIterationCount");
                }
            }
        }

        public string SCryptBlockSize
        {
            get
            {
                return (AppConfig.Instance.SCryptBlockSize.ToString());
            }
            set
            {
                try
                {
                    if (AppConfig.Instance.SCryptBlockSize != int.Parse(value))
                    {
                        AppConfig.Instance.SCryptBlockSize = int.Parse(value);
                        NotifyPropertyChanged("SCryptBlockSize");
                    }
                }
                catch (FormatException)
                {
                    App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception, "Invalid value entered for '{0}'.", "SCryptBlockSize");
                    NotifyPropertyChanged("SCryptBlockSize");
                }
            }
        }

        public string SCryptThreadCount
        {
            get
            {
                return (AppConfig.Instance.SCryptThreadCount.ToString());
            }
            set
            {
                try
                {
                    if (AppConfig.Instance.SCryptThreadCount != int.Parse(value))
                    {
                        AppConfig.Instance.SCryptThreadCount = int.Parse(value);
                        NotifyPropertyChanged("SCryptThreadCount");
                    }
                }
                catch (FormatException)
                {
                    App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception, "Invalid value entered for '{0}'.", "SCryptThreadCount");
                    NotifyPropertyChanged("SCryptThreadCount");
                }
            }
        }

        public bool EnableClipboardObfuscator
        {
            get
            {
                return (AppConfig.Instance.EnableClipboardObfuscator);
            }
            set
            {
                if(AppConfig.Instance.EnableClipboardObfuscator != value)
                {
                    AppConfig.Instance.EnableClipboardObfuscator = value;
                    NotifyPropertyChanged("EnableClipboardObfuscator");
                }
            }
        }

        public string ClipboardObfuscatorDisableSecondsAfterCopy
        {
            get
            {
                return (AppConfig.Instance.ClipboardObfuscatorDisableSecondsAfterCopy.ToString());
            }
            set
            {
                try
                {
                    if (AppConfig.Instance.ClipboardObfuscatorDisableSecondsAfterCopy != int.Parse(value))
                    {
                        AppConfig.Instance.ClipboardObfuscatorDisableSecondsAfterCopy = int.Parse(value);
                        NotifyPropertyChanged("ClipboardObfuscatorDisableSecondsAfterCopy");
                    }
                }
                catch (FormatException)
                {
                    App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception, "Invalid value entered for '{0}'.", "ClipboardObfuscatorDisableSecondsAfterCopy");
                    NotifyPropertyChanged("ClipboardObfuscatorDisableSecondsAfterCopy");
                }
            }
        }

        public string TestSettingsButtonText
        {
            get
            {
                return (_testSettingsButtonText);
            }
            set
            {
                if(_testSettingsButtonText != value)
                {
                    _testSettingsButtonText = value;
                    NotifyPropertyChanged("TestSettingsButtonText");
                }
            }
        }

        public bool LoggingEnabled
        {
            get
            {
                return (_loggingEnabled);
            }
            set
            {
                _loggingEnabled = value; ;
            }
        }

        public ICommand AddCloudProvider
        {
            get
            {
                return (_addCloudProvider);
            }
        }

        public ICommand ResetCommand
        {
            get
            {
                return (_resetCommand);
            }
        }

        public ICommand TestDerivationSettingsCommand
        {
            get
            {
                return (_testDerivationSettingsCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public SettingsViewModel(SettingsView view)
        {
            _view = view;

            _addCloudProvider = new Command(new Action<object>(AddCloudProviderAction));
            _resetCommand = new Command(new Action<object>(ResetCommandAction));
            _testDerivationSettingsCommand= new Command(new Action<object>(TestDerivationSettingsCommandAction));
        }

        #endregion

        #region base class overrides

        protected override bool OnValidate()
        {
            if (PBKDF2Enabled)
            {
                try
                {
                    int iterationCount = int.Parse(PBKDF2IterationCount);
                    if (iterationCount <= 0) return(false);
                }
                catch(Exception)
                {
                    return (false);
                }             
            }
            if(SCryptEnabled)
            {
                try
                {
                    int iterationCount = int.Parse(SCryptIterationCount);
                    int blockSize = int.Parse(SCryptBlockSize);
                    int threadCount = int.Parse(SCryptThreadCount);
                    if (iterationCount <= 0 && blockSize <= 0 && threadCount <= 0) return (false);
                }
                catch (Exception)
                {
                    return (false);
                }
            }

            return (true);
        }

        protected override void IsValidChanged(bool isValid)
        {
            EnableDisableAccept(isValid);
        }

        #endregion

        #region private methods

        private void EnableDisableAccept(bool enabled)
        {
            BurgerMenuItem accept = App.Controller.MainPageInstance.SelectedItem.GetChildItemByKey("accept");
            if (accept != null)
            {
                accept.IsEnabled = enabled;
            }
        }

        private void RefreshBindingContext()
        {
            AppConfig.Instance.Reload();
            View.BindingContext = null;
            View.BindingContext = this;
        }

        private async void ReauthenticateCloudProvider()
        {
            devoctomy.cachy.Framework.Native.Native.WebUtility.ClearInAppBrowserCache();
            Dictionary<string, object> parameters = await AuthenticationHelpers.BeginOAuthAuthentication(SelectedCloudProvider.ProviderKey);
            if (parameters != null)
            {
                App.Controller.ShowPopup("settings.oauth",
                    parameters.ToArray());
            }
            else
            {
                await App.Controller.MainPageInstance.DisplayAlert("Authentication Failure",
                    "Failed to start the authentication process, please try again later.",
                    "OK");
            }
        }

        private Tuple<bool, TimeSpan> TestPasswordDerivationFunction(int count)
        {
            TimeSpan elapsed = new TimeSpan(0);
            bool success = false;
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    TestSettingsButtonText = "Please Wait...";
                    View.IsEnabled = false;
                });
                Stopwatch stopWatch = new Stopwatch();
                string password = "";
                byte[] salt = new byte[16];
                if (PBKDF2Enabled)
                {
                    Rfc2898DeriveBytes pbkd2f = new Rfc2898DeriveBytes(
                        password,
                        salt,
                        int.Parse(PBKDF2IterationCount));

                    stopWatch.Restart();
                    int curKey = 1;
                    while (curKey < count)
                    {
                        byte[] result = pbkd2f.GetBytes(32);
                        curKey += 1;
                    }
                    elapsed = stopWatch.Elapsed;
                    success = true;
                }
                else
                {
                    SCrypt scrypt = new SCrypt(
                        int.Parse(SCryptIterationCount),
                        int.Parse(SCryptBlockSize),
                        int.Parse(SCryptThreadCount));

                    byte[] saltUsed = null;
                    stopWatch.Restart();
                    int curKey = 1;
                    while (curKey < count)
                    {
                        byte[] result = scrypt.DeriveBytes(
                            password,
                            salt,
                            out saltUsed);
                        curKey += 1;
                    }
                    elapsed = stopWatch.Elapsed;
                    success = true;
                }
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    TestSettingsButtonText = "Test Settings";
                    View.IsEnabled = true;
                });
            }
            return (new Tuple<bool, TimeSpan>(success, elapsed));
        }

        #endregion

        #region public methods

        public void Accept(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Accept invoked.");

            AppConfig.Instance.Save();
            DLoggerManager.Instance.Config.DefaultEnabled = LoggingEnabled;
            App.Controller.GoBack();
        }

        public void Cancel(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Cancel invoked.");

            AppConfig.DiscardChanges();
            App.Controller.GoBack();
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            //No parameters to set
        }

        public void SetParameters(params KeyValuePair<String, Object>[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                foreach (KeyValuePair<String, Object> curParameter in parameters)
                {
                    SetParameter(curParameter.Key,
                        curParameter.Value);
                }
            }
        }

        public async void OnClosePopup(View item, object parameter)
        {
            if(item is OAuthAuthenticateView)
            {
                if(parameter != null)
                {
                    Dictionary<string, object> parameters = parameter as Dictionary<string, object>;
                    string accessToken = AuthenticationHelpers.CompleteOAuthAutentication(parameters);
                    CloudStorageProviderBase cloudStorageProvider = CloudStorageProviderBase.CreateOAuth<DropboxStorageProvider>(App.AppLogger.Logger, accessToken);
                    CloudProviderResponse<CloudStorageProviderUserBase> getAccountUserResponse = await cloudStorageProvider.GetAccountUser();
                    if(getAccountUserResponse.ResponseValue == CloudProviderResponse<CloudStorageProviderUserBase>.Response.Success)
                    {
                        await CloudProviders.Instance.UpdateProvider(
                            (string)parameters["ProviderID"],
                            getAccountUserResponse.Result.Email,
                            accessToken);
                        NotifyPropertyChanged("ConfiguredCloudProviders");
                    }
                }
            }
            IsValidChanged(IsValid);
        }

        public void OnNavigateTo(View view, object parameter)
        {
            RefreshBindingContext();
            SelectedTabPage = "VAULT";

            _loggingEnabled = DLoggerManager.Instance.Config.DefaultEnabled;
            NotifyPropertyChanged("LoggingEnabled");

            IsValidChanged(IsValid);
        }

        public void OnNavigateFrom(View view, object parameter)
        {

        }

        public void OnGoBack(View view, object parameter)
        {

        }

        #endregion

        #region commands

        public void AddCloudProviderAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "AddCloudProvider invoked.");

            App.Controller.ShowPopup("settings.addcloudprovider");
        }

        public async void ResetCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Reset command invoked.");

            await ((App)App.Current).ConfirmReset(true);
        }

        public async void TestDerivationSettingsCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Test settings command invoked.");

            await Task.Yield();
            int count = 10;
            Tuple<bool, TimeSpan> result = await Task<Tuple<bool, TimeSpan>>.Run(() => { return (TestPasswordDerivationFunction(count)); });
            if (result.Item1)
            {
                await App.Controller.MainPageInstance.DisplayAlert("Password Derivation Test",
                    String.Format("Password derivation took '{0}' to generate '{1}' keys.", result.Item2.ToString(), count),
                    "OK");
            }

        }

        #endregion

    }

}

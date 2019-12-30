using cachy.Config;
using devoctomy.cachy.Framework.Cryptography.OAuth;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Data.Cloud;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Pages
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FirstRun : CarouselPage
	{

        #region public events

        public event EventHandler<FirstRunFinishEventArgs> Finish;

        #endregion

        #region public enums

        public enum FirstRunStage
        {
            None = 0,
            DontCreateFirstVault = 1,
            FullSetup = 2
        }

        #endregion

        #region private objects

        private bool _pbkdf2Enabled = false;
        private bool _sCryptEnabled = true;
        private bool _clipboardObfuscatorDisabled = false;
        private bool _clipboardObfuscatorEnabled = true;
        private bool _autoSaveDisabled = false;
        private bool _autoSaveEnabled = true;
        private Common.SyncMode _syncMode = Common.SyncMode.None;
        private bool _isConnecting = false;
        private ProviderType _cloudStorageProviderType;
        private string _accessID = String.Empty;
        private string _secretKey = String.Empty;
        private AmazonS3Region _region;
        private string _bucketName = String.Empty;
        private string _path = String.Empty;
        private bool _isAuthenticated;
        private CloudProvider _cloudProvider;
        private CloudStorageProviderBase _cloudStorageProvider;
        private CloudStorageProviderUserBase _cloudStorageAccountUser;
        private ContentPage _forcePage;
        private string _vaultName = String.Empty;
        private string _vaultDescription = String.Empty;
        private bool _showMasterPassphrase;
        private string _masterPassphrase = String.Empty;

        private ICommand _providerLogout;

        #endregion

        #region public properties

        public bool PBKDF2Enabled
        {
            get
            {
                return (_pbkdf2Enabled);
            }
            set
            {
                if(_pbkdf2Enabled != value)
                {
                    _pbkdf2Enabled = value;
                    OnPropertyChanged("PBKDF2Enabled");
                }
            }
        }

        public bool SCryptEnabled
        {
            get
            {
                return (_sCryptEnabled);
            }
            set
            {
                if (_sCryptEnabled != value)
                {
                    _sCryptEnabled = value;
                    OnPropertyChanged("SCriptEnabled");
                }
            }
        }

        public bool ClipboardObfuscatorDisabled
        {
            get
            {
                return (_clipboardObfuscatorDisabled);
            }
            set
            {
                if (_clipboardObfuscatorDisabled != value)
                {
                    _clipboardObfuscatorDisabled = value;
                    OnPropertyChanged("ClipboardObfuscatorDisabled");
                }
            }
        }

        public bool ClipboardObfuscatorEnabled
        {
            get
            {
                return (_clipboardObfuscatorEnabled);
            }
            set
            {
                if (_clipboardObfuscatorEnabled != value)
                {
                    _clipboardObfuscatorEnabled = value;
                    OnPropertyChanged("ClipboardObfuscatorEnabled");
                }
            }
        }

        public bool AutoSaveDisabled
        {
            get
            {
                return (_autoSaveDisabled);
            }
            set
            {
                if(_autoSaveDisabled != value)
                {
                    _autoSaveDisabled = value;
                    OnPropertyChanged("AutoSaveDisabled");
                }
            }
        }

        public bool AutoSaveEnabled
        {
            get
            {
                return (_autoSaveEnabled);
            }
            set
            {
                if (_autoSaveEnabled != value)
                {
                    _autoSaveEnabled = value;
                    OnPropertyChanged("AutoSaveEnabled");
                }
            }
        }

        public bool IsConnecting
        {
            get
            {
                return (_isConnecting);
            }
            set
            {
                if(_isConnecting != value)
                {
                    _isConnecting = value;
                    OnPropertyChanged("IsConnecting");
                }
            }
        }

        public ProviderType CloudStorageProviderType
        {
            get
            {
                return (_cloudStorageProviderType);
            }
            set
            {
                if(_cloudStorageProviderType != value)
                {
                    _cloudStorageProviderType = value;
                    OnPropertyChanged("CloudStorageProviderType");
                    SetupCloudProvider();
                }
            }
        }

        public string AccessID
        {
            get
            {
                return (_accessID);
            }
            set
            {
                if(_accessID != value)
                {
                    _accessID = value;
                    OnPropertyChanged("AccessID");
                    OnPropertyChanged("AmazonCredsAreSet");
                }
            }
        }

        public string SecretKey
        {
            get
            {
                return (_secretKey);
            }
            set
            {
                if (_secretKey != value)
                {
                    _secretKey = value;
                    OnPropertyChanged("SecretKey");
                    OnPropertyChanged("AmazonCredsAreSet");
                }
            }
        }

        public AmazonS3Region Region
        {
            get
            {
                return (_region);
            }
            set
            {
                if (_region != value)
                {
                    _region = value;
                    OnPropertyChanged("Region");
                    OnPropertyChanged("AmazonCredsAreSet");
                }
            }
        }

        public string BucketName
        {
            get
            {
                return (_bucketName);
            }
            set
            {
                if (_bucketName != value)
                {
                    _bucketName = value;
                    OnPropertyChanged("BucketName");
                    OnPropertyChanged("AmazonCredsAreSet");
                }
            }
        }

        public string Path
        {
            get
            {
                return (_path);
            }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    OnPropertyChanged("Path");
                    OnPropertyChanged("AmazonCredsAreSet");
                }
            }
        }

        public bool AmazonCredsAreSet
        {
            get
            {
                return (!String.IsNullOrWhiteSpace(AccessID) &&
                    !String.IsNullOrWhiteSpace(SecretKey) &&
                    Region != null &&
                    !String.IsNullOrWhiteSpace(BucketName) &&
                    !String.IsNullOrWhiteSpace(Path));
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return (_isAuthenticated);
            }
            set
            {
                if(_isAuthenticated != value)
                {
                    _isAuthenticated = value;
                    OnPropertyChanged("IsAuthenticated");
                }
            }
        }

        public CloudStorageProviderUserBase CloudStorageAccountUser
        {
            get
            {
                return (_cloudStorageAccountUser);
            }
            set
            {
                if(_cloudStorageAccountUser != value)
                {
                    _cloudStorageAccountUser = value;
                    OnPropertyChanged("CloudStorageAccountUser");
                }
            }
        }

        public string VaultName
        {
            get
            {
                return (_vaultName);
            }
            set
            {
                if(_vaultName != value)
                {
                    _vaultName = value;
                    OnPropertyChanged("VaultName");
                    OnPropertyChanged("VaultNameIsSet");
                }
            }
        }

        public bool VaultNameIsSet
        {
            get
            {
                return (!String.IsNullOrEmpty(VaultName));
            }
        }

        public string VaultDescription
        {
            get
            {
                return (_vaultDescription);
            }
            set
            {
                if (_vaultDescription != value)
                {
                    _vaultDescription = value;
                    OnPropertyChanged("VaultDescription");
                }
            }
        }

        public bool ShowMasterPassphrase
        {
            get
            {
                return (_showMasterPassphrase);
            }
            set
            {
                if(_showMasterPassphrase != value)
                {
                    _showMasterPassphrase = value;
                    OnPropertyChanged("ShowMasterPassphrase");
                }
            }
        }

        public string MasterPassphrase
        {
            get
            {
                return (_masterPassphrase);
            }
            set
            {
                if(_masterPassphrase != value)
                {
                    _masterPassphrase = value;
                    OnPropertyChanged("MasterPassphrase");
                    OnPropertyChanged("MasterPassphraseIsSet");
                }
            }
        }

        public bool MasterPassphraseIsSet
        {
            get
            {
                return (!String.IsNullOrEmpty(MasterPassphrase));
            }
        }

        public string LoginMessage
        {
            get
            {
                return (String.Format("You are now logged into '{0}' with the following account,", CloudStorageProviderType == null ? String.Empty : CloudStorageProviderType.Name));
            }
        }

        public ICommand ProviderLogout
        {
            get
            {
                return (_providerLogout);
            }
        }

        #endregion

        #region constructor / destructor

        public FirstRun()
        {
            _providerLogout = new Command(new Action<object>(ProviderLogoutAction));
            InitializeComponent();
        }

        #endregion

        #region private methods

        private async void SetupCloudProvider()
        {
            if (CloudStorageProviderType == null) return;

            switch(CloudStorageProviderType.AuthType)
            {
                case ProviderType.AuthenticationType.OAuth:
                    {
                        try
                        {
                            IsConnecting = true;
                            _forcePage = CloudProviderPage;
                            devoctomy.cachy.Framework.Native.Native.WebUtility.ClearInAppBrowserCache();

                            Dictionary<string, object> beginParams = await AuthenticationHelpers.BeginOAuthAuthentication(CloudStorageProviderType.Name);
                            string sessionID = (string)beginParams["SessionID"];
                            Uri authoriseURI = null;
                            switch (CloudStorageProviderType.Name)
                            {
                                case "Dropbox":
                                    {
                                        string redirectURI = Uri.EscapeDataString("https://cachywebfunctions20190202044830.azurewebsites.net/api/DropboxOAuthRedirect");
                                        string uri = String.Format(
                                            "https://www.dropbox.com/oauth2/authorize?response_type=code&client_id={0}&redirect_uri={1}&state={2}",
                                            "bllblee6oqr9q22",
                                            redirectURI,
                                            sessionID);
                                        authoriseURI = new Uri(uri);
                                        break;
                                    }
                                case "OneDrive":
                                    {
                                        string redirectURI = Uri.EscapeDataString("https://cachywebfunctions20190202044830.azurewebsites.net/api/OneDriveOAuthRedirect");
                                        string uri = String.Format(
                                            "https://login.microsoftonline.com/consumers/oauth2/v2.0/authorize?response_type=code&client_id={0}&scope={1}&redirect_uri={2}&state={3}",
                                            "28cb64d4-f3a2-42db-8889-b760e2163496",
                                            Uri.EscapeDataString("https://graph.microsoft.com/Files.ReadWrite.AppFolder,https://graph.microsoft.com/User.Read"),
                                            redirectURI,
                                            sessionID);
                                        authoriseURI = new Uri(uri);
                                        break;
                                    }
                                default:
                                    {
                                        //Uknown OAuth cloud provider type!
                                        return;
                                    }
                            }
                            OAuthWebView.Source = authoriseURI;
                            OAuthWebView.IsVisible = true;
                            string continueResponseString = await AuthenticationHelpers.ContinueOAuthAuthentication(CloudStorageProviderType.Name, sessionID);

                            Dictionary<string, object> continueParameters = new Dictionary<string, object>();
                            continueParameters.Add("ProviderID", CloudStorageProviderType.Name);
                            continueParameters.Add("AuthResponse", continueResponseString);
                            continueParameters.Add("RSA", beginParams["RSA"]);

                            string accessToken = AuthenticationHelpers.CompleteOAuthAutentication(continueParameters);

                            Dictionary<string, string> createParams = new Dictionary<string, string>();
                            createParams.Add("AuthType", "OAuth");
                            createParams.Add("ProviderKey", (string)continueParameters["ProviderID"]);
                            createParams.Add("AccessToken", accessToken);
                            _cloudStorageProvider = CloudStorageProviderBase.Create(
                                App.AppLogger.Logger,
                                createParams);

                            CloudProviderResponse<CloudStorageProviderUserBase> getAccountUserResponse = await _cloudStorageProvider.GetAccountUser();
                            if (getAccountUserResponse.ResponseValue == CloudProviderResponse<CloudStorageProviderUserBase>.Response.Success)
                            {
                                CloudStorageAccountUser = getAccountUserResponse.Result;

                                switch (CloudStorageProviderType.AuthType)
                                {
                                    case ProviderType.AuthenticationType.OAuth:
                                        {
                                            _cloudProvider = CloudProviders.Instance.AddProvider(
                                                ProviderType.AuthenticationType.OAuth,
                                                _cloudStorageProvider.TypeName,
                                                CloudStorageAccountUser.Email,
                                                accessToken,
                                                true);
                                            break;
                                        }
                                }

                                IsAuthenticated = true;
                            }
                            else
                            {
                                CloudStorageAccountUser = null;
                                IsAuthenticated = false;
                            }
                        }
                        catch(Exception ex)
                        {
                            //log exception here
                        }
                        finally
                        {
                            IsConnecting = false;
                            _forcePage = null;
                            OAuthWebView.IsVisible = false;
                        }

                        break;
                    }
                case ProviderType.AuthenticationType.Amazon:
                    {
                        AmazonCredInput.IsVisible = true;
                        break;
                    }
            }
        }

        #endregion

        #region commands

        private void ProviderLogoutAction(object parameter)
        {
            CloudStorageAccountUser = null;
            IsAuthenticated = false;
        }

        #endregion

        #region object events

        private void WelcomeSkipButton_Clicked(object sender, EventArgs e)
        {
            Finish?.Invoke(this, new FirstRunFinishEventArgs());
        }

        private void CreateFirstVaultSkipButton_Clicked(object sender, EventArgs e)
        {
            Finish?.Invoke(this, new FirstRunFinishEventArgs(
                PBKDF2Enabled ? Common.PasswordDerivationMode.PBKDF2 : Common.PasswordDerivationMode.SCrypt,
                ClipboardObfuscatorEnabled,
                AutoSaveEnabled));
        }

        private void FinishButton_Clicked(object sender, EventArgs e)
        {
            Finish?.Invoke(this, new FirstRunFinishEventArgs(
                PBKDF2Enabled ? Common.PasswordDerivationMode.PBKDF2 : Common.PasswordDerivationMode.SCrypt,
                ClipboardObfuscatorEnabled,
                AutoSaveEnabled,
                _syncMode,
                _cloudProvider,
                CloudStorageProviderType,
                CloudStorageAccountUser,
                VaultName,
                VaultDescription,
                MasterPassphrase));
        }

        #endregion

        #region base class events

        private void Root_CurrentPageChanged(object sender, EventArgs e)
        {
            //keep on forced page
            if (_forcePage != null && CurrentPage != _forcePage)
            {
                CurrentPage = _forcePage;
                return;
            }

            //apply page specific resets
            if (CurrentPage == SyncModePage)
            {
                CloudStorageAccountUser = null;
                IsAuthenticated = false;
                OAuthWebView.IsVisible = false;
                AmazonCredInput.IsVisible = false;
                if(CloudStorageProviderType != null)
                {
                    CloudStorageProviderType.IsSelected = false;
                    CloudStorageProviderType = null;
                    CloudProviderList.DeselectAll();
                }
                AccessID = String.Empty;
                SecretKey = String.Empty;
                Region = null;
                BucketName = String.Empty;
                Path = "cachy";
                _forcePage = SyncModePage;
            }
            else if(CurrentPage == MasterPassphrasePage)
            {
                if(String.IsNullOrEmpty(VaultName))
                {
                    CurrentPage = NameDescriptionPage;
                    return;
                }
            }
            else if(CurrentPage == FinishPage)
            {
                if(String.IsNullOrEmpty(MasterPassphrase))
                {
                    CurrentPage = MasterPassphrasePage;
                    return;
                }
            }

            //Put logic below to stop pages being navigated to when we shouldn't
            if(CurrentPage == CloudProviderPage && _syncMode != Common.SyncMode.CloudProvider)
            {
                _syncMode = Common.SyncMode.None;
                CurrentPage = SyncModePage;
                _forcePage = SyncModePage;
            }
        }


        private void LocalSyncModeButton_Clicked(object sender, EventArgs e)
        {
            _forcePage = null;
            _syncMode = Common.SyncMode.LocalOnly;
            CurrentPage = NameDescriptionPage;
        }

        private void CloudSyncModeButton_Clicked(object sender, EventArgs e)
        {
            _forcePage = null;
            _syncMode = Common.SyncMode.CloudProvider;
            CurrentPage = CloudProviderPage;
        }

        private void PositiveAffirmationsButton_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://www.google.co.uk/search?q=positive+affirmations"));
        }

        private async void AcceptAmazonCredsButton_Clicked(object sender, EventArgs e)
        {
            AmazonS3Config config = new AmazonS3Config(
                AccessID,
                SecretKey,
                Region.Name,
                BucketName,
                Path);
            Dictionary<string, string> createParams = config.ToDictionary();
            createParams.Add("ProviderKey", CloudStorageProviderType.Name);
            _cloudStorageProvider = CloudStorageProviderBase.Create(
                App.AppLogger.Logger,
                createParams);
            _cloudProvider = CloudProviders.Instance.AddProvider(
                ProviderType.AuthenticationType.Amazon,
                _cloudStorageProvider.TypeName,
                config.AccessID,
                config.ToString(),
                true);
            CloudProviderResponse<CloudStorageProviderUserBase> getAccountUserResponse = await _cloudStorageProvider.GetAccountUser();
            if (getAccountUserResponse.ResponseValue == CloudProviderResponse<CloudStorageProviderUserBase>.Response.Success)
            {
                CloudStorageAccountUser = getAccountUserResponse.Result;
                IsAuthenticated = true;
                AmazonCredInput.IsVisible = false;
            }
        }

        #endregion

    }

}
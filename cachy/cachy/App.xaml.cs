using Xamarin.Forms;
using cachy.Views;
using Xamarin.Forms.Xaml;
using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Data;
using System.Collections.Generic;
using System;
using cachy.Pages;
using cachy.Navigation.BurgerMenu;
using System.Threading.Tasks;
using cachy.Fonts;
using cachy.Config;
using devoctomy.cachy.Framework.Data.Cloud;
using System.Linq;
using cachy.Data;
using System.Security.Permissions;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace cachy
{

    public partial class App : Application
    {

        #region private objects

        private static AppController<BurgerMenuHostPage> _appController;
        private Func<Boolean> _checkPermissions;
        private Action _requestPermissions;
        private Action _permissionsChanged;
        private Boolean _performedStartup;
        private Action<Boolean> _androidSetSoftInputMode;
        private static System.Timers.Timer _restartClipboardObfuscatorTimer;
        private static bool _clipboardObfuscatorIsInitialised;
        private static bool _runningClipboardObfuscator;
        private static bool _noVaultsOnStartup = true;
        private static VaultIndex _openedVault;

        #endregion

        #region public properties

        public static DLoggerManager AppLogger
        {
            get
            {
                return (DLoggerManager.DefaultInstance("cachy"));
            }
        }

        public static AppController<BurgerMenuHostPage> Controller
        {
            get
            {
                return (_appController);
            }
        }

        public Func<Boolean> CheckPermissions
        {
            get
            {
                return (_checkPermissions);
            }
        }

        public Action RequestPermissions
        {
            get
            {
                return (_requestPermissions);
            }
        }

        public Action PermissionsChanged
        {
            get
            {
                return (_permissionsChanged);
            }
        }

        public static bool NoVaultsOnStartup
        {
            get
            {
                return (_noVaultsOnStartup);
            }
        }

        public VaultIndex OpenedVault
        {
            get
            {
                return (_openedVault);
            }
            set
            {
                if(_openedVault != value)
                {
                    _openedVault = value;
                }
            }
        }

        #endregion

        #region public methods

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public App(Func<Boolean> checkPermissions,
            Action requestPermissions,
            Action<Boolean> androidSetSoftInputMode)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            InitializeComponent();

            _androidSetSoftInputMode = androidSetSoftInputMode;
            _checkPermissions = checkPermissions;
            _requestPermissions = requestPermissions;
            _permissionsChanged = new Action(OnPermissionsChanged);
            _restartClipboardObfuscatorTimer = new System.Timers.Timer();
            _restartClipboardObfuscatorTimer.Elapsed += _restartClipboardObfuscatorTimer_Elapsed;

            //string currentTheme = "Dark";
            //Application.Current.Resources["Accent"] = Application.Current.Resources[String.Format("{0}.Accent", currentTheme)];
            //Application.Current.Resources["TextColour"] = Application.Current.Resources[String.Format("{0}.TextColour", currentTheme)];
            //Application.Current.Resources["CommentTextColour"] = Application.Current.Resources[String.Format("{0}.CommentTextColour", currentTheme)];
            //Application.Current.Resources["BackgroundColour"] = Application.Current.Resources[String.Format("{0}.BackgroundColour", currentTheme)];
            //Application.Current.Resources["ItemBorderColour"] = Application.Current.Resources[String.Format("{0}.ItemBorderColour", currentTheme)];
            //Application.Current.Resources["AltGlyphButtonBackgroundColour"] = Application.Current.Resources[String.Format("{0}.AltGlyphButtonBackgroundColour", currentTheme)];

            //Let's set the accent colour to the one in our theme resource
            var accentColorProp = typeof(Color).GetProperty(nameof(Color.Accent), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var xamarinAccentColor = (Color)Application.Current.Resources["Accent"];
            accentColorProp.SetValue(null, xamarinAccentColor, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static, null, null, System.Globalization.CultureInfo.CurrentCulture);

            MainPage = new LandingPage();
        }

        public void Start()
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Performing application startup.");
            _appController.Start(new KeyValuePair<String, Object>("Vaults", VaultIndexFile.Instance.Indexes));
        }

        public void NavigateToVaultsList()
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Navigating to vaults list.");
            _appController.NavigateTo("vaultlist",
                new KeyValuePair<String, Object>("Vaults", VaultIndexFile.Instance.Indexes));
        }

        public void PerformStartup()
        {
            if (!_checkPermissions())
            {
                MainPage = new PermissionsRequests();
            }
            else
            {
                DisplayFirstRunIfApplicable();
            }
        }

        public void StartupProblems()
        {
            MainPage = new StartupHelpPage();
        }

        public void DisplayLandingPage()
        {
            MainPage = new LandingPage();
        }

        public async Task<bool> ConfirmReset(
            bool suggestAppResart,
            Page page = null)
        {
            if(page == null)
            {
                page = App.Controller.MainPageInstance;
            }
            String appDataPath = String.Empty;
            devoctomy.DFramework.Core.IO.Directory.ResolvePath("{AppData}", out appDataPath);
            if (await page.DisplayAlert("Reset",
                String.Format("Are you sure you want to reset cachy? The app configuration will be lost, including stored OAuth tokens.  No files will be deleted, but your currently configured vaults will need to be re-configured manually. By selecting 'Yes' you assume all responsibility for this process.", appDataPath),
                "Yes I assume all responsibility",
                "No"))
            {
                ((App)App.Current).Reset();
                await page.DisplayAlert("Reset",
                    String.Format("Successfully reset cachy.{0}", suggestAppResart ? " You must now close the application." : String.Empty),
                    "OK");
                return (true);
            }
            return (false);
        }

        public void AndroidSetSoftInputMode(bool adjustResize)
        {
            _androidSetSoftInputMode?.Invoke(adjustResize);
        }

        public async void ProcessArguments(object parameter)
        {
            string commandLine = parameter as string;
            if (!String.IsNullOrEmpty(commandLine))
            {
                if (commandLine.ToLower().EndsWith(".vault"))
                {
                    if (await devoctomy.cachy.Framework.Native.Native.FileHandler.Exists(commandLine))
                    {
                        VaultIndex index = VaultIndex.Prepare(commandLine);
                        App.Controller.ShowPopup("vaultlist.unlockvault",
                            new KeyValuePair<String, Object>("VaultIndex", index));
                    }
                }
            }
        }

        public object GetProviderValue(
            string id,
            string key)
        {
            IEnumerable<CloudProvider> providers = CloudProviders.Instance.Providers.Where(cp => cp.ID == id);
            if (providers.Any())
            {
                CloudProvider provider = providers.First();
                Dictionary<string, object> dictionary = provider.ToDictionary();
                return (dictionary[key]);
            }
            else
            {
                return (String.Empty);
            }
        }

        public string GetCredential(string key)
        {
            Dictionary<string, string> credential = devoctomy.cachy.Framework.Native.Native.PasswordVault.GetCredential(key);
            if (credential != null)
            {
                return (credential["Password"]);
            }
            else
            {
                return (null);
            }
        }

        public static void SetClipboardObfuscatorInitialised()
        {
            _clipboardObfuscatorIsInitialised = true;
        }

        public static void StartClipboardObfuscator(bool start)
        {
            if(start)
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information,
                    "Starting clipboard obfuscator.");
                _runningClipboardObfuscator = true;
            }
            else
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information,
                    "Resuming clipboard obfuscator.");
            }
            if (_runningClipboardObfuscator)
            {
                ClipboardObfuscator.Instance.Start();
            }
        }

        public static void StopClipboardObfuscator(
            bool autoRestart,
            TimeSpan interval)
        {
            if(autoRestart)
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information,
                    "Stopping clipboard obfuscator, for '{0}'.", interval.ToString());
            }
            else
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information,
                    "Stopping clipboard obfuscator.");
            }
            _runningClipboardObfuscator = false;
            ClipboardObfuscator.Instance.Stop();
            if (autoRestart)
            {
                _restartClipboardObfuscatorTimer.Interval = interval.TotalMilliseconds;
                _restartClipboardObfuscatorTimer.Start();
            }
        }

        public void AndroidLostFocus()
        {
            if(_clipboardObfuscatorIsInitialised)
            {
                StopClipboardObfuscator(
                    false,
                    new TimeSpan(0));
            }
        }

        public void AndroidGotFocus()
        {
            if (_clipboardObfuscatorIsInitialised && AppConfig.Instance.EnableClipboardObfuscator)
            {
                App.StartClipboardObfuscator(true);
            }
        }

        public void Reset()
        {
            String appDataPath = String.Empty;
            devoctomy.DFramework.Core.IO.Directory.ResolvePath("{AppData}", out appDataPath);
            if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;

            if (System.IO.Directory.Exists(appDataPath))
            {
                try
                {
                    String localConfigPath = String.Format("{0}{1}", appDataPath, "Config");
                    localConfigPath += DLoggerManager.PathDelimiter;

                    string cachyConfig = String.Format("{0}{1}", localConfigPath, "cachy.config.json");
                    string cloudProviders = String.Format("{0}{1}", localConfigPath, "cloudproviders.json");
                    string index = String.Format("{0}{1}", localConfigPath, "index.json");
                    string loggerConfig = String.Format("{0}{1}", localConfigPath, "logger.config.json");

                    DateTime now = DateTime.Now;
                    string date = DateTime.Now.ToString("ddMMyyyy");
                    string time = (now - now.Date).TotalSeconds.ToString();

                    if (System.IO.File.Exists(cachyConfig)) System.IO.File.Move(cachyConfig, String.Format("{0}{1}_{2}{3}", cachyConfig, date, time, ".bak"));
                    if (System.IO.File.Exists(cloudProviders)) System.IO.File.Move(cloudProviders, String.Format("{0}{1}_{2}{3}", cloudProviders, date, time, ".bak"));
                    if (System.IO.File.Exists(index)) System.IO.File.Move(index, String.Format("{0}{1}_{2}{3}", index, date, time, ".bak"));
                    if (System.IO.File.Exists(loggerConfig)) System.IO.File.Move(loggerConfig, String.Format("{0}{1}_{2}{3}", loggerConfig, date, time, ".bak"));
                }
                catch (Exception)
                {

                }
            }

            devoctomy.cachy.Framework.Native.Native.PasswordVault.RemoveAll();
        }

        #endregion

        #region private methods

        private void DisplayFirstRunIfApplicable()
        {
            CreateAppFolders();
            AppConfig.Initialise();

            if (AppConfig.Instance.IsFirstRun)
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information,
                    "Starting first run wizard.");

                AndroidSetSoftInputMode(true);
                FirstRun firstRun = new FirstRun();
                firstRun.Finish += FirstRun_Finish;
                MainPage = firstRun;
            }
            else
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information,
                    "Initialising clipboard obfuscator (it isn't started at this point).");

                _appController = new AppController<BurgerMenuHostPage>(this,
                    SetupMainPage);
                try
                {
                    App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "cachy App:PerformStartup");

                    CloudStorageSyncManager.Initialise(
                        App.AppLogger.Logger,
                        GetProviderValue,
                        GetCredential);
                    Start();

                    _performedStartup = true;
                }
                catch (UnauthorizedAccessException) //uaex)
                {
                    //we don't have storage permission
                    throw;
                }
            }
        }

        private void CreateAppFolders()
        {
            String appDataPath = String.Empty;
            devoctomy.DFramework.Core.IO.Directory.ResolvePath("{AppData}", out appDataPath);
            if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
            String localVaultsPath = String.Format("{0}{1}", appDataPath, "LocalVaults");
            String localConfigPath = String.Format("{0}{1}", appDataPath, "Config");
            String localLogPath = String.Format("{0}{1}", appDataPath, "Log");
            String vaultExportsPath = String.Format("{0}{1}", appDataPath, "Exports");

            System.IO.Directory.CreateDirectory(localVaultsPath);
            System.IO.Directory.CreateDirectory(localConfigPath);
            System.IO.Directory.CreateDirectory(localLogPath);
            System.IO.Directory.CreateDirectory(vaultExportsPath);
        }

        private BurgerMenuHostPage SetupMainPage()
        {
            MainPage mainPage = new MainPage();
           
            //Vault list
            BurgerMenuViewItem vaultListItem = new BurgerMenuViewItem("vaultlist", "My Vaults", CachyFont.Glyph.None, "My Vaults", typeof(VaultListView), false);
            vaultListItem.AddChildCommandItem("createvault", "Create Vault", CachyFont.Glyph.New, "CreateVault");
            vaultListItem.AddChildCommandItem("addexistingvaultcmd", "Add Existing Vault", CachyFont.Glyph.Add_New, "AddExistingVault");
            vaultListItem.AddChildCommandItem("sync", "Synchronise", CachyFont.Glyph.Cloud_Recycle, "Synchronise");
            vaultListItem.AddChildPopupItem("unlockvault", typeof(UnlockView));
            vaultListItem.AddChildPopupItem("addexistingvaultpopup", typeof(AddExistingVaultView));
            vaultListItem.AddChildPopupItem("cloudfiles", typeof(CloudProviderFileSelectView));
            vaultListItem.AddChildPopupItem("addcloudprovider", typeof(AddCloudProviderView));
            vaultListItem.AddChildPopupItem("vaultinfo", typeof(VaultInfoView));

            //Create vault
            BurgerMenuViewItem createVaultItem = new BurgerMenuViewItem("createvault", "Create Vault", CachyFont.Glyph.None, "Create Vault", typeof(CreateNewVaultView), true);
            createVaultItem.AddChildCommandItem("accept", "Accept", CachyFont.Glyph.Check, "Accept");
            createVaultItem.AddChildCommandItem("cancel", "Cancel", CachyFont.Glyph.In, "Cancel");
            createVaultItem.AddChildPopupItem("addcloudprovider", typeof(AddCloudProviderView));
            createVaultItem.AddChildPopupItem("oauth", typeof(OAuthAuthenticateView));
            createVaultItem.AddChildPopupItem("s3setup", typeof(AmazonS3SetupView));

            //Vault
            BurgerMenuViewItem vaultItem = new BurgerMenuViewItem("vault", "VAULT NAME GOES HERE", typeof(VaultView), false);
            vaultItem.AddChildCommandItem("lockvault", "Lock Vault", CachyFont.Glyph.Lock, "Lock");
            vaultItem.AddChildCommandItem("createcred", "Create Credential", CachyFont.Glyph.New, "CreateCredential");
            vaultItem.AddChildCommandItem("importcsv", "Import CSV File", CachyFont.Glyph.Data_Import, "ImportCSV");
            vaultItem.AddChildCommandItem("exportcsv", "Export CSV File", CachyFont.Glyph.Export, "ExportCSV");
            vaultItem.AddChildCommandItem("report", "Vault Report", CachyFont.Glyph.New_Report, "Report");
            vaultItem.AddChildCommandItem("audit", "View Vault Audit", CachyFont.Glyph.Audit, "Audit");
            vaultItem.AddChildPopupItem("vaultreport", typeof(VaultReportView));
            vaultItem.AddChildPopupItem("vaultaudit", typeof(VaultAuditView));
            vaultItem.AddChildPopupItem("credaudit", typeof(CredentialAuditView));
            vaultItem.AddChildPopupItem("importsource", typeof(SelectImportSourceView));
            vaultItem.AddChildPopupItem("importmap", typeof(ImportMappingView));

            //Create credential
            BurgerMenuViewItem createCredItem = new BurgerMenuViewItem("crededit", "Create Credential", CachyFont.Glyph.None, "Create Credential", typeof(CreateCredentialView), false);
            createCredItem.AddChildCommandItem("changeglyphicon", "Change Icon", CachyFont.Glyph.Card_Image, "ChangeGlyphIcon");
            createCredItem.AddChildCommandItem("changeglyphcolour", "Change Colour", CachyFont.Glyph.Palette_01, "ChangeGlyphColour");
            createCredItem.AddChildCommandItem("accept", "Accept", CachyFont.Glyph.Check, "Accept");
            createCredItem.AddChildCommandItem("cancel", "Cancel", CachyFont.Glyph.In, "Cancel");
            createCredItem.AddChildPopupItem("selectglyph", typeof(GlyphSelectView));
            createCredItem.AddChildPopupItem("selectcolour", typeof(ColourSelectView));
            createCredItem.AddChildPopupItem("genpass", typeof(GeneratePasswordView));

            //Add root items
            List<BurgerMenuViewItem> rootItems = new List<BurgerMenuViewItem>();
            rootItems.Add(vaultListItem);
            rootItems.Add(createVaultItem);
            rootItems.Add(vaultItem);
            rootItems.Add(createCredItem);

            BurgerMenuViewItem settingsItem = new BurgerMenuViewItem("settings", "Settings", CachyFont.Glyph.Settings_02, "Settings", typeof(SettingsView), false);
            settingsItem.AddChildCommandItem("accept", "Accept", CachyFont.Glyph.Check, "Accept");
            settingsItem.AddChildCommandItem("cancel", "Cancel", CachyFont.Glyph.In, "Cancel");
            settingsItem.AddChildPopupItem("addcloudprovider", typeof(AddCloudProviderView));
            settingsItem.AddChildPopupItem("oauth", typeof(OAuthAuthenticateView));
            settingsItem.AddChildPopupItem("s3setup", typeof(AmazonS3SetupView));

            BurgerMenuViewItem aboutItem = new BurgerMenuViewItem("about", "About", CachyFont.Glyph.Help, "About cachy", typeof(AboutView), false);
            aboutItem.AddChildCommandItem("changelog", "Changes", CachyFont.Glyph.Audit, "Changes");
            aboutItem.AddChildCommandItem("acks", "Acknowledgements", CachyFont.Glyph.Love_01, "Acknowledgements");
            aboutItem.AddChildCommandItem("cancel", "Cancel", CachyFont.Glyph.In, "Cancel");
            aboutItem.AddChildPopupItem("changes", typeof(ChangeLogView));
            aboutItem.AddChildPopupItem("acknowledgements", typeof(AcknowledgementsView));

            List<BurgerMenuViewItem> commonItems = new List<BurgerMenuViewItem>();
            commonItems.Add(settingsItem);
            commonItems.Add(aboutItem);

            ((BurgerMenuHostPage)mainPage).Setup(rootItems,
                "vaultlist",
                commonItems);

            return (mainPage);
        }

        private void OnPermissionsChanged()
        {
            if (CheckPermissions())
            {             
                DisplayFirstRunIfApplicable();
            }
        }

        #endregion

        #region base class overrides

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
            if (_performedStartup)
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "cachy App:OnSleep");
                _appController.Sleep();
            }
        }

        protected override void OnResume()
        {
            if (_performedStartup)
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "cachy App:OnResume");
                _appController.Resume();
            }
        }

        #endregion

        #region object events

        private async void FirstRun_Finish(object sender, FirstRunFinishEventArgs e)
        {
            AndroidSetSoftInputMode(false);
            CreateAppFolders();
            e.ApplySettings();
            _noVaultsOnStartup = VaultIndexFile.Instance.Indexes.Count == 0;
            await e.CreateVault();
            PerformStartup();
        }

        private void _restartClipboardObfuscatorTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _restartClipboardObfuscatorTimer.Stop();
            Device.BeginInvokeOnMainThread(() =>
            {
                ClipboardObfuscator.Instance.Start();
            });
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception,
                e.Exception.ToString());
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception,
                e.ExceptionObject.ToString());
        }

        #endregion

    }

}
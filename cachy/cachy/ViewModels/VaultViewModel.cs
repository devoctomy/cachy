using devoctomy.cachy.Framework.Data;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.Generic;
using devoctomy.cachy.Framework.Serialisers.AESEncrypted;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq;
using cachy.Navigation.BurgerMenu;
using cachy.Config;
using Plugin.FilePicker.Abstractions;
using Plugin.FilePicker;
using devoctomy.cachy.Framework.Data.Importers;
using cachy.Data;
using devoctomy.cachy.Framework.Data.Exporters;
using System.IO;
using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Cryptography.Random;

namespace cachy.ViewModels
{

    public class VaultViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private constants

        private const string CREDENTIAL_SEARCH_DEFAULTTEXT = "Search...";

        #endregion

        #region private objects

        private VaultView _view;
        private VaultIndex _vaultIndex;
        private BurgerMenuViewItem _viewItem;
        private String _masterPassphrase = String.Empty;
        private Vault _vault;
        private String _filterString = String.Empty;
        private bool _searching;
        private Credential _selectedCredential;
        private System.Timers.Timer _autoLockTimer;
        private System.Timers.Timer _searchRefresh;
        private DateTime? _idleSince;
        private string _sortField = "Name";
        private bool _ascending = true;
        private static bool _showCredentialListTip = true;

        private ICommand _duplicateCredentialCommand;
        private ICommand _editCredentialCommand;
        private ICommand _removeCredentialCommand;
        private ICommand _credentialSelectedCommand;
        private ICommand _viewCredentialAuditLogCommand;
        private ICommand _searchTextChangedCommand;
        private ICommand _searchCompletedCommand;
        private ICommand _clipboardFieldCopyCommand;
        private ICommand _clipboardFieldRevealCommand;
        private ICommand _openInBrowserCommand;
        private ICommand _closeCredentialListTipCommand;

        #endregion

        #region public properties

        public VaultIndex VaultIndex
        {
            get
            {
                return (_vaultIndex);
            }
        }

        public String MasterPassphrase
        {
            get
            {
                return (_masterPassphrase);
            }
        }

        public Vault Vault
        {
            get
            {
                return (_vault);
            }
            private set
            {
                if(_vault != value)
                {
                    if(_vault != null)
                    {
                        _vault.PropertyChanged -= _vault_PropertyChanged;
                    }
                    _vault = value;
                    if(_vault != null)
                    {
                        _vault.PropertyChanged += _vault_PropertyChanged;
                    }
                    NotifyPropertyChanged("ShowCredentialListTip");
                }
            }
        }

        public IEnumerable<Credential> FilteredCredentials
        {
            get
            {
                ClearSelectedCredential(true);
                if (_searching)
                {
                    IEnumerable<Credential> results = null;
                    if (Vault != null)
                    {
                        List<IEnumerable<Credential>> allFiltered = new List<IEnumerable<Credential>>();
                        string[] filterStringParts = _filterString.Split(' ');
                        foreach (string curFilterString in filterStringParts)
                        {
                            IEnumerable<Credential> searchResults = null;
                            if (curFilterString.StartsWith("#"))
                            {
                                searchResults = Vault.Credentials.Where(cred => cred.Tags.Contains(curFilterString.ToLower().TrimStart('#')));
                            }
                            else if(curFilterString.StartsWith("@"))
                            {
                                string[] advFilterParts = curFilterString.Split(':');
                                switch (advFilterParts[0])
                                {
                                    case "@reused":
                                        {
                                            searchResults = Vault.Credentials.Where(cred => cred.IsReused());
                                            break;
                                        }
                                    case "@known":
                                        {
                                            searchResults = Vault.Credentials.Where(cred => cred.IsKnown());
                                            break;
                                        }
                                    case "@weak":
                                        {
                                            searchResults = Vault.Credentials.Where(cred => cred.IsWeak());
                                            break;
                                        }
                                    case "@old":
                                        {
                                            try
                                            {
                                                TimeSpan age = new TimeSpan(180, 0, 0, 0);
                                                if (advFilterParts.Length == 2)
                                                {
                                                    age = new TimeSpan(int.Parse(advFilterParts[1]), 0, 0, 0);
                                                }
                                                searchResults = Vault.Credentials.Where(cred => cred.IsOlderThan(age));
                                            }
                                            catch(Exception)
                                            {
                                                //Log error
                                            }
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                searchResults = Vault.Credentials.Where(cred => cred.Name.ToLower().Contains(_filterString.ToLower()) ||
                                    cred.Description.ToLower().Contains(_filterString.ToLower()));
                            }
                            if (searchResults != null)
                            {
                                allFiltered.Add(searchResults);
                            }
                        }
                        results = ConcatenateEnumerables<Credential>(allFiltered.ToArray());

                        System.Reflection.PropertyInfo prop = typeof(Credential).GetProperty(SortField);
                        if (Ascending)
                        {
                            results = results.OrderBy(cred => prop.GetValue(cred, null));
                        }
                        else
                        {
                            results = results.OrderByDescending(cred => prop.GetValue(cred, null));
                        }
                    }
                    return (results);
                }
                else
                {
                    IEnumerable<Credential> results = null;
                    if(Vault != null)
                    {
                        System.Reflection.PropertyInfo prop = typeof(Credential).GetProperty(SortField);
                        if (Ascending)
                        {
                            results = Vault.Credentials.OrderBy(cred => prop.GetValue(cred, null));
                        }
                        else
                        {
                            results = Vault.Credentials.OrderByDescending(cred => prop.GetValue(cred, null));
                        }
                    }
                    return (results);
                }
            }
        }

        public bool ShowPasswordAge
        {
            get
            {
                return (AppConfig.Instance.ShowPasswordAge);
            }
        }

        public string SortField
        {
            get
            {
                return (_sortField);
            }
            set
            {
                if(_sortField != value)
                {
                    _sortField = value;
                    NotifyPropertyChanged("SortField");
                    NotifyPropertyChanged("FilteredCredentials");
                }
            }
        }

        public bool Ascending
        {
            get
            {
                return (_ascending);
            }
            set
            {
                if(_ascending != value)
                {
                    _ascending = value;
                    NotifyPropertyChanged("Ascending");
                    NotifyPropertyChanged("FilteredCredentials");
                }
            }
        }

        public List<string> SortFields
        {
            get
            {
                string[] sortFields = new string[]
                {
                    "Name",
                    "Description",
                    "Website",
                    "CreatedAt",
                    "LastModifiedAt",
                    "PasswordLastModifiedAt",
                    "Notes",
                    "Username",
                };
                return (new List<string>(sortFields));
            }
        }

        public bool ShowCredentialListTip
        {
            get
            {
                return (Vault != null && !Vault.IsEmpty && _showCredentialListTip);
            }
            set
            {
                if (_showCredentialListTip != value)
                {
                    _showCredentialListTip = value;
                    NotifyPropertyChanged("ShowCredentialListTip");
                }
            }
        }

        public ICommand DuplicateCredentialCommand
        {
            get
            {
                return (_duplicateCredentialCommand);
            }
        }

        public ICommand EditCredentialCommand
        {
            get
            {
                return (_editCredentialCommand);
            }
        }

        public ICommand RemoveCredentialCommand
        {
            get
            {
                return (_removeCredentialCommand);
            }
        }

        public ICommand CredentialSelectedCommand
        {
            get
            {
                return (_credentialSelectedCommand);
            }
        }

        public ICommand ViewCredentialAuditLogCommand
        {
            get
            {
                return (_viewCredentialAuditLogCommand);
            }
        }

        public ICommand SearchTextChangedCommand
        {
            get
            {
                return (_searchTextChangedCommand);
            }
        }

        public ICommand SearchCompletedCommand
        {
            get
            {
                return (_searchCompletedCommand);
            }
        }

        public ICommand ClipboardFieldCopyCommand
        {
            get
            {
                return (_clipboardFieldCopyCommand);
            }
        }

        public ICommand ClipboardFieldRevealCommand
        {
            get
            {
                return (_clipboardFieldRevealCommand);
            }
        }

        public ICommand OpenInBrowserCommand
        {
            get
            {
                return (_openInBrowserCommand);
            }
        }

        public ICommand CloseCredentialListTipCommand
        {
            get
            {
                return (_closeCredentialListTipCommand);
            }
        }

        public VaultView View
        {
            get
            {
                return (_view);
            }
        }

        #endregion

        #region constructor / destructor

        public VaultViewModel(VaultView view)
        {
            _view = view;
            _autoLockTimer = new System.Timers.Timer(1000);
            _autoLockTimer.Elapsed += _autoLockTimer_Elapsed;
            _searchRefresh = new System.Timers.Timer(1000);
            _searchRefresh.Elapsed += _searchRefresh_Elapsed;
            _duplicateCredentialCommand = new Command(new Action<Object>(DuplicateCredentialCommandAction));
            _editCredentialCommand = new Command(new Action<Object>(EditCredentialCommandAction));
            _removeCredentialCommand = new Command(new Action<Object>(RemoveCredentialCommandAction));
            _credentialSelectedCommand = new Command(new Action<Object>(CredentialSelectedCommandAction));
            _viewCredentialAuditLogCommand = new Command(new Action<Object>(ViewCredentialAuditLogCommandAction));
            _searchTextChangedCommand = new Command(new Action<Object>(SearchTextChangedCommandAction));
            _searchCompletedCommand = new Command(new Action<Object>(SearchCompletedCommandAction));
            _clipboardFieldCopyCommand = new Command(new Action<Object>(ClipboardFieldCopyCommandAction));
            _clipboardFieldRevealCommand = new Command(new Action<Object>(ClipboardFieldRevealCommandAction));
            _openInBrowserCommand = new Command(new Action<Object>(OpenInBrowserCommandAction));
            _closeCredentialListTipCommand = new Command(new Action<Object>(CloseCredentialListTipCommandAction));
        }

        #endregion

        #region private methods

        private static IEnumerable<T> ConcatenateEnumerables<T>(params IEnumerable<T>[] lists)
        {
            return lists.SelectMany(x => x);
        }

        private async Task<Common.SaveResult> Save()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("KeyDerivationFunction", AppConfig.Instance.KeyDerivationFunction);
            switch (AppConfig.Instance.KeyDerivationFunction)
            {
                case AppConfig.CACHY_KEYDERIVATIONFUNCTION_PBKDF2:
                    {
                        parameters.Add("IterationCount", AppConfig.Instance.PBKDF2IterationCount.ToString());
                        break;
                    }
                case AppConfig.CACHY_KEYDERIVATIONFUNCTION_SCRYPT:
                    {
                        parameters.Add("IterationCount", AppConfig.Instance.SCryptIterationCount.ToString());
                        parameters.Add("BlockSize", AppConfig.Instance.SCryptBlockSize.ToString());
                        parameters.Add("ThreadCount", AppConfig.Instance.SCryptThreadCount.ToString());
                        break;
                    }
            }

            Common.SaveResult saveResult = await Vault.Save(parameters.ToArray());
            return (saveResult);
        }

        private void ResetIdleTime()
        {
            _idleSince = DateTime.UtcNow;
        }

        private async Task<Boolean> UnlockVault()
        {
            if (VaultIndex != null && !String.IsNullOrEmpty(MasterPassphrase))
            {
                try
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("KeyDerivationFunction", AppConfig.Instance.KeyDerivationFunction);
                    switch (AppConfig.Instance.KeyDerivationFunction)
                    {
                        case AppConfig.CACHY_KEYDERIVATIONFUNCTION_PBKDF2:
                            {
                                parameters.Add("IterationCount", AppConfig.Instance.PBKDF2IterationCount.ToString());
                                break;
                            }
                        case AppConfig.CACHY_KEYDERIVATIONFUNCTION_SCRYPT:
                            {
                                parameters.Add("IterationCount", AppConfig.Instance.SCryptIterationCount.ToString());
                                parameters.Add("BlockSize", AppConfig.Instance.SCryptBlockSize.ToString());
                                parameters.Add("ThreadCount", AppConfig.Instance.SCryptThreadCount.ToString());
                                break;
                            }
                    }

                    AESEncryptedVaultSerialiser serialiser = new AESEncryptedVaultSerialiser();
                    GenericResult<Common.LoadResult, Vault> loadResult = await _vaultIndex.Load(serialiser, MasterPassphrase, parameters.ToArray());
                    _masterPassphrase = String.Empty;
                    if (loadResult.Result == Common.LoadResult.Success)
                    {
                        Vault = loadResult.Value;
                        Vault.PropertyChanged += _vault_PropertyChanged;
                        if (!_vaultIndex.IsInLocalVaultStore && !VaultIndexFile.Instance.VaultIsIndexed(_vault))
                        {
                            if (VaultIndex.SyncMode == Common.SyncMode.CloudProvider)
                            {
                                throw new NotImplementedException("Adding existing cloud provider vaults is not currently supported.");
                            }

                            VaultIndexFile.Instance.AddVaultToLocalVaultStoreIndex(_vault,
                                VaultIndex.SyncMode,
                                VaultIndex.Provider,
                                String.Empty,
                                false);
                        }

                        NotifyPropertyChanged("Vault");
                        NotifyPropertyChanged("FilteredCredentials");
                        UpdatePageTitle();
                        ResetIdleTime();

                        if (_vaultIndex.Unopened)
                        {
                            //Mark it as opened and update the name and description
                            _vaultIndex.MarkAsOpened(
                                _vault.Name,
                                _vault.Description);
                            VaultIndexFile.Instance.Save();
                        }
                        _vaultIndex = null;

                        if (AppConfig.Instance.AutoCloseVault) _autoLockTimer.Start();
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                }
                catch (Exception ex)
                {
                    await App.Controller.MainPageInstance.DisplayAlert("Unlock Vault",
                        String.Format("Failed to unlock the vault, {0}.", ex.Message),
                        "OK");
                    ((App)App.Current).NavigateToVaultsList();
                    return (false);
                }
            }
            else
            {
                return (false);
            }
        }

        private void ClearSelectedCredential(bool deselectItem)
        {
            if (_selectedCredential != null)
            {
                _selectedCredential.IsSelected = false;     //This will clear the clipboard fields
                _selectedCredential = null;
                if (deselectItem)
                {
                    ListView listView = View.FindByName<ListView>("FilteredCredentialsList");
                    listView.SelectedItem = null;
                }
            }
            if (Vault != null)
            {
                //Make sure all items are unselected
                foreach (Credential curCredential in Vault.Credentials)
                {
                    curCredential.IsSelected = false;
                }
            }
        }

        private void UpdatePageTitle()
        {
            if (_viewItem != null)
            {
                _viewItem.PageTitle = Vault != null ? Vault.Name : String.Empty;
            }
        }

        #endregion

        #region public methods

        public async void Lock(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Lock command invoked.");

            if (Vault.IsDirty)
            {
                Common.SaveResult saveResult = Common.SaveResult.None;
                if (await App.Controller.MainPageInstance.DisplayAlert("Lock Vault",
                    "This vault has unsaved changes, would you like to save the changes before locking the vault?  Changes will be lost of not saved.",
                    "Yes", "No"))
                {
                    saveResult = await Save();
                }
                if (saveResult == Common.SaveResult.Success)
                {
                    VaultIndexFile.Invalidate();
                }
                ((App)App.Current).NavigateToVaultsList();
            }
            else
            {
                ((App)App.Current).NavigateToVaultsList();
            }
        }

        public void CreateCredential(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Create Credential.");

            App.Controller.NavigateTo("crededit",
                new KeyValuePair<String, Object>("Credential", Vault.CreateCredential()));
        }

        public void Audit(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "View audit log.");

            App.Controller.ShowPopup("vault.vaultaudit",
                new KeyValuePair<String, Object>("Vault", Vault));
        }

        public async void ImportCSV(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Import CSV.");

            FileData file = await CrossFilePicker.Current.PickFile(new string[] { ".csv" });
            if (file != null)
            {
                string destinationPath = file.FilePath;
                byte[] fileData = await devoctomy.cachy.Framework.Native.Native.FileHandler.ReadAsync(destinationPath);
                string fileStringData = System.Text.Encoding.UTF8.GetString(fileData);
                CSVImporter import = new CSVImporter(fileStringData);
                Common.CSVFormat autoFormat = await import.DetermineFormat();
                App.Controller.ShowPopup("vault.importsource",
                    new KeyValuePair<string, object>("Format", autoFormat),
                    new KeyValuePair<string, object>("CSVImporter", import));
            }
        }

        public async Task ExportCSV(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Export CSV.");

            VaultExporter exporter = new VaultExporter(
                Common.ExportFormat.cachyCSV1_0,
                Common.ExportWrapping.PasswordProtectedZip);

            string extension = exporter.ExportWrapping == Common.ExportWrapping.PasswordProtectedZip ? "zip" : "csv";
            string name = String.Format("{0}_{1}", Vault.Name, DateTime.Now.ToString("ddMMyyyy"));
            string fileName = String.Format("{0}.{1}", name, extension);

            string passsword = SimpleRandomGenerator.QuickGetRandomString(
                SimpleRandomGenerator.CharSelection.All,
                16,
                true);

            byte[] exportData = exporter.Export(
                Vault,
                new KeyValuePair<string, string>("password", passsword));   //auto generate the password and display it in a popup after

            String fullExportPath = String.Empty;
            bool success = false;

            try
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.UWP:
                        {
                            KeyValuePair<string, string[]> extensions = new KeyValuePair<string, string[]>(extension.ToUpper(), new string[] { String.Format(".{0}", extension) });
                            KeyValuePair<string, Stream>? output = await devoctomy.cachy.Framework.Native.Native.FileHandler.PickFileForSave(
                                fileName,
                                extensions);
                            if (output.HasValue)
                            {
                                fullExportPath = output.Value.Key;
                                await output.Value.Value.WriteAsync(exportData, 0, exportData.Length);
                                await output.Value.Value.FlushAsync();
                                output.Value.Value.Close();
                                success = true;
                            }

                            break;
                        }
                    default:
                        {
                            String appDataPath = String.Empty;
                            devoctomy.DFramework.Core.IO.Directory.ResolvePath("{AppData}", out appDataPath);
                            if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
                            String vaultExportsPath = String.Format("{0}{1}", appDataPath, "Exports");
                            fullExportPath = String.Format("{0}\\{1}", vaultExportsPath, fileName);
                            try
                            {
                                await devoctomy.cachy.Framework.Native.Native.FileHandler.WriteAsync(fullExportPath, exportData);
                                success = true;
                            }
                            catch (Exception)
                            { }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception, "Failed to export credentials. {0}", ex.ToString());
            }

            if (success)
            {
                Credential credential = Vault.CreateCredential();
                credential.GlyphKey = Fonts.CachyFont.Glyph.Export.ToString();
                credential.GlyphColour = "Red";
                credential.Name = name;
                credential.Description = "Password protected ZIP export.";
                credential.Notes = fullExportPath;
                credential.Password = passsword;
                credential.AddToVault(true);

                if (AppConfig.Instance.AutoSave && AppConfig.Instance.AutoSaveOnDuplicatingCred)
                {
                    Common.SaveResult saveResult = await Save();
                    if (saveResult == Common.SaveResult.Success)
                    {
                        VaultIndexFile.Invalidate();
                    }
                }

                NotifyPropertyChanged("FilteredCredentials");

                await App.Controller.MainPageInstance.DisplayAlert("Export Credentials",
                    String.Format("Export was successful, the password for the export ZIP file has been placed in your vault, under the name '{0}'. Please remember to lock your vault to save the credential if you do not have aut-save enabled.", name),
                    "OK");
            }
        }

        public async Task Report(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Report vault command invoked.");

            await Task.Yield();

            App.Controller.ShowPopup("vault.vaultreport",
                new KeyValuePair<String, Object>("Vault", Vault));
        }

        #endregion

        #region ipagenavigationaware

            public void SetParameter(String key,
            Object parameter)
        {
            switch (key)
            {
                case "Vault":
                    {
                        Vault = (Vault)parameter;
                        Vault.PropertyChanged += _vault_PropertyChanged;
                        NotifyPropertyChanged("Vault");
                        NotifyPropertyChanged("FilteredCredentials");
                        UpdatePageTitle();
                        ResetIdleTime();
                        ((App)App.Current).OpenedVault = VaultIndexFile.Instance.Indexes.FirstOrDefault(vi => vi.ID == Vault.ID);
                        if (AppConfig.Instance.AutoCloseVault) _autoLockTimer.Start();

                        break;
                    }
                case "VaultIndex":
                    {
                        //we should be sent this along with the master passphrase 
                        //if the vault is first being opened
                        _vaultIndex = (VaultIndex)parameter;
                        ((App)App.Current).OpenedVault = _vaultIndex;
                        break;
                    }
                case "MasterPassphrase":
                    {
                        _masterPassphrase = (String)parameter;
                        break;
                    }
            }
        }

        public async void SetParameters(params KeyValuePair<String, Object>[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                foreach (KeyValuePair<String, Object> curParameter in parameters)
                {
                    SetParameter(curParameter.Key,
                        curParameter.Value);
                }
            }
            await UnlockVault();
        }

        public async void OnClosePopup(View item, object parameter)
        {
            if (item is SelectImportSourceView)
            {
                if (parameter != null)
                {
                    Dictionary<string, object> parameters = parameter as Dictionary<string, object>;
                    if (parameters != null)
                    {
                        CSVImporter csvImporter = (CSVImporter)parameters["CSVImporter"];
                        ImportSource importSource = (ImportSource)parameters["ImportSource"];
                        if (importSource.Format != Common.CSVFormat.Unknown)
                        {
                            List<ImportFieldMapping> mappings = CSVImporter.CreateMappings(importSource.Format);
                            await csvImporter.ImportToVault(Vault, mappings);
                        }
                        else
                        {
                            App.Controller.ShowPopup(
                                "vault.importmap",
                                new KeyValuePair<string, object>("CSVImporter", csvImporter),
                                new KeyValuePair<string, object>("ImportHeaders", csvImporter.Headers),
                                new KeyValuePair<string, object>("StandardFields", Credential.StandardFields));
                        }
                    }
                }
            }
            else if (item is ImportMappingView)
            {
                Dictionary<string, object> parameters = parameter as Dictionary<string, object>;
                if (parameters != null)
                {
                    CSVImporter csvImporter = (CSVImporter)parameters["CSVImporter"];
                    List<ImportFieldMapping> mappings = (List<ImportFieldMapping>)parameters["Mappings"];
                    await csvImporter.ImportToVault(Vault, mappings);
                }
            }
            else if(item is VaultReportView)
            {
                string filter = (string)parameter;
                if(!String.IsNullOrEmpty(filter))
                {
                    Entry searchEntry = View.FindByName<Entry>("SearchEntry");
                    if (searchEntry != null)
                    {
                        searchEntry.Text = "@" + filter;
                    }
                }
            }
            ResetIdleTime();
            ClearSelectedCredential(true);
            NotifyPropertyChanged("FilteredCredentials");
        }

        public void OnNavigateTo(View view, object parameter)
        {
            _viewItem = parameter as BurgerMenuViewItem;
            UpdatePageTitle();
            ClearSelectedCredential(true);
        }

        public void OnNavigateFrom(View view, object parameter)
        {
            _autoLockTimer.Stop();
            ClearSelectedCredential(true);
        }

        public void OnGoBack(View view, object parameter)
        {
            //reset vault list

        }

        #endregion

        #region commands

        private void CloseCredentialListTipCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Close credential list tip command invoked.");

            ShowCredentialListTip = false;
        }

        public void OpenInBrowserCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Open in browser command invoked.");

            Credential credential = (Credential)parameter;
            if(credential.HasWebsite)
            {
                try
                {
                    string website = credential.Website;
                    if(!website.Contains("://"))
                    {
                        website = String.Format("{0}{1}", AppConfig.Instance.DefaultBrowseProtocol, website);
                    }
                    Uri uri = new Uri(website);
                    Device.OpenUri(uri);
                }
                catch (Exception ex)
                {
                    App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Error, "Failed to browse website '{0}'.", credential.Website);
                }
            }
        }

        public void ViewCredentialAuditLogCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "View credential audit log command invoked.");

            ResetIdleTime();
            Credential credential = (Credential)parameter;
            App.Controller.ShowPopup("vault.credaudit",
                new KeyValuePair<String, Object>("Credential", credential));
        }

        public async void DuplicateCredentialCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Duplicate credential command invoked.");

            Credential credential = (Credential)parameter;
            string offsetNamePrefix = credential.Name;
            int offset = 1;
            string offsetNameSuffix = String.Format("({0})", offset);
            string offsetName = String.Format("{0} {1}", offsetNamePrefix, offsetNameSuffix);
            while(Vault.CredentialExists(offsetName))
            {
                offset += 1;
                offsetNameSuffix = String.Format("({0})", offset);
                offsetName = String.Format("{0} {1}", offsetNamePrefix, offsetNameSuffix);
            }
            Credential copy = credential.Clone(false);
            copy.Name = offsetName;
            Vault.AddCredential(copy, true);

            if(AppConfig.Instance.AutoSave && AppConfig.Instance.AutoSaveOnDuplicatingCred)
            {
                Common.SaveResult saveResult = await Save();
                if (saveResult == Common.SaveResult.Success)
                {
                    VaultIndexFile.Invalidate();
                }
            }

            NotifyPropertyChanged("FilteredCredentials");
        }

        public void EditCredentialCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Edit credential command invoked.");

            Credential credential = (Credential)parameter;
            Credential credentialCopy = credential.Clone(true);
            credentialCopy.BlockVaultDirty = true;
            App.Controller.NavigateTo("crededit",
                new KeyValuePair<String, Object>("Credential", credentialCopy),
                new KeyValuePair<String, Object>("Mode", CreateCredentialViewModel.EditorMode.Edit));
        }

        public async void RemoveCredentialCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Remove credential command invoked.");

            ResetIdleTime();
            Credential credential = (Credential)parameter;
            credential.RemoveFromVault();

            if (AppConfig.Instance.AutoSave && AppConfig.Instance.AutoSaveOnDeletingCred)
            {
                Common.SaveResult saveResult = await Save();
                if (saveResult == Common.SaveResult.Success)
                {
                    VaultIndexFile.Invalidate();
                }
            }

            NotifyPropertyChanged("FilteredCredentials");
        }

        public void CredentialSelectedCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Credential selected command invoked.");

            ResetIdleTime();
            Credential selectedCredential = parameter as Credential;
            ClearSelectedCredential(false);
            if (selectedCredential != null)
            {
                selectedCredential.SetClipboardFields(_clipboardFieldCopyCommand,
                    _clipboardFieldRevealCommand);
                _selectedCredential = selectedCredential;
                _selectedCredential.IsSelected = true;
            }
        }

        public void SearchTextChangedCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Search text changed command invoked.");

            _searchRefresh.Stop();
            _searchRefresh.Start();
        }

        public void SearchCompletedCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Search unfocused command invoked.");

            Entry entry = (Entry)parameter;
            _filterString = entry.Text;
            _searching = !String.IsNullOrEmpty(_filterString);
            NotifyPropertyChanged("FilteredCredentials");
        }

        public async void ClipboardFieldCopyCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Clipboard field copy command invoked.");

            ResetIdleTime();
            ClipboardField field = parameter as ClipboardField;
            if(field != null)
            {
                try
                {
                    if(AppConfig.Instance.EnableClipboardObfuscator)
                    {
                        App.StopClipboardObfuscator(true, new TimeSpan(0, 0, 30));
                        await ClipboardObfuscator.Instance.PerformObfuscation(SimpleRandomGenerator.QuickGetRandomInt(1, 10));
                    }
                    await Xamarin.Essentials.Clipboard.SetTextAsync(field.Value);
                    App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Sensitive,
                        "Successfully copied field to clipboard.");
                }
                catch (Exception ex)
                {
                    App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception,
                        "Failed to set clipboard text.\r\n'{0}'.",
                        ex.ToString());
                }
            }
        }

        public void ClipboardFieldRevealCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Clipboard field copy command invoked.");

            ResetIdleTime();
            ClipboardField field = parameter as ClipboardField;
            if (field != null)
            {
                App.Controller.MainPageInstance.DisplayAlert(String.Format("Reveal {0} Field", field.Name),
                    field.Value,
                    "OK");
            }
        }

        #endregion

        #region object events

        private void _vault_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsEmpty":
                    {
                        NotifyPropertyChanged("ShowCredentialListTip");
                        break;
                    }
            }
        }

        private void _searchRefresh_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _searchRefresh.Stop();
            Entry entry = View.FindByName<Entry>("SearchEntry");
            if(entry != null)
            {
                _filterString = entry.Text;
                _searching = !String.IsNullOrEmpty(_filterString);
                NotifyPropertyChanged("FilteredCredentials");
            }
        }

        private void _autoLockTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            bool _restart = true;
            try
            {
                _autoLockTimer.Stop();
                if (_idleSince.HasValue)
                {
                    TimeSpan openTime = DateTime.UtcNow - _idleSince.Value;
                    if (openTime > AppConfig.Instance.AutoCloseTimeSpan)
                    {
                        //No saving, just navigate back
                        _restart = false;
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ((App)App.Current).NavigateToVaultsList();
                        });
                    }
                }
            }
            finally
            {
                if(_restart) _autoLockTimer.Start();
            }           
        }

        #endregion

    }

}

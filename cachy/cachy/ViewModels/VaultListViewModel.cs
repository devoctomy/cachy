using devoctomy.cachy.Framework.Data;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.Generic;
using cachy.Config;
using System.Linq;
using devoctomy.cachy.Framework.Data.Cloud;
using System.Threading.Tasks;
using cachy.Data;
using devoctomy.cachy.Framework.Serialisers.AESEncrypted;

namespace cachy.ViewModels
{

    public class VaultListViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private VaultListView _view;
        private ObservableCollection<VaultIndex> _vaults = new ObservableCollection<VaultIndex>();
        private bool _isUnlocking;
        private static bool _firstView = true;
        private System.Timers.Timer _syncTimer;
        private static bool _startedClipboardObfuscator;
        private static bool _showVaultListTip = true;
        private bool _isSynchronising;
        private ICommand _vaultSelectedCommand;
        private ICommand _removeVaultCommand;
        private ICommand _deleteVaultCommand;
        private ICommand _vaultInformationCommand;
        private ICommand _closeVaultListTipCommand;

        #endregion

        #region public properties

        public ObservableCollection<VaultIndex> Vaults
        {
            get
            {
                return (_vaults);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (_vaults == null || _vaults.Count == 0);
            }
        }

        public bool IsUnlocking
        {
            get
            {
                return (_isUnlocking);
            }
            set
            {
                if(_isUnlocking != value)
                {
                    _isUnlocking = value;
                    NotifyPropertyChanged("IsUnlocking");
                }
            }
        }

        public VaultListView View
        {
            get
            {
                return (_view);
            }
        }

        public bool ShowVaultListTip
        {
            get
            {
                return (!IsEmpty && _showVaultListTip);
            }
            set
            {
                if(_showVaultListTip != value)
                {
                    _showVaultListTip = value;
                    NotifyPropertyChanged("ShowVaultListTip");
                }
            }
        }

        public bool IsSynchronising
        {
            get
            {
                return (_isSynchronising);
            }
            set
            {
                if(_isSynchronising != value)
                {
                    _isSynchronising = value;
                    NotifyPropertyChanged("IsSynchronising");
                }
            }
        }

        public ICommand VaultSelectedCommand
        {
            get
            {
                return (_vaultSelectedCommand);
            }
        }

        public ICommand RemoveVaultCommand
        {
            get
            {
                return (_removeVaultCommand);
            }
        }

        public ICommand DeleteVaultCommand
        {
            get
            {
                return (_deleteVaultCommand);
            }
        }

        public ICommand VaultInformationCommand
        {
            get
            {
                return (_vaultInformationCommand);
            }
        }

        public ICommand CloseVaultListTipCommand
        {
            get
            {
                return (_closeVaultListTipCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public VaultListViewModel(VaultListView view)
        {
            _view = view;

            _syncTimer = new System.Timers.Timer(new TimeSpan(0, 1, 0).TotalMilliseconds);
            _syncTimer.Elapsed += _syncTimer_Elapsed;
            _vaultSelectedCommand = new Command(new Action<Object>(VaultSelectedCommandAction));
            _removeVaultCommand = new Command(new Action<Object>(RemoveVaultCommandAction));
            _deleteVaultCommand = new Command(new Action<Object>(DeleteVaultCommandAction));
            _vaultInformationCommand = new Command(new Action<Object>(VaultInformationCommandAction));
            _closeVaultListTipCommand = new Command(new Action<Object>(CloseVaultListTipCommandAction));
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed, "VaultListViewModel SetParameter '{0}'.", key);

            switch (key)
            {
                case "Vaults":
                    {
                        Vaults.Clear();
                        ObservableCollection<VaultIndex> vaultIndexes = (ObservableCollection<VaultIndex>)parameter;
                        foreach (VaultIndex curVaultIndex in vaultIndexes)
                        {
                            Vaults.Add(curVaultIndex);
                        }
                        NotifyPropertyChanged("IsEmpty");
                        NotifyPropertyChanged("ShowVaultListTip");

                        break;
                    }
            }
        }

        public void SetParameters(params KeyValuePair<String, Object>[] parameters)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed, "VaultListViewModel SetParameters.");

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
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed, "VaultListViewModel OnClosePopup.");

            if(item is UnlockView)
            {
                Dictionary<string, object> parameters = parameter as Dictionary<string, object>;
                if(parameters != null)
                {
                    VaultIndex vaultIndex = (VaultIndex)parameters["VaultIndex"];
                    string masterPassphrase = (string)parameters["MasterPassphrase"];
                    if(await TestVaultPassphrase(
                        vaultIndex,
                        masterPassphrase))
                    {
                        App.Controller.NavigateTo(
                            "vault",
                            parameters.ToArray());
                    }
                    else
                    {
                        ((App)App.Current).OpenedVault = null;
                        await App.Controller.MainPageInstance.DisplayAlert("Unlock Vault",
                            String.Format("Failed to unlock the vault."),
                            "OK");
                    }
                }

                ListView vaultsList = (ListView)View.FindByName("VaultsList");
                vaultsList.SelectedItem = null;
                IsUnlocking = false;
            }
            else
            {
                ListView vaultsList = (ListView)View.FindByName("VaultsList");
                vaultsList.SelectedItem = null;
            }
        }

        public async void OnNavigateTo(View view, object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed, "VaultListViewModel OnNavigateTo.");

            ListView vaultsList = (ListView)View.FindByName("VaultsList");
            ((App)App.Current).OpenedVault = null;

            await Synchronise(null);

            if (_firstView)
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Vault list first view.");

                if (AppConfig.Instance.AutoOpenVault)
                {
                    App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Attempting to auto-open vault.");

                    if (VaultIndexFile.Instance.Indexes.Count > 0)
                    {
                        App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Looking for vault '{0}'.", AppConfig.Instance.DefaultVaultID);

                        IEnumerable<VaultIndex> defaultVaultMatches = VaultIndexFile.Instance.Indexes.Where(vi => vi.ID == AppConfig.Instance.DefaultVaultID);
                        if (defaultVaultMatches.Any())
                        {
                            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Found vault, auto opening.");

                            vaultsList.SelectedItem = defaultVaultMatches.First();
                        }
                        else
                        {
                            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Vault not found.");

                            vaultsList.SelectedItem = null;
                        }
                    }
                    else
                    {
                        App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "No vaults to auto-open.");
                        vaultsList.SelectedItem = null;
                    }
                }

                ClipboardObfuscator.Initialise(
                    App.AppLogger.Logger,
                    ObfuscateClipboardAction);
                App.SetClipboardObfuscatorInitialised();

                _firstView = false;
            }
            else
            {
                vaultsList.SelectedItem = null;
            }

            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Starting cloud synchronisation timer.");
            _syncTimer.Start();

            if (!_startedClipboardObfuscator && AppConfig.Instance.EnableClipboardObfuscator)
            {
                App.StartClipboardObfuscator(true);
                _startedClipboardObfuscator = true;
            }
            else if(_startedClipboardObfuscator && !AppConfig.Instance.EnableClipboardObfuscator)
            {
                App.StopClipboardObfuscator(false, new TimeSpan(0));
                _startedClipboardObfuscator = false;
            }
        }

        public void OnNavigateFrom(View view, object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseLow | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Stopping cloud synchronisation timer.");
            //_syncTimer.Stop();
        }

        public async void OnGoBack(View view, object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed, "VaultListViewModel OnGoBack.");

            RefreshVaults();
            await Synchronise(null);

            if (!_startedClipboardObfuscator && AppConfig.Instance.EnableClipboardObfuscator)
            {
                App.StartClipboardObfuscator(true);
                _startedClipboardObfuscator = true;
            }
            else if (_startedClipboardObfuscator && !AppConfig.Instance.EnableClipboardObfuscator)
            {
                App.StopClipboardObfuscator(false, new TimeSpan(0));
                _startedClipboardObfuscator = false;
            }
        }

        #endregion

        #region private methods

        private void RefreshVaults()
        {
            Vaults.Clear();
            foreach (VaultIndex curVaultIndex in VaultIndexFile.Instance.Indexes)
            {
                Vaults.Add(curVaultIndex);
            }
            NotifyPropertyChanged("IsEmpty");
            NotifyPropertyChanged("ShowVaultListTip");
        }

        private async Task<bool> TestVaultPassphrase(
            VaultIndex vaultIndex,
            string masterPassphrase)
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
            try
            {
                GenericResult<Common.LoadResult, Vault> loadResult = await vaultIndex.Load(serialiser, masterPassphrase, parameters.ToArray());
                return (loadResult.Result == Common.LoadResult.Success);
            }
            catch (Exception ex)
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception,
                    "Failed to unlock the vault at '{0}'.\r\n'{1}'.",
                    vaultIndex.FullPath,
                    ex.Message);
                return (false);
            }
        }

        private async Task ObfuscateClipboardAction(ClipboardObfuscatorGeneratedFakeEventArgs args)
        {
            try
            {
                //App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Sensitive,
                //    "Obfuscating clipboard with '{0}'.",
                //    args.Value);

                await Xamarin.Essentials.Clipboard.SetTextAsync(args.Value);
                App.StartClipboardObfuscator(false);

            }
            catch (Exception)
            {
                //App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception,
                //    "Failed to set clipboard text, this can occur when the application is not in the foreground.\r\n'{0}'.",
                //    ex.ToString());
            }
        }

        #endregion

        #region public methods

        public void AddExistingVault(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Add Existing Vault method invoked.");

            App.Controller.ShowPopup("vaultlist.addexistingvaultpopup", null);
        }


        public void CreateVault(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Create Vault invoked.");

            App.Controller.NavigateTo("createvault");
        }

        public async Task Synchronise(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Synchronise invoked.");

            if(!IsSynchronising)
            {
                try
                {
                    IsSynchronising = true;
                    if (Vaults.Count > 0)
                    {
                        App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Performing auto cloud synchronisation.");
                        VaultIndex opened = ((App)App.Current).OpenedVault;
                        await CloudStorageSyncManager.Instance.UpdateAllVaultsSyncStatus(opened);
                        await CloudStorageSyncManager.Instance.SyncAllVaults(opened);
                    }
                    else
                    {
                        App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Skipping synchronisation as there are no vaults.");
                    }
                }
                finally
                {
                    IsSynchronising = false;
                }
            }
        }

        #endregion

        #region commands

        private void VaultSelectedCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Vault Selected command invoked.");

            VaultIndex vaultIndex = (VaultIndex)parameter;
            if (vaultIndex != null)
            {
                IsUnlocking = true;
                App.Controller.ShowPopup("vaultlist.unlockvault",
                    new KeyValuePair<String, Object>("VaultIndex", vaultIndex));
            }
        }

        private void RemoveVaultCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Remove Vault command invoked.");

            VaultIndex vaultIndex = (VaultIndex)parameter;
            if (VaultIndexFile.Instance.RemoveFromVault(vaultIndex))
            {
                Vaults.Remove(vaultIndex);
                NotifyPropertyChanged("IsEmpty");
                NotifyPropertyChanged("ShowVaultListTip");
            }
        }

        private async void DeleteVaultCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Delete Vault command invoked.");

            VaultIndex vaultIndex = (VaultIndex)parameter;
            if (vaultIndex.IsInLocalVaultStore)
            {
                Boolean? deleted = VaultIndexFile.Instance.DeleteFromVault(vaultIndex);
                if (deleted.HasValue)
                {
                    Vaults.Remove(vaultIndex);
                    if (!deleted.Value)
                    {
                        await App.Controller.MainPageInstance.DisplayAlert("Delete Vault",
                            "Successfully removed the vault from cachy, but failed to delete the vault file from the local vault store.",
                            "OK");
                    }
                }
                else
                {
                    await App.Controller.MainPageInstance.DisplayAlert("Delete Vault",
                        "Failed to remove the vault from cachy.",
                        "OK");
                }
                NotifyPropertyChanged("IsEmpty");
                NotifyPropertyChanged("ShowVaultListTip");
            }
            else
            {
                await App.Controller.MainPageInstance.DisplayAlert("Delete Vault",
                    "Only vaults that are stored in the local vault store can be deleted, you must remove this vault instead.",
                    "OK");
            }
        }

        private void VaultInformationCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Vault information command invoked.");

            VaultIndex index = parameter as VaultIndex;
            if (index != null)
            {
                App.Controller.ShowPopup("vaultlist.vaultinfo",
                    new KeyValuePair<string, object>("VaultIndex", index));
            }
        }

        private void CloseVaultListTipCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Close vault list tip command invoked.");

            ShowVaultListTip = false;
        }

        #endregion

        #region object events

        private async void _syncTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {           
            try
            {
                _syncTimer.Stop();
                await Synchronise(null);
            }
            finally
            {
                _syncTimer.Start();
            }
        }

        private void _vaults_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("IsEmpty");
            NotifyPropertyChanged("ShowVaultListTip");
        }

        #endregion

    }

}

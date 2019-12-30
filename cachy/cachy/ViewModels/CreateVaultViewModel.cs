using cachy.Config;
using cachy.Controls;
using cachy.Navigation;
using cachy.Navigation.BurgerMenu;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Data.Cloud;
using devoctomy.cachy.Framework.Serialisers.AESEncrypted;
using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class CreateVaultViewModel : ViewModelBase, IPageNavigationAware
    {

        #region public enums

        public enum SyncMode
        {
            None = 0,
            LocalOnly = 1,
            CloudSync = 2
        }

        #endregion

        #region private objects

        private CreateNewVaultView _view;
        private string _defaultTabPage = "INFO";
        private string _selectedTabPage = String.Empty;
        private string _fileName = "{ID}.vault";
        private string _name = String.Empty;
        private string _description = String.Empty;
        private string _masterPassphrase = String.Empty;
        private bool _showMasterPassphrase;
        private SyncMode _syncMode = SyncMode.LocalOnly;
        private CloudProvider _selectedCloudProvider;

        private ICommand _addCloudProvider;

        #endregion

        #region public properties

        public String DefaultTabPage
        {
            get
            {
                return (_defaultTabPage);
            }
            set
            {
                if(_defaultTabPage != value)
                {
                    _defaultTabPage = value;
                    NotifyPropertyChanged("DefaultTabPage");
                }
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

        public string FileName
        {
            get
            {
                return (_fileName);
            }
            set
            {
                if(_fileName != value)
                {
                    _fileName = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public String Name
        {
            get
            {
                return (_name);
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public String Description
        {
            get
            {
                return (_description);
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public String MasterPassphrase
        {
            get
            {
                return (_masterPassphrase);
            }
            set
            {
                if (_masterPassphrase != value)
                {
                    _masterPassphrase = value;
                    NotifyPropertyChanged("MasterPassphrase");
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
                    NotifyPropertyChanged("ShowMasterPassphrase");
                }
            }
        }

        public Boolean SyncModeLocalOnly
        {
            get
            {
                return (_syncMode == SyncMode.LocalOnly);
            }
            set
            {
                _syncMode = value ? SyncMode.LocalOnly : SyncMode.CloudSync;
                NotifyPropertyChanged("SyncModeLocalOnly");
                NotifyPropertyChanged("SyncModeCloudSync");
            }
        }

        public Boolean SyncModeCloudSync
        {
            get
            {
                return (_syncMode == SyncMode.CloudSync);
            }
            set
            {
                _syncMode = value ? SyncMode.CloudSync : SyncMode.LocalOnly;
                NotifyPropertyChanged("SyncModeLocalOnly");
                NotifyPropertyChanged("SyncModeCloudSync");
            }
        }

        public CreateNewVaultView View
        {
            get
            {
                return (_view);
            }
        }

        public ObservableCollection<CloudProvider> CloudProviders
        {
            get
            {
                return (Config.CloudProviders.Instance.Providers);
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
                    if(value != null && value.CredentialError)
                    {
                        Task alertTask = App.Controller.MainPageInstance.DisplayAlert("Cloud Provider",
                            "This cloud provider has a credential error, you must reauthenticate through the App settings 'SYNC' tab to re-enable it.",
                            "OK");
                        value.IsSelected = false;

                        //!!!We need to deselect the item in the list
                        //_selectedCloudProvider = null;
                        //NotifyPropertyChanged("SelectedCloudProvider");
                    }
                    else
                    {
                        _selectedCloudProvider = value;
                        NotifyPropertyChanged("SelectedCloudProvider");
                    }
                }
            }
        }

        public ICommand AddCloudProvider
        {
            get
            {
                return (_addCloudProvider);
            }
        }

        #endregion

        #region constructor / destructor

        public CreateVaultViewModel(CreateNewVaultView view)
        {
            _view = view;

            _addCloudProvider = new Command<object>(new Action<object>(AddCloudProviderAction));
        }

        #endregion

        #region base class overrides

        protected override bool OnValidate()
        {
            if (String.IsNullOrEmpty(Name)) return (false);
            if (String.IsNullOrEmpty(MasterPassphrase)) return (false);
            if(SyncModeCloudSync)
            {
                if(SelectedCloudProvider == null)
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

        private string CreateFileName(
            Vault vault,
            string fileName)
        {
            if(String.IsNullOrEmpty(fileName))
            {
                return (String.Format("{0}.vault", vault.ID));
            }
            else
            {
                string name = fileName;
                name = name.Replace("{ID}", vault.ID);
                name = name.Replace("{Name}", vault.Name);
                name = name.Replace("{ddMMyyyy}", DateTime.Now.ToString("ddMMyyyy"));
                return (name);
            }
        }

        #endregion

        #region public methods

        public void Cancel(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Back command invoked.");

            App.Controller.GoBack();
        }

        public async void Accept(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Accept command invoked.");

            if (String.IsNullOrEmpty(Name)) return;
            if (String.IsNullOrEmpty(MasterPassphrase)) return;

            Vault vault = new Vault(Name,
                Description);
            String appDataPath = String.Empty;
            Directory.ResolvePath("{AppData}", out appDataPath);
            if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
            string fileName = CreateFileName(vault, FileName);
            String vaultFullPath = String.Format("{0}{1}", appDataPath, String.Format(@"LocalVaults{0}{1}", DLoggerManager.PathDelimiter, fileName));

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("KeyDerivationFunction", AppConfig.Instance.KeyDerivationFunction);
            switch(AppConfig.Instance.KeyDerivationFunction)
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

            Common.SaveResult result = await vault.SaveAs(
                new AESEncryptedVaultSerialiser(),
                vaultFullPath,
                MasterPassphrase,
                true,
                parameters.ToArray());

            if (result == Common.SaveResult.Success)
            {
                if (SyncModeCloudSync)
                {
                    VaultIndexFile.Instance.AddVaultToLocalVaultStoreIndex(vault,
                        Common.SyncMode.CloudProvider,
                        SelectedCloudProvider.ID,
                        String.Format("/{0}/{1}", "Vaults", fileName),
                        false);
                }
                else
                {
                    VaultIndexFile.Instance.AddVaultToLocalVaultStoreIndex(vault,
                        Common.SyncMode.LocalOnly,
                        String.Empty,
                        String.Empty,
                        false);
                }
            }
            else
            {
                await App.Controller.MainPageInstance.DisplayAlert("Create Vault",
                    "Failed to create vault.",
                    "OK");
            }
            ((App)App.Current).NavigateToVaultsList();
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            
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

        public void OnClosePopup(View item, object parameter)
        {
            if(item is AddCloudProviderView && parameter != null)
            {
                SelectedCloudProvider = (CloudProvider)parameter;               
            }
            EnableDisableAccept(IsValid);
        }

        public void OnNavigateTo(View view, object parameter)
        {
            SelectedTabPage = DefaultTabPage;
            EnableDisableAccept(IsValid);
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

            App.Controller.ShowPopup("createvault.addcloudprovider");
        }

        #endregion

    }

}

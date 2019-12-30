using cachy.Config;
using cachy.Controls;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Data.Cloud;
using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using Newtonsoft.Json.Linq;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class AddExistingVaultViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private AddExistingVaultView _view;
        private CloudProvider _selectedCloudProvider;

        private ICommand _localVaultCommand;
        private ICommand _addCloudProvider;
        private ICommand _backCommand;
        private ICommand _acceptCommand;

        #endregion

        #region public properties

        public AddExistingVaultView View
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
                if (_selectedCloudProvider != value)
                {
                    _selectedCloudProvider = value;
                    NotifyPropertyChanged("SelectedCloudProvider");
                    BrowseCloudProvider();
                }
            }
        }

        public ICommand LocalVaultCommand
        {
            get
            {
                return (_localVaultCommand);
            }
        }

        public ICommand AddCloudProvider
        {
            get
            {
                return (_addCloudProvider);
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return (_backCommand);
            }
        }

        public ICommand AcceptCommand
        {
            get
            {
                return (_acceptCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public AddExistingVaultViewModel(AddExistingVaultView view)
        {
            _view = view;

            _localVaultCommand = new Command(new Action<Object>(LocalVaultCommandAction));
            _addCloudProvider = new Command<object>(new Action<object>(AddCloudProviderAction));
            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acceptCommand = new Command(new Action<Object>(AcceptCommandAction));
        }

        #endregion

        #region private methods

        private void BrowseCloudProvider()
        {
            if(SelectedCloudProvider != null)
            {
                App.Controller.ShowPopup("vaultlist.cloudfiles",
                    new KeyValuePair<string, object>("ProviderID", SelectedCloudProvider.ID));
            }
        }

        private void ClearSelectedProvider(bool deselectItem)
        {
            if (_selectedCloudProvider != null)
            {
                _selectedCloudProvider.IsSelected = false;
                _selectedCloudProvider = null;
                if (deselectItem)
                {
                    CloudProvidersList providerList = View.FindByName<CloudProvidersList>("CloudProvidersList");
                    providerList.DeSelect();
                }
            }
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            //set parameters
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
            if(item is CloudProviderFileSelectView)
            {
                if(parameter != null)
                {
                    CloudStorageProviderFileBase cloudFile = parameter as CloudStorageProviderFileBase;
                    if(cloudFile != null)
                    {
                        String appDataPath = String.Empty;
                        Directory.ResolvePath("{AppData}", out appDataPath);
                        if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
                        String vaultFullPath = String.Format("{0}{1}", appDataPath, String.Format(@"LocalVaults{0}{1}", DLoggerManager.PathDelimiter, cloudFile.Name));

                        string cloudFilePath = cloudFile.Path;

                        IEnumerable<CloudProvider> providers = cachy.Config.CloudProviders.Instance.Providers.Where(cp => cp.ID == SelectedCloudProvider.ID);
                        if (providers.Any())
                        {
                            CloudProvider provider = providers.First();
                            switch (provider.AuthType)
                            {
                                case ProviderType.AuthenticationType.Amazon:
                                    {
                                        string secret = ((App)App.Current).GetCredential(SelectedCloudProvider.ID);
                                        JObject s3ConfigJSON = JObject.Parse(secret);
                                        AmazonS3Config s3Config = AmazonS3Config.FromJSON(s3ConfigJSON);

                                        if(cloudFilePath.StartsWith(s3Config.Path))
                                        {
                                            cloudFilePath = cloudFilePath.Substring(s3Config.Path.Length);
                                        }

                                        break;
                                    }
                            }
                        }

                        VaultIndex index = VaultIndexFile.Instance.CreateLocalVaultStoreIndex("Name",
                            "Description",
                            Common.SyncMode.CloudProvider,
                            SelectedCloudProvider.ID,
                            vaultFullPath,
                            cloudFilePath);

                        ClearSelectedProvider(true);
                        ((App)App.Current).NavigateToVaultsList();
                    }
                }
                else
                {
                    ClearSelectedProvider(true);
                }
            }
            else if (item is AddCloudProviderView)
            {
                if(parameter != null)
                {
                    SelectedCloudProvider = (CloudProvider)parameter;
                }
            }
        }

        public void OnNavigateTo(View view, object parameter)
        {

        }

        public void OnNavigateFrom(View view, object parameter)
        {

        }

        public void OnGoBack(View view, object parameter)
        {

        }

        #endregion

        #region commands

        public async void LocalVaultCommandAction(object parameter)
        {
            FileData file = await CrossFilePicker.Current.PickFile(new string[] { ".vault" });
            if (file != null)
            {
                string destinationPath = file.FilePath;
                VaultIndex index = VaultIndex.Prepare(destinationPath);
                App.Controller.ShowPopup("vaultlist.unlockvault",
                    new KeyValuePair<String, Object>("VaultIndex", index));
            }
        }

        public void AddCloudProviderAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "AddCloudProvider invoked.");

            App.Controller.ShowPopup("vaultlist.addcloudprovider");
        }

        public void BackCommandAction(Object parameter)
        {
            App.Controller.ClosePopup(parameter);
        }

        public void AcceptCommandAction(Object parameter)
        {
            //accept
        }

        #endregion

    }

}

using cachy.Config;
using cachy.Controls;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Cryptography.OAuth;
using devoctomy.cachy.Framework.Data.Cloud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class AddCloudProviderViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private AddCloudProviderView _view;

        private ProviderType _cloudStorageProviderType;
        private CloudStorageProviderBase _cloudStorageProvider;
        private CloudStorageProviderUserBase _cloudStorageAccountUser;
        private bool _isAuthenticated;
        private string _accessToken = String.Empty;
        private AmazonS3Config _s3Config = null;
        private bool _isConnecting;

        private ICommand _providerLogin;
        private ICommand _providerLogout;
        private ICommand _backCommand;
        private ICommand _acceptCommand;

        #endregion

        #region public properties

        public AddCloudProviderView View
        {
            get
            {
                return (_view);
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
                    NotifyPropertyChanged("CloudStorageProviderType");
                    if(value != null)
                    {
                        SelectProviderType();
                    }
                }
            }
        }

        public CloudStorageProviderUserBase CloudStorageAccountUser
        {
            get
            {
                return (_cloudStorageAccountUser);
            }
        }

        public string LoginMessage
        {
            get
            {
                return (String.Format("You are now logged into '{0}' with the following account,", CloudStorageProviderType == null ? String.Empty : CloudStorageProviderType.Name));
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
                    NotifyPropertyChanged("CloudStorageAccountUser");
                    NotifyPropertyChanged("LoginMessage");
                    NotifyPropertyChanged("IsAuthenticated");
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
                    NotifyPropertyChanged("IsConnecting");
                }
            }
        }

        public ICommand ProviderLogin
        {
            get
            {
                return (_providerLogin);
            }
        }

        public ICommand ProviderLogout
        {
            get
            {
                return (_providerLogout);
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

        public AddCloudProviderViewModel(AddCloudProviderView view)
        {
            _view = view;

            _providerLogin = new Command(new Action<Object>(ProviderLoginAction));
            _providerLogout = new Command(new Action<Object>(ProviderLogoutAction));
            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acceptCommand = new Command(new Action<Object>(AcceptCommandAction));
        }

        #endregion

        #region private methods

        private void SelectProviderType()
        {
            ProviderLoginAction(null);
        }

        private void DeselectProviderType()
        {
            SupportedCloudProvidersList list = View.FindByName("SupportedProvidersList") as SupportedCloudProvidersList;
            if (list != null)
            {
                list.DeselectAll();
            }
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            //set parameters here
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
            if (item is OAuthAuthenticateView)
            {
                if(parameter != null)
                {
                    try
                    {
                        Dictionary<string, object> parameters = parameter as Dictionary<string, object>;
                        string accessToken = AuthenticationHelpers.CompleteOAuthAutentication(parameters);

                        Dictionary<string, string> createParams = new Dictionary<string, string>();
                        createParams.Add("AuthType", "OAuth");
                        createParams.Add("ProviderKey", (string)parameters["ProviderID"]);
                        createParams.Add("AccessToken", accessToken);
                        _cloudStorageProvider = CloudStorageProviderBase.Create(
                            App.AppLogger.Logger,
                            createParams);

                        CloudProviderResponse<CloudStorageProviderUserBase> getAccountUserResponse = await _cloudStorageProvider.GetAccountUser();
                        if (getAccountUserResponse.ResponseValue == CloudProviderResponse<CloudStorageProviderUserBase>.Response.Success)
                        {
                            _cloudStorageAccountUser = getAccountUserResponse.Result;
                            _accessToken = accessToken;
                            IsAuthenticated = true;
                        }
                        else
                        {
                            _cloudStorageProvider = null;
                            IsAuthenticated = false;
                        }
                        IEnumerable<ProviderType> matchingTypes = SupportedProviderTypes.SupportedTypes.Where(pt => pt.Name == _cloudStorageProvider.TypeName);
                        if (matchingTypes.Any())
                        {
                            _cloudStorageProviderType = matchingTypes.First();
                        }
                        IsConnecting = false;
                        NotifyPropertyChanged("CloudStorageProviderType");

                    }
                    finally
                    {
                        IsConnecting = false;
                    }
                }
                else
                {
                    IsConnecting = false;
                    DeselectProviderType();
                }
            }
            else if(item is AmazonS3SetupView)
            {
                if (parameter != null)
                {
                    try
                    {
                        AmazonS3Config s3Config = (AmazonS3Config)parameter;
                        Dictionary<string, string> createParams = new Dictionary<string, string>();
                        createParams.Add("ProviderKey", "AmazonS3");
                        createParams.Add("AccessID", s3Config.AccessID);
                        createParams.Add("SecretKey", s3Config.SecretKey);
                        createParams.Add("Region", s3Config.Region);
                        createParams.Add("BucketName", s3Config.BucketName);
                        createParams.Add("Path", s3Config.Path);
                        _cloudStorageProvider = CloudStorageProviderBase.Create(
                            App.AppLogger.Logger,
                            createParams);

                        CloudProviderResponse<CloudStorageProviderUserBase> getAccountUserResponse = await _cloudStorageProvider.GetAccountUser();
                        if (getAccountUserResponse.ResponseValue == CloudProviderResponse<CloudStorageProviderUserBase>.Response.Success)
                        {
                            _cloudStorageAccountUser = getAccountUserResponse.Result;
                            _s3Config = s3Config;
                            IsAuthenticated = true;
                        }
                        else
                        {
                            _cloudStorageProvider = null;
                            IsAuthenticated = false;
                        }
                        IEnumerable<ProviderType> matchingTypes = SupportedProviderTypes.SupportedTypes.Where(pt => pt.Name == _cloudStorageProvider.TypeName);
                        if (matchingTypes.Any())
                        {
                            _cloudStorageProviderType = matchingTypes.First();
                        }
                        IsConnecting = false;
                        NotifyPropertyChanged("CloudStorageProviderType");
                    }
                    finally
                    {
                        IsConnecting = false;
                    }
                }
                else
                {
                    IsConnecting = false;
                    DeselectProviderType();
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

        public async void ProviderLoginAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "ProviderLoginAction command invoked.");
            IsConnecting = true;
            switch(CloudStorageProviderType.AuthType)
            {
                case ProviderType.AuthenticationType.OAuth:
                    {
                        devoctomy.cachy.Framework.Native.Native.WebUtility.ClearInAppBrowserCache();
                        Dictionary<string, object> parameters = await AuthenticationHelpers.BeginOAuthAuthentication(CloudStorageProviderType.Name);
                        if (parameters != null)
                        {
                            App.Controller.ShowPopup("createvault.oauth",
                                parameters.ToArray());
                        }
                        else
                        {
                            await App.Controller.MainPageInstance.DisplayAlert("Authentication Failure",
                                "Failed to start the authentication process, please try again later.",
                                "OK");
                        }
                        break;
                    }
                case ProviderType.AuthenticationType.Amazon:
                    {
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        AmazonS3Config config = new AmazonS3Config(
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            String.Empty,
                            "cachy");
                        parameters.Add("S3Config", config);
                        App.Controller.ShowPopup("createvault.s3setup",
                            parameters.ToArray());
                        break;
                    }
            }
        }

        public void ProviderLogoutAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "ProviderLogout command invoked.");

            _cloudStorageProvider = null;
            _cloudStorageAccountUser = null;
            _accessToken = String.Empty;
            IsAuthenticated = false;
        }

        public void BackCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Back command invoked.");

            DeselectProviderType();
            App.Controller.ClosePopup(null);
        }

        public void AcceptCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Accept command invoked.");

            CloudProvider cloudProvider = null;
            switch (CloudStorageProviderType.AuthType)
            {
                case ProviderType.AuthenticationType.OAuth:
                    {
                        cloudProvider = CloudProviders.Instance.AddProvider(
                            ProviderType.AuthenticationType.OAuth,
                            _cloudStorageProvider.TypeName,
                            CloudStorageAccountUser.Email,
                            _accessToken,
                            true);
                        break;
                    }
                case ProviderType.AuthenticationType.Amazon:
                    {
                        cloudProvider = CloudProviders.Instance.AddProvider(
                            ProviderType.AuthenticationType.Amazon,
                            _cloudStorageProvider.TypeName,
                            _s3Config.AccessID,
                            _s3Config.ToString(),
                            true);
                        break;
                    }
            }
            DeselectProviderType();
            App.Controller.ClosePopup(cloudProvider);
        }

        #endregion

    }

}

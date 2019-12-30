using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Cryptography.OAuth;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class OAuthAuthenticateViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private OAuthAuthenticateView _view;
        private string _title = String.Empty;
        private string _authCode = String.Empty;
        private string _providerID = String.Empty;
        private string _sessionID = String.Empty;
        private Uri _authoriseURI;
        private RSA _rsa;
        private ICommand _backCommand;

        #endregion

        #region public properties

        public OAuthAuthenticateView View
        {
            get
            {
                return (_view);
            }
        }

        public string Title
        {
            get
            {
                return (_title);
            }
        }

        public Uri AuthenticateURI
        {
            get
            {
                return (_authoriseURI);
            }
            set
            {
                if(_authoriseURI != value)
                {
                    _authoriseURI = value;
                    NotifyPropertyChanged("AuthenticateURI");
                }
            }
        }

        public string AuthCode
        {
            get
            {
                return (_authCode);
            }
            set
            {
                if(_authCode != value)
                {
                    _authCode = value;
                    NotifyPropertyChanged("AuthCode");
                    NotifyPropertyChanged("AuthCodeProvided");
                }
            }
        }

        public bool AuthCodeProvided
        {
            get
            {
                return (!String.IsNullOrEmpty(AuthCode));
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return (_backCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public OAuthAuthenticateViewModel(OAuthAuthenticateView view)
        {
            _view = view;

            _backCommand = new Command(new Action<Object>(BackCommandAction));
        }

        #endregion

        #region public methods

        public void Cancel(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Cancel invoked.");

            App.Controller.GoBack();
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            switch(key)
            {
                case "Title":
                    {
                        _title = (string)parameter;
                        NotifyPropertyChanged("Title");
                        break;
                    }
                case "RSA":
                    {
                        _rsa = parameter as RSA;
                        break;
                    }
                case "ProviderID":
                    {
                        _providerID = (string)parameter;
                        break;
                    }
                case "SessionID":
                    {
                        _sessionID = (string)parameter;
                        Uri authoriseURI = null;
                        switch (_providerID)
                        {
                            case "Dropbox":
                                {
                                    string redirectURI = Uri.EscapeDataString("https://cachywebfunctions20190202044830.azurewebsites.net/api/DropboxOAuthRedirect");
                                    string uri = String.Format(
                                        "https://www.dropbox.com/oauth2/authorize?response_type=code&client_id={0}&redirect_uri={1}&state={2}",
                                        "bllblee6oqr9q22",
                                        redirectURI,
                                        _sessionID);
                                    authoriseURI = new Uri(uri); // "https://www.dropbox.com/oauth2/authorize?response_type=code&client_id=bllblee6oqr9q22&redirect_uri=https%3A%2F%2Fcachywebfunctions20190202044830.azurewebsites.net%2Fapi%2FDropboxOAuthRedirect&state=" + _sessionID);
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
                                        _sessionID);
                                    authoriseURI = new Uri(uri);
                                    break;
                                }
                        }
                        AuthenticateURI = authoriseURI;
                        break;
                    }
            }
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

        }

        public async void OnNavigateTo(View view, object parameter)
        {
            ((App)App.Current).AndroidSetSoftInputMode(true);
            string continueResponseString = await AuthenticationHelpers.ContinueOAuthAuthentication(_providerID, _sessionID);
            if(!String.IsNullOrEmpty(continueResponseString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("ProviderID", _providerID);
                parameters.Add("AuthResponse", continueResponseString);
                parameters.Add("RSA", _rsa);
                App.Controller.ClosePopup(parameters);
            }
            else
            {
                await App.Controller.MainPageInstance.DisplayAlert("Authentication Error",
                    String.Format("An error occurred whilst authenticating with '{0}'.  Please try again later.", _providerID),
                    "OK");
                App.Controller.ClosePopup(null);
            }
        }

        public void OnNavigateFrom(View view, object parameter)
        {
            ((App)App.Current).AndroidSetSoftInputMode(false);
        }

        public void OnGoBack(View view, object parameter)
        {

        }

        #endregion

        #region commands

        public void BackCommandAction(Object parameter)
        {
            App.Controller.ClosePopup(null);
        }

        #endregion

    }

}

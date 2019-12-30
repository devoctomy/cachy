using cachy.Controls.Interfaces;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Data.Cloud;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace cachy.Config
{

    public class CloudProvider : ICloneable, INotifyPropertyChanged, ISelectableItem
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private string _id = Guid.NewGuid().ToString();
        private ProviderType.AuthenticationType _authType;
        private string _providerKey = String.Empty;
        private string _username = String.Empty;
        private bool _isSelected;
        private bool _credentialError;

        #endregion

        #region public properties

        public CloudProvider Me
        {
            get
            {
                return (this);
            }
        }

        public string ID
        {
            get
            {
                return (_id);
            }
        }

        public ProviderType.AuthenticationType AuthType
        {
            get
            {
                return (_authType);
            }
        }

        public string ProviderKey
        {
            get
            {
                return (_providerKey);
            }
        }

        public string UserName
        {
            get
            {
                if (String.IsNullOrEmpty(_username))
                {
                    _username = GetUserName();
                }
                return (_username);
            }
        }

        public string AccessToken
        {
            get
            {
                return (GetAccessToken());
            }
        }

        public bool IsSelected
        {
            get
            {
                return (_isSelected);
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public string GlyphText
        {
            get
            {
                IEnumerable<ProviderType> matches = SupportedProviderTypes.SupportedTypes.Where(pt => pt.Name == ProviderKey);
                if (matches.Any())
                {
                    return (matches.First().GlyphText);
                }
                else
                {
                    return (String.Empty);
                }
            }
        }

        public bool CredentialError
        {
            get
            {
                return (_credentialError);
            }
        }

        #endregion

        #region constructor / destructor

        public CloudProvider(
            ProviderType.AuthenticationType authType,
            string providerKey)
        {
            _authType = authType;
            _providerKey = providerKey;
        }

        public CloudProvider(
            ProviderType.AuthenticationType authType,
            string id,
            string providerKey)
        {
            _authType = authType;
            _id = id;
            _providerKey = providerKey;
        }

        #endregion

        #region private methods

        private string GetUserName()
        {
            Dictionary<string, string> cred = devoctomy.cachy.Framework.Native.Native.PasswordVault.GetCredential(ID);
            if (cred != null)
            {
                return (cred["UserName"]);
            }
            else
            {
                //This will return a null if cachy has been uninstalled then reinstalled
                //as the AndroidKeyStore will no longer use the same AES key
                return (null);
            }
        }

        private string GetAccessToken()
        {
            Dictionary<string, string> cred = devoctomy.cachy.Framework.Native.Native.PasswordVault.GetCredential(ID);
            if (cred != null)
            {
                return (cred["Password"]);
            }
            else
            {
                //This will return a null if cachy has been uninstalled then reinstalled
                //as the AndroidKeyStore will no longer use the same AES key
                return (null);
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region public methods

        public async Task<bool> CheckCredentialAccessAsync()
        {
            Dictionary<string, string> cred = devoctomy.cachy.Framework.Native.Native.PasswordVault.GetCredential(ID);
            _credentialError = (cred == null);
            if (!_credentialError)
            {
                string secret = cred["Password"];

                switch(AuthType)
                {
                    case ProviderType.AuthenticationType.OAuth:
                        {
                            Dictionary<string, string> parameters = new Dictionary<string, string>();
                            parameters.Add("AuthType", "OAuth");
                            parameters.Add("ProviderKey", ProviderKey);
                            parameters.Add("AccessToken", secret);

                            CloudStorageProviderBase provider = CloudStorageProviderBase.Create(
                                App.AppLogger.Logger,
                                parameters);
                            try
                            {
                                CloudProviderResponse<CloudStorageProviderUserBase> accountUserResponse = await provider.GetAccountUser();
                                _credentialError = !(accountUserResponse.ResponseValue == CloudProviderResponse<CloudStorageProviderUserBase>.Response.Success);
                                _username = String.Empty;
                            }
                            catch (Exception)
                            {
                                _credentialError = true;
                            }
                            break;
                        }
                    case ProviderType.AuthenticationType.Amazon:
                        {
                            JObject s3ConfigJSON = JObject.Parse(secret);
                            AmazonS3Config s3Config = AmazonS3Config.FromJSON(s3ConfigJSON);

                            Dictionary<string, string> parameters = s3Config.ToDictionary();
                            parameters.Add("ProviderKey", ProviderKey);
                            CloudStorageProviderBase provider = CloudStorageProviderBase.Create(
                                App.AppLogger.Logger,
                                parameters);
                            try
                            {
                                CloudProviderResponse<CloudStorageProviderUserBase> accountUserResponse = await provider.GetAccountUser();
                                _credentialError = !(accountUserResponse.ResponseValue == CloudProviderResponse<CloudStorageProviderUserBase>.Response.Success);
                                _username = String.Empty;
                            }
                            catch (Exception)
                            {
                                _credentialError = true;
                            }
                            break;
                        }
                }
            }
            NotifyPropertyChanged("CredentialError");
            NotifyPropertyChanged("UserName");
            return (_credentialError);
        }

        public static CloudProvider FromJSON(JObject json)
        {
            string id = json["ID"].Value<string>();
            ProviderType.AuthenticationType authType = (ProviderType.AuthenticationType)Enum.Parse(typeof(ProviderType.AuthenticationType), json["AuthType"].Value<string>(), true);
            string providerKey = json["ProviderKey"].Value<string>();
            return (new CloudProvider(
                authType,
                id,
                providerKey));
        }

        public JObject ToJSON()
        {
            JObject json = new JObject
            {
                { "AuthType", new JValue(AuthType.ToString()) },
                { "ID", new JValue(ID) },
                { "ProviderKey", new JValue(ProviderKey) }
            };
            return (json);
        }

        public bool CheckIsInUse()
        {
            IEnumerable<VaultIndex> matches = VaultIndexFile.Instance.Indexes.Where(vi => vi.Provider == ID);
            return (matches.Any());
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary.Add("AuthType", AuthType);
            dictionary.Add("ID", ID);
            dictionary.Add("ProviderKey", ProviderKey);
            return (dictionary);
        }

        #endregion

        #region base class overrides

        public override string ToString()
        {
            return (String.Format("{0} ({1})", ProviderKey, UserName));
        }

        #endregion

        #region icloneable

        public object Clone()
        {
            CloudProvider clone = new CloudProvider(
                AuthType,
                ProviderKey);
            return (clone);
        }

        #endregion

    }

}

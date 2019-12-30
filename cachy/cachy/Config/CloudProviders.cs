using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace cachy.Config
{

    public class CloudProviders
    {

        #region private objects

        private static CloudProviders _instance;

        private string _fullPath;
        ObservableCollection<CloudProvider> _providers;

        #endregion

        #region public properties

        public static CloudProviders Instance
        {
            get
            {
                if (_instance == null)
                {
                    String appDataPath = String.Empty;
                    Directory.ResolvePath("{AppData}", out appDataPath);
                    if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
                    String indexFullPath = String.Format("{0}Config{1}{2}", appDataPath, DLoggerManager.PathDelimiter, "cloudproviders.json");
                    _instance = FromFile(indexFullPath);
                }
                return (_instance);
            }
        }

        public ObservableCollection<CloudProvider> Providers
        {
            get
            {
                return (_providers);
            }
        }

        #endregion

        #region constructor / destructor

        private CloudProviders()
        {
            _providers = new ObservableCollection<CloudProvider>();
        }

        #endregion

        #region public methods

        public static CloudProviders FromFile(string fullPath)
        {
            CloudProviders cloudProviders = new CloudProviders();
            cloudProviders._fullPath = fullPath;
            if (System.IO.File.Exists(cloudProviders._fullPath))
            {
                String providersData = System.IO.File.ReadAllText(fullPath);
                JObject providersJSON = JObject.Parse(providersData);
                JArray providers = providersJSON["Providers"].Value<JArray>();
                foreach (JObject curProvider in providers)
                {
                    CloudProvider provider = CloudProvider.FromJSON(curProvider);
                    cloudProviders._providers.Add(provider);
                }
            }
            return (cloudProviders);
        }

        public void Save()
        {
            JObject providersJSON = new JObject();
            JArray providers = new JArray();
            foreach (CloudProvider curProvider in Providers)
            {
                providers.Add(curProvider.ToJSON());
            }
            providersJSON.Add("Providers", providers);
            System.IO.File.WriteAllText(_fullPath, providersJSON.ToString());
        }

        public async Task<bool> UpdateProvider(
            string id,
            string username,
            string accessToken)
        {
            IEnumerable<CloudProvider> matches = _providers.Where(cp => cp.ID == id).ToArray();
            if (matches.Any())
            {
                CloudProvider provider = matches.First();
                devoctomy.cachy.Framework.Native.Native.PasswordVault.StoreCredential(
                    provider.ID, 
                    username, 
                    accessToken, 
                    true);
                await provider.CheckCredentialAccessAsync();
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public CloudProvider AddProvider(
            ProviderType.AuthenticationType authType,
            string providerKey,
            string username,
            string secret,
            bool replaceExisting,
            string forceID = "")
        {
            if(replaceExisting)
            {
                IEnumerable<CloudProvider> matches = Providers.Where(cp => cp.ProviderKey == providerKey && cp.UserName == username);
                if(matches.Any())
                {
                    foreach(CloudProvider curMatch in matches)
                    {
                        RemoveProvider(curMatch.ID);
                    }
                }
            }
            CloudProvider provider;
            if(!String.IsNullOrEmpty(forceID))
            {
                CloudProvider[] matches = _providers.Where(cp => cp.ID == forceID).ToArray();
                if(matches.Length > 0)
                {
                    foreach(CloudProvider curProvider in matches)
                    {
                        _providers.Remove(curProvider);
                    }
                }
                provider = new CloudProvider(
                    authType,
                    forceID,
                    providerKey);
            }
            else
            {
                provider = new CloudProvider(authType, providerKey);
            }
            devoctomy.cachy.Framework.Native.Native.PasswordVault.StoreCredential(provider.ID, username, secret, true);
            _providers.Add(provider);
            Save();
            return (provider);
        }

        public bool RemoveProvider(string id)
        {
            IEnumerable<CloudProvider> matches = Providers.Where(cp => cp.ID == id);
            if(matches.Any())
            {
                Providers.Remove(matches.First());
                Save();
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public bool ProviderAlreadyConfigured(
            string providerKey,
            string username)
        {
            IEnumerable<CloudProvider> matches = Providers.Where(cp => cp.ProviderKey == providerKey && cp.UserName == username);
            return (matches.Any());
        }

        public void DeselectAll()
        {
            foreach (CloudProvider curProvider in Providers)
            {
                curProvider.IsSelected = false;
            }
        }

        #endregion

    }

}

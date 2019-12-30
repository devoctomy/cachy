using devoctomy.cachy.Framework.Data.Cloud;
using devoctomy.cachy.Framework.Serialisers.Interfaces;
using devoctomy.DFramework.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using static devoctomy.cachy.Framework.Data.Common;

namespace devoctomy.cachy.Framework.Data
{

    public class VaultIndex : INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private string _id = String.Empty;
        private string _name = String.Empty;
        private string _description = String.Empty;
        private SyncMode _syncMode = SyncMode.None;
        private string _provider = String.Empty;
        private string _fullPath = String.Empty;
        private string _cloudProviderPath = String.Empty;
        private DateTime? _lastModified;
        private CloudProviderSyncStatus _syncStatus;
        private bool _unopened;

        #endregion

        #region public properties

        public string ID
        {
            get
            {
                return (_id);
            }
        }

        public string Name
        {
            get
            {
                return (Unopened ? "Unopened" : _name);
            }
        }

        public string Description
        {
            get
            {
                return (Unopened ? "Open this vault to update the name and description." : _description);
            }
        }

        public SyncMode SyncMode
        {
            get
            {
                return (_syncMode);
            }
        }

        public string Provider
        {
            get
            {
                return (_provider);
            }
        }

        public string FullPath
        {
            get
            {
                return (_fullPath);
            }
        }

        public string CloudProviderPath
        {
            get
            {
                return (_cloudProviderPath);
            }
        }

        public DateTime? LastModified
        {
            get
            {
                return (_lastModified);
            }
        }

        public bool IsInLocalVaultStore
        {
            get
            {
                string appDataPath = String.Empty;
                devoctomy.DFramework.Core.IO.Directory.ResolvePath("{AppData}", out appDataPath);
                if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
                string localVaultStoreFullPath = String.Format("{0}{1}", appDataPath, String.Format(@"LocalVaults{0}", DLoggerManager.PathDelimiter));
                return (FullPath.ToLower().StartsWith(localVaultStoreFullPath.ToLower()));
            }
        }

        public CloudProviderSyncStatus SyncStatus
        {
            get
            {
                if(_syncStatus == null)
                {
                    _syncStatus = new CloudProviderSyncStatus(this);
                    _syncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.Unchecked);
                }
                return (_syncStatus);
            }
        }

        public bool IsCloudSynced
        {
            get
            {
                return (SyncMode == SyncMode.CloudProvider);
            }
        }

        public bool HasSyncStatusInformation
        {
            get
            {
                return (!String.IsNullOrEmpty(SyncStatus.Message));
            }
        }

        public bool Unopened
        {
            get
            {
                return (_unopened);
            }
        }

        #endregion

        #region constructor / destructor

        internal VaultIndex(string id,
            string name,
            string description,
            SyncMode syncMode,
            string provider,
            string fullPath,
            string cloudProviderPath,
            bool unopened)
        {
            _id = id;
            _name = name;
            _description = description;
            _syncMode = syncMode;
            _provider = provider;
            _fullPath = fullPath;
            _cloudProviderPath = cloudProviderPath;
            _unopened = unopened;

            UpdateLastModified();
        }

        #endregion

        #region private methods

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region public methods

        public static VaultIndex Prepare(string fullPath)
        {
            VaultIndex index = new VaultIndex(Guid.NewGuid().ToString(),
                String.Empty,
                String.Empty,
                SyncMode.LocalOnly,
                String.Empty,
                fullPath,
                String.Empty,
                true);
            return (index);
        }

        public async Task<GenericResult<Common.LoadResult, Vault>> Load(
            ISerialiser serialiser,
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            bool exists = await Native.Native.FileHandler.Exists(FullPath);
            if (exists)
            {
                Byte[] encrypted = await Native.Native.FileHandler.ReadAsync(FullPath);
                Vault vault = (Vault)serialiser.Read(encrypted, masterPassphrase, parameters);
                Common.LoadResult result = vault != null ? Common.LoadResult.Success : Common.LoadResult.UnknownError;
                if(result == Common.LoadResult.Success)
                {
                    vault.SetSaveParams(serialiser, 
                        FullPath, 
                        masterPassphrase,
                        false);
                    vault.SetLoaded();
                }
                return (new GenericResult<Common.LoadResult, Vault>(result, vault));
            }
            else
            {
                return (new GenericResult<Common.LoadResult, Vault>(Common.LoadResult.FileNotFound, null));
            }
        }

        public void Rename(string name,
            string description)
        {
            _name = name;
            _description = description;
        }

        public static VaultIndex FromJSON(JObject json)
        {
            string id = json["ID"].Value<string>();
            string name = json["Name"].Value<string>();
            string description = json["Description"].Value<string>();
            string syncMode = json["SyncMode"].Value<string>();
            string provider = json["Provider"].Value<string>();
            string fullPath = json["FullPath"].Value<string>();
            string cloudProviderPath = json["CloudProviderPath"].Value<string>();
            bool unopened = json["Unopened"].Value<bool>();
            VaultIndex index = new VaultIndex(id,
                name,
                description,
                (SyncMode)Enum.Parse(typeof(SyncMode), syncMode),
                provider,
                fullPath,
                cloudProviderPath,
                unopened);
            return (index);
        }

        public JObject ToJSON()
        {
            JObject json = new JObject
            {
                { "ID", new JValue(ID) },
                { "Name", new JValue(Name) },
                { "Description", new JValue(Description) },
                { "SyncMode", new JValue(SyncMode.ToString()) },
                { "Provider", new JValue(Provider) },
                { "FullPath", new JValue(FullPath) },
                { "CloudProviderPath", new JValue(CloudProviderPath) },
                { "Unopened", new JValue(Unopened) }
            };
            return (json);
        }

        [Obsolete("Needs updating")]
        public void UpdateLastModified()
        {
            if(!String.IsNullOrEmpty(FullPath))
            {                
                FileInfo vaultFile = new FileInfo(FullPath);
                if (vaultFile.Exists)
                {
                    _lastModified = vaultFile.LastWriteTimeUtc;
                    NotifyPropertyChanged("LastModifiedAtAsString");
                }
            }
        }

        public void MarkAsOpened(
            string name,
            string description)
        {
            _name = name;
            _description = description;
            _unopened = false;
        }

        #endregion

        #region base class overrides

        public override string ToString()
        {
            return (String.Format("{0}{1}", Name, !String.IsNullOrEmpty(Description) ? String.Format(" ({0})", Description) : String.Empty));
        }

        #endregion

    }

}

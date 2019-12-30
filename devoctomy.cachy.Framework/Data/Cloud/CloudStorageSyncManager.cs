using devoctomy.DFramework.Logging;
using devoctomy.DFramework.Logging.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class CloudStorageSyncManager
    {

        #region public events

        public event EventHandler<EventArgs> SyncStatusUpdated;
        public event EventHandler<EventArgs> SyncComplete;

        #endregion

        #region private objects

        private static CloudStorageSyncManager _instance;
        private IDLogger _logger;
        private Dictionary<VaultIndex, CloudStorageProviderBase> _providers;
        private Func<string, string, object> _getProviderValueCallback;
        private Func<string, string> _getCredentialCallback;

        #endregion

        #region public properties

        public static CloudStorageSyncManager Instance
        {
            get
            {
                return (_instance);
            }
        }

        public ObservableCollection<VaultIndex> VaultIndexes
        {
            get
            {
                return (VaultIndexFile.Instance.Indexes);
            }
        }

        #endregion

        #region constructor / destructor

        public CloudStorageSyncManager(IDLogger logger,
            Func<string, string, object> getProviderValueCallback,
            Func<string, string> getCredentialCallback)
        {
            DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "Creating instance of CloudStorageSyncManager.");

            _logger = logger;
            _getProviderValueCallback = getProviderValueCallback;
            _getCredentialCallback = getCredentialCallback;
            _providers = new Dictionary<VaultIndex, CloudStorageProviderBase>();
        }

        #endregion

        #region private methods

        private Native.IO.Common.HashStyle GetHashStyleFromProviderKey(string providerKey)
        {
            switch(providerKey)
            {
                case "Dropbox":
                    {
                        return Native.IO.Common.HashStyle.DropboxSHA256;
                    }
                case "OneDrive":
                    {
                        return Native.IO.Common.HashStyle.OneDriveSHA1;
                    }
                case "AmazonS3":
                    {
                        return Native.IO.Common.HashStyle.AmazonS3MD5;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        #endregion

        #region public methods

        public static void Initialise(IDLogger logger,
            Func<string, string, object> getProviderValueCallback,
            Func<string, string> getCredentialCallback)
        {
            DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "Initialising static instance of CloudStorageSyncManager.");

            _instance = new CloudStorageSyncManager(
                logger,
                getProviderValueCallback,
                getCredentialCallback);
        }

        public async Task UpdateAllVaultsSyncStatus(
            params VaultIndex[] exclude)
        {
            List<VaultIndex> excludeVaults = new List<VaultIndex>(exclude);
            IEnumerable<VaultIndex> matches = VaultIndexes.Where(vi => vi.SyncMode == Common.SyncMode.CloudProvider && !excludeVaults.Contains(vi));
            VaultIndex[] allCloudSyncVaults = matches.Any() ? matches.ToArray() : null;
            if(allCloudSyncVaults != null)
            {
                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "CloudStorageSyncManager Updating all vaults sync status.");

                foreach (VaultIndex curIndex in allCloudSyncVaults)
                {
                    await UpdateVaultSyncStatus(curIndex);
                    SyncStatusUpdated?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "CloudStorageSyncManager No vaults configured for cloud synchronising, so skipping UpdateAllVaultsSyncStatus.");
            }
        }

        public async Task UpdateVaultSyncStatus(VaultIndex index)
        {
            if(index.SyncMode == Common.SyncMode.CloudProvider)
            {
                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "CloudStorageSyncManager Updating vault sync status of vault '{0}'.", index.ID);

                CloudStorageProviderBase storageProvider = null;
                if (_providers.ContainsKey(index))
                {
                    storageProvider = _providers[index];
                }
                else
                {
                    string authType = _getProviderValueCallback(index.Provider, "AuthType").ToString();
                    switch(authType)
                    {
                        case "OAuth":
                            {
                                string providerKey = (string)_getProviderValueCallback(index.Provider, "ProviderKey");
                                string accessToken = _getCredentialCallback(index.Provider);
                                if (!String.IsNullOrEmpty(accessToken))
                                {
                                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                                    parameters.Add("AuthType", "OAuth");
                                    parameters.Add("ProviderKey", providerKey);
                                    parameters.Add("AccessToken", accessToken);
                                    storageProvider = CloudStorageProviderBase.Create(
                                        _logger,
                                        index,
                                        parameters);
                                    _providers.Add(index, storageProvider);
                                }
                                else
                                {
                                    index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.AuthenticationError, "The OAuth credentials for the associated cloud provider could not be retrieved. This may have be caused by uninstalling / reinstalling cachy. To fix this, re-authenticate with the cloud provider through the application settings 'SYNC' menu.");
                                    return;
                                }
                                break;
                            }
                        case "Amazon":
                            {
                                string providerKey = (string)_getProviderValueCallback(index.Provider, "ProviderKey");
                                string secret = _getCredentialCallback(index.Provider);
                                if (!String.IsNullOrEmpty(secret))
                                {
                                    JObject s3ConfigJSON = JObject.Parse(secret);
                                    AmazonS3Config s3Config = AmazonS3Config.FromJSON(s3ConfigJSON);
                                    Dictionary<string, string> createParams = s3Config.ToDictionary();
                                    createParams.Add("ProviderKey", providerKey);
                                    storageProvider = CloudStorageProviderBase.Create(
                                        DLoggerManager.Instance.Logger,
                                        createParams);
                                    _providers.Add(index, storageProvider);
                                }
                                else
                                {
                                    index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.AuthenticationError, "The OAuth credentials for the associated cloud provider could not be retrieved. This may have be caused by uninstalling / reinstalling cachy. To fix this, re-authenticate with the cloud provider through the application settings 'SYNC' menu.");
                                    return;
                                }
                                break;
                            }
                    }
                }

                CloudProviderResponse<CloudStorageProviderFileBase> fileResponse = await storageProvider.GetFileInfo(index.CloudProviderPath);
                switch (fileResponse.ResponseValue)
                {
                    case CloudProviderResponse<CloudStorageProviderFileBase>.Response.Success:
                        {
                            //does our local file exist?
                            if (System.IO.File.Exists(index.FullPath))
                            {
                                if (!index.LastModified.HasValue)
                                {
                                    index.UpdateLastModified();
                                    if (!index.LastModified.HasValue)
                                    {
                                        DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "CloudStorageSyncManager Uknown error for vault '{0}'.", index.ID);
                                        index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.UnknownError);
                                        break;
                                    }
                                }

                                CloudStorageProviderFileBase remote = fileResponse.Result;

                                string currenthash = await Native.Native.FileHandler.HashFileAsync(
                                    GetHashStyleFromProviderKey(storageProvider.TypeName),
                                    index.FullPath);
                                bool hashesMatch = currenthash == remote.Hash;
                                if (hashesMatch)
                                {
                                    DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseHigh | LoggerMessageType.Information, "CloudStorageSyncManager Vault '{0}' is up to date.", index.ID);
                                    index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.UpToDate);
                                }
                                else
                                {
                                    //These dates will *never* match
                                    index.SyncStatus.SetStatus(remote.LastModified > index.LastModified ?
                                        CloudProviderSyncStatus.SyncStatus.CloudCopyNewer :
                                        CloudProviderSyncStatus.SyncStatus.LocalCopyNewer);
                                }
                            }
                            else
                            {
                                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Warning, "CloudStorageSyncManager Local copy does not exist for vault '{0}'.", index.ID);
                                index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.NoLocalCopyExists);
                            }

                            break;
                        }
                    case CloudProviderResponse<CloudStorageProviderFileBase>.Response.NotFound:
                        {
                            DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Warning, "CloudStorageSyncManager File not found in cloud for vault '{0}', provider type '{1}'.", index.ID, storageProvider.TypeName);
                            index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.NoCloudCopyExists);
                            break;
                        }
                    case CloudProviderResponse<CloudStorageProviderFileBase>.Response.AuthenticationError:
                        {
                            DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Error, "CloudStorageSyncManager Authentication error for cloud provider type '{0}'.", storageProvider.TypeName);
                            index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.AuthenticationError, "The OAuth credentials for the associated cloud provider are invalid. To fix this, re-authenticate with the cloud provider through the application settings 'SYNC' menu.");
                            break;
                        }
                    case CloudProviderResponse<CloudStorageProviderFileBase>.Response.UnknownError:
                        {
                            DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Error, "CloudStorageSyncManager Unknown error for cloud provider type '{0}'.", storageProvider.TypeName);
                            index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.UnknownError);
                            break;
                        }
                }
            }
        }

        public async Task SyncAllVaults(
            params VaultIndex[] exclude)
        {
            List<VaultIndex> excludeVaults = new List<VaultIndex>(exclude);
            IEnumerable<VaultIndex> matches = VaultIndexes.Where(vi => vi.SyncMode == Common.SyncMode.CloudProvider && !excludeVaults.Contains(vi));
            VaultIndex[] allCloudSyncVaults = matches.Any() ? matches.ToArray() : null;
            if(allCloudSyncVaults != null)
            {
                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "CloudStorageSyncManager Synchronising all cloud sync vaults.");

                foreach (VaultIndex curIndex in allCloudSyncVaults)
                {
                    if(curIndex.SyncStatus.Status != CloudProviderSyncStatus.SyncStatus.UpToDate)
                    {
                        await SyncVault(curIndex);
                        SyncComplete?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            else
            {
                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "CloudStorageSyncManager No vaults configured for cloud synchronising, so skipping SyncAllVaults.");
            }
        }

        public async Task SyncVault(VaultIndex index)
        {
            if (index.SyncMode == Common.SyncMode.CloudProvider)
            {
                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "CloudStorageSyncManager Synchronising vault '{0}'.", index.ID);

                CloudStorageProviderBase provider = null;
                if (_providers.ContainsKey(index))
                {
                    provider = _providers[index];
                }
                else
                {
                    return;
                }
                try
                {
                    switch (index.SyncStatus.Status)
                    {
                        case CloudProviderSyncStatus.SyncStatus.NoLocalCopyExists:
                            {
                                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "Vault sync status is 'NoLocalCopyExists', so downloading cloud copy.");

                                CloudProviderResponse<byte[]> getFileResponse = await provider.GetFileInMemory(index.CloudProviderPath);
                                await Native.Native.FileHandler.WriteAsync(
                                    index.FullPath,
                                    getFileResponse.Result);

                                index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.UpToDate);
                                break;
                            }
                        case CloudProviderSyncStatus.SyncStatus.CloudCopyNewer:
                            {
                                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "Vault sync status is 'CloudCopyNewer', so downloading cloud copy.");

                                CloudProviderResponse<byte[]> gutFileResponse = await provider.GetFileInMemory(index.CloudProviderPath);
                                await Native.Native.FileHandler.WriteAsync(
                                    index.FullPath,
                                    gutFileResponse.Result);
                                index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.UpToDate);
                                break;
                            }
                        case CloudProviderSyncStatus.SyncStatus.LocalCopyNewer:
                            {
                                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "Vault sync status is 'LocalCopyNewer', so uploading local copy.");

                                byte[] fileData = await Native.Native.FileHandler.ReadAsync(index.FullPath);
                                CloudProviderResponse<bool> putFileResponse = await provider.PutFile(
                                    fileData,
                                    index.CloudProviderPath,
                                    true);
                                index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.UpToDate);
                                break;
                            }
                        case CloudProviderSyncStatus.SyncStatus.NoCloudCopyExists:
                            {
                                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "Vault sync status is 'NoCloudCopyExists', so uploading local copy.");

                                byte[] fileData = await Native.Native.FileHandler.ReadAsync(index.FullPath);
                                CloudProviderResponse<bool> putFileResponse = await provider.PutFile(
                                    fileData,
                                    index.CloudProviderPath,
                                    false);
                                index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.UpToDate);
                                break;
                            }
                        case CloudProviderSyncStatus.SyncStatus.UpToDate:
                            {
                                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseHigh | LoggerMessageType.Information, "CloudStorageSyncManager Vault '{0}' is up to date.", index.ID);
                                break;
                            }
                        default:
                            {
                                DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Error, "CloudStorageSyncManager Unknown cloud sync status of '{0}'.", index.SyncStatus.Status);
                                break;
                            }
                    }
                }
                catch(Exception ex)
                {
                    DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Error, "CloudStorageSyncManager Unknown error whilst synchronising vault '{0}'.", index.ID);
                    index.SyncStatus.SetStatus(CloudProviderSyncStatus.SyncStatus.UnknownError);
                }
            }
        }

        #endregion

    }

}

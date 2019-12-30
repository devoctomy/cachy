using devoctomy.DFramework.Logging;
using devoctomy.DFramework.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class CloudStorageProviderBase
    {

        #region private objects

        private IDLogger _logger;

        #endregion

        #region public properties

        public virtual string AuthType
        {
            get
            {
                return ("None");
            }
        }

        public virtual string TypeName
        {
            get
            {
                return ("Base");
            }
        }

        public IDLogger Logger
        {
            get
            {
                return (_logger);
            }
        }

        #endregion

        #region constructor / destructor

        protected CloudStorageProviderBase(IDLogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region public methods

        public static CloudStorageProviderBase Create(
            IDLogger logger,
            VaultIndex vaultIndex,
            Dictionary<string, string> parameters)
        {
            if(vaultIndex.SyncMode == Common.SyncMode.CloudProvider)
            {
                string providerKey = parameters["ProviderKey"];
                switch (providerKey)
                {
                    case "Dropbox":
                        {
                            string accessToken = parameters["AccessToken"];
                            return (CreateOAuth<DropboxStorageProvider>(
                                logger, 
                                accessToken));
                        }
                    case "OneDrive":
                        {
                            string accessToken = parameters["AccessToken"];
                            return (CreateOAuth<OneDriveStorageProvider>(
                                logger, 
                                accessToken));
                        }
                    case "AmazonS3":
                        {
                            string accessID = parameters["AccessID"];
                            string secretKey = parameters["SecretKey"];
                            string region = parameters["Region"];
                            string bucketName = parameters["BucketName"];
                            string path = parameters["Path"];
                            return (CreateAmazon<AmazonS3StorageProvider>(
                                logger,
                                accessID,
                                secretKey,
                                region,
                                bucketName,
                                path));
                        }
                    default:
                        {
                            throw new NotSupportedException(String.Format("Sync OAuth provider of type '{0}' not supported.", providerKey));
                        }
                }
            }
            else
            {
                return (null);
            }
        }

        public static CloudStorageProviderBase Create(
            IDLogger logger,
            Dictionary<string, string> parameters)
        {
            string providerKey = parameters["ProviderKey"];
            switch (providerKey)
            {
                case "Dropbox":
                    {
                        string accessToken = parameters["AccessToken"];
                        return (CreateOAuth<DropboxStorageProvider>(
                            logger, 
                            accessToken));
                    }
                case "OneDrive":
                    {
                        string accessToken = parameters["AccessToken"];
                        return (CreateOAuth<OneDriveStorageProvider>(
                            logger, 
                            accessToken));
                    }
                case "AmazonS3":
                    {
                        string accessID = parameters["AccessID"];
                        string secretKey = parameters["SecretKey"];
                        string region = parameters["Region"];
                        string bucketName = parameters["BucketName"];
                        string path = parameters["Path"];
                        return (CreateAmazon<AmazonS3StorageProvider>(
                            logger, 
                            accessID,
                            secretKey,
                            region,
                            bucketName,
                            path));
                    }
                default:
                    {
                        throw new NotSupportedException(String.Format("Sync provider of type '{0}' not supported.", providerKey));
                    }
            }
        }

        public static CloudStorageProviderBase CreateOAuth<T>(IDLogger logger,
            string accessToken) where T : CloudStorageProviderBase
        {
            DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "Creating CloudStorageProviderBase instance of type '{0}'.", typeof(T).Name);

            T instance = (T)Activator.CreateInstance(typeof(T), logger, accessToken);
            return (instance);
        }

        public static CloudStorageProviderBase CreateAmazon<T>(IDLogger logger,
            string accessID,
            string secretKey,
            string region,
            string bucketName,
            string path) where T : CloudStorageProviderBase
        {
            DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "Creating CloudStorageProviderBase instance of type '{0}'.", typeof(T).Name);

            AmazonS3Config config = new AmazonS3Config(
                accessID,
                secretKey,
                region,
                bucketName,
                path);
            T instance = (T)Activator.CreateInstance(typeof(T), logger, config);
            return (instance);
        }

        public virtual async Task<CloudProviderResponse<CloudStorageProviderUserBase>> GetAccountUser()
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        public virtual async Task<CloudProviderResponse<CloudStorageProviderFileBase>> GetFileInfo(string path)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        public virtual async Task<CloudProviderResponse<bool>> PutFile(
            byte[] data,
            string path,
            bool overwrite)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        public virtual async Task<CloudProviderResponse<byte[]>> GetFileInMemory(string path)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        public virtual async Task<CloudProviderResponse<List<CloudStorageProviderFileBase>>> ListFiles()
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        #endregion

    }
}

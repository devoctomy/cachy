using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using devoctomy.DFramework.Logging.Interfaces;
using Dropbox.Api;
using Dropbox.Api.FileProperties;
using Dropbox.Api.Files;
using Dropbox.Api.Stone;
using Dropbox.Api.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class AmazonS3StorageProvider : CloudStorageProviderBase
    {

        #region private objects

        private AmazonS3Client _s3Client;
        private AmazonS3Config _config;

        #endregion

        #region public properties

        public override string AuthType
        {
            get
            {
                return ("Amazon");
            }
        }

        public override string TypeName
        {
            get
            {
                return ("AmazonS3");
            }
        }

        #endregion

        #region constructor / destructor

        public AmazonS3StorageProvider(IDLogger logger,
            AmazonS3Config config) : base(logger)
        {
            _config = config;
            RegionEndpoint regionEndpoint = AmazonS3Regions.GetRegionByDisplayName(config.Region);
            _s3Client = new AmazonS3Client(
                config.AccessID,
                config.SecretKey,
                regionEndpoint);
        }

        #endregion

        #region public methods

        public override async Task<CloudProviderResponse<CloudStorageProviderUserBase>> GetAccountUser()
        {

            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "AmazonS3StorageProvider GetAccountUser.");

                await Task.Yield();
                AmazonS3StorageProviderUser user = new AmazonS3StorageProviderUser(
                    _config.AccessID,
                    "N/A",
                    String.Empty);
                return (new CloudProviderResponse<CloudStorageProviderUserBase>(user));
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting to get account info from AmazonS3StorageProvider. {1}", ex.Message);
                return (new CloudProviderResponse<CloudStorageProviderUserBase>(CloudProviderResponse<CloudStorageProviderUserBase>.Response.UnknownError, ex));
            }
        }

        public override async Task<CloudProviderResponse<CloudStorageProviderFileBase>> GetFileInfo(string path)
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "AmazonS3StorageProvider GetFileInfo '{0}'.", path);

                string fullPath = String.Format("{0}{1}", _config.Path, path);
                GetObjectMetadataResponse response = await _s3Client.GetObjectMetadataAsync(
                    _config.BucketName,
                    fullPath);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return (new CloudProviderResponse<CloudStorageProviderFileBase>(
                        new CloudStorageProviderFileBase(
                            path, 
                            false, 
                            fullPath, 
                            response.ETag.ToUpper(), 
                            response.LastModified)));
                }
                else
                {
                    throw new Exception(String.Format("S3 Client GetObjectMetadataAsync returned status code '{0}'.", response.HttpStatusCode));
                }
            }
            catch(AmazonS3Exception as3ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An error occurred whilst attempting to get file info for '{0}' from AmazonS3StorageProvider. {1}", path, as3ex.Message);
                if (as3ex.ErrorCode == "NotFound")
                {
                    return (new CloudProviderResponse<CloudStorageProviderFileBase>(CloudProviderResponse<CloudStorageProviderFileBase>.Response.NotFound, as3ex));
                }
                else
                {
                    return (new CloudProviderResponse<CloudStorageProviderFileBase>(CloudProviderResponse<CloudStorageProviderFileBase>.Response.UnknownError, as3ex));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting to get file info for '{0}' from AmazonS3StorageProvider. {1}", path, ex.Message);
                return (new CloudProviderResponse<CloudStorageProviderFileBase>(CloudProviderResponse<CloudStorageProviderFileBase>.Response.UnknownError, ex));
            }
        }

        public override async Task<CloudProviderResponse<bool>> PutFile(
            byte[] data,
            string path,
            bool overwrite)
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "AmazonS3StorageProvider PutFile '{0}'.", path);

                string fullPath = String.Format("{0}{1}", _config.Path, path);
                PutObjectRequest putRequest = new PutObjectRequest()
                {
                    AutoCloseStream = true,
                    AutoResetStreamPosition = true,
                    InputStream = new MemoryStream(data),
                    Key = fullPath,
                    BucketName = _config.BucketName                   
                };
                PutObjectResponse response = await _s3Client.PutObjectAsync(putRequest);
                return (new CloudProviderResponse<bool>(response.HttpStatusCode == System.Net.HttpStatusCode.OK));
            }
            catch (AuthException ae)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An authorisation error occurred whilst attempting to put file '{0}' to AmazonS3StorageProvider. {1}", path, ae.Message);
                return (new CloudProviderResponse<bool>(CloudProviderResponse<bool>.Response.AuthenticationError, ae));
            }
            catch (DropboxException de)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An error occurred whilst attempting to put file '{0}' to AmazonS3StorageProvider. {1}", path, de.Message);
                return (new CloudProviderResponse<bool>(CloudProviderResponse<bool>.Response.UnknownError, de));
            }
            catch(Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting put file '{0}' to AmazonS3StorageProvider. {1}", path, ex.Message);
                return (new CloudProviderResponse<bool>(CloudProviderResponse<bool>.Response.UnknownError, ex));
            }
        }

        public override async Task<CloudProviderResponse<byte[]>> GetFileInMemory(string path)
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "AmazonS3StorageProvider PutFile '{0}'.", path);

                string fullPath = String.Format("{0}{1}", _config.Path, path);
                GetObjectResponse response = await _s3Client.GetObjectAsync(
                    _config.BucketName,
                    fullPath);
                if(response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await response.ResponseStream.CopyToAsync(memoryStream);
                        return(new CloudProviderResponse<byte[]>(memoryStream.ToArray()));
                    }
                }
                else
                {
                    throw new Exception(String.Format("S3 Client GetObjectAsync returned status code '{0}'.", response.HttpStatusCode));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting to get file '{0}' from AmazonS3StorageProvider. {1}", path, ex.Message);
                return (new CloudProviderResponse<byte[]>(CloudProviderResponse<byte[]>.Response.UnknownError, ex));
            }
        }

        public override async Task<CloudProviderResponse<List<CloudStorageProviderFileBase>>> ListFiles()
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "AmazonS3StorageProvider ListFiles.");

                ListObjectsResponse listResponse = await _s3Client.ListObjectsAsync(_config.BucketName,
                    _config.Path + "/");
                if (listResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<CloudStorageProviderFileBase> files = new List<CloudStorageProviderFileBase>();
                    foreach (S3Object curObject in listResponse.S3Objects)
                    {
                        bool isFolder = curObject.Key.EndsWith("/");
                        string name = String.Empty; 
                        if(isFolder)
                        {
                            string removedTrailing = curObject.Key.TrimEnd('/');
                            name = removedTrailing.Substring(removedTrailing.LastIndexOf("/") + 1);
                        }
                        else
                        {
                            name = curObject.Key.Substring(curObject.Key.LastIndexOf("/"));
                        }
                        files.Add(new AmazonS3StorageProviderFile(
                            isFolder ? name : name.TrimStart('/'), 
                            isFolder,
                            isFolder ? curObject.Key.TrimEnd('/') : curObject.Key,
                            curObject.ETag.ToUpper(), 
                            curObject.LastModified));
                    }
                    return (new CloudProviderResponse<List<CloudStorageProviderFileBase>>(files));
                }
                else
                {
                    throw new Exception(String.Format("S3 Client GetObjectAsync returned status code '{0}'.", listResponse.HttpStatusCode));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting to list files from AmazonS3StorageProvider. {0}", ex.Message);
                return (new CloudProviderResponse<List<CloudStorageProviderFileBase>>(CloudProviderResponse<List<CloudStorageProviderFileBase>>.Response.UnknownError, ex));
            }
        }

        #endregion

    }

}

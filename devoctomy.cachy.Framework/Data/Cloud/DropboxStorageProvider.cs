using devoctomy.DFramework.Logging.Interfaces;
using Dropbox.Api;
using Dropbox.Api.FileProperties;
using Dropbox.Api.Files;
using Dropbox.Api.Stone;
using Dropbox.Api.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class DropboxStorageProvider : CloudStorageProviderBase
    {

        #region private objects

        DropboxClient _client;

        #endregion

        #region public properties

        public override string AuthType
        {
            get
            {
                return ("OAuth");
            }
        }

        public override string TypeName
        {
            get
            {
                return ("Dropbox");
            }
        }

        #endregion

        #region constructor / destructor

        public DropboxStorageProvider(IDLogger logger,
            string accessToken) : base(logger)
        {
            _client = new DropboxClient(accessToken, 
                new DropboxClientConfig());
        }

        #endregion

        #region public methods

        public override async Task<CloudProviderResponse<CloudStorageProviderUserBase>> GetAccountUser()
        {

            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "DropboxStorageProvider GetAccountUser.");

                FullAccount full = await _client.Users.GetCurrentAccountAsync();
                DropboxStorageProviderUser user = DropboxStorageProviderUser.FromFullAccount(full);
                return (new CloudProviderResponse<CloudStorageProviderUserBase>(user));
            }
            catch (AuthException ae)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An authorisation error occurred whilst attempting to get account info from DropboxStorageProvider. {1}", ae.Message);
                return (new CloudProviderResponse<CloudStorageProviderUserBase>(CloudProviderResponse<CloudStorageProviderUserBase>.Response.AuthenticationError, ae));
            }
            catch (DropboxException e)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An error occurred whilst attempting to get account info from DropboxStorageProvider. {1}", e.Message);
                return (new CloudProviderResponse<CloudStorageProviderUserBase>(CloudProviderResponse<CloudStorageProviderUserBase>.Response.UnknownError, e));
            }
        }

        //Need to change this to return a response object that contains the result
        //as a property if successful
        public override async Task<CloudProviderResponse<CloudStorageProviderFileBase>> GetFileInfo(string path)
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "DropboxStorageProvider GetFileInfo '{0}'.", path);

                Metadata fileMetaData = await _client.Files.GetMetadataAsync(path,
                    false,
                    false,
                    false);
                DropboxStorageProviderFile file = DropboxStorageProviderFile.FromMetadata(fileMetaData);
                return (new CloudProviderResponse<CloudStorageProviderFileBase>(file));
            }
            catch (AuthException ae)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An authorisation error occurred whilst attempting to get file info for '{0}' from DropboxStorageProvider. {1}", path, ae.Message);
                return (new CloudProviderResponse<CloudStorageProviderFileBase>(CloudProviderResponse<CloudStorageProviderFileBase>.Response.AuthenticationError, ae));
            }
            catch (DropboxException e)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An error occurred whilst attempting to get file info for '{0}' from DropboxStorageProvider. {1}", path, e.Message);
                if(e.Message.ToLower().Contains("not_found"))
                {
                    return (new CloudProviderResponse<CloudStorageProviderFileBase>(CloudProviderResponse<CloudStorageProviderFileBase>.Response.NotFound, e));
                }
                else
                {
                    return(new CloudProviderResponse<CloudStorageProviderFileBase>(CloudProviderResponse<CloudStorageProviderFileBase>.Response.UnknownError, e));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting to get file info for '{0}' from DropboxStorageProvider. {1}", path, ex.Message);
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
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "DropboxStorageProvider PutFile '{0}'.", path);

                using (System.IO.MemoryStream dataStream = new System.IO.MemoryStream(data))
                {
                    CommitInfo commit = new CommitInfo(
                        path,
                        overwrite ? (WriteMode)WriteMode.Overwrite.Instance : (WriteMode)WriteMode.Add.Instance);
                    FileMetadata fileMetaData = await _client.Files.UploadAsync(commit, dataStream);
                    return (new CloudProviderResponse<bool>(true));
                }
            }
            catch (AuthException ae)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An authorisation error occurred whilst attempting to put file '{0}' to DropboxStorageProvider. {1}", path, ae.Message);
                return (new CloudProviderResponse<bool>(CloudProviderResponse<bool>.Response.AuthenticationError, ae));
            }
            catch (DropboxException de)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An error occurred whilst attempting to put file '{0}' to DropboxStorageProvider. {1}", path, de.Message);
                return (new CloudProviderResponse<bool>(CloudProviderResponse<bool>.Response.UnknownError, de));
            }
            catch(Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting put file '{0}' to DropboxStorageProvider. {1}", path, ex.Message);
                return (new CloudProviderResponse<bool>(CloudProviderResponse<bool>.Response.UnknownError, ex));
            }
        }

        public override async Task<CloudProviderResponse<byte[]>> GetFileInMemory(string path)
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "DropboxStorageProvider PutFile '{0}'.", path);

                //we should really download to a temporary location
                IDownloadResponse<FileMetadata> downloadResponse = await _client.Files.DownloadAsync(path);
                byte[] data = await downloadResponse.GetContentAsByteArrayAsync();
                return (new CloudProviderResponse<byte[]>(data));
            }
            catch (AuthException ae)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An authorisation error occurred whilst attempting to get file '{0}' from DropboxStorageProvider. {1}", path, ae.Message);
                return (new CloudProviderResponse<byte[]>(CloudProviderResponse<byte[]>.Response.AuthenticationError, ae));
            }
            catch (DropboxException de)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An error occurred whilst attempting to get file '{0}' from DropboxStorageProvider. {1}", path, de.Message);
                if (de.Message.ToLower().Contains("not_found"))
                {
                    return (new CloudProviderResponse<byte[]>(CloudProviderResponse<byte[]>.Response.NotFound, de));
                }
                else
                {
                    return (new CloudProviderResponse<byte[]>(CloudProviderResponse<byte[]>.Response.UnknownError, de));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting to get file '{0}' from DropboxStorageProvider. {1}", path, ex.Message);
                return (new CloudProviderResponse<byte[]>(CloudProviderResponse<byte[]>.Response.UnknownError, ex));
            }
        }

        public override async Task<CloudProviderResponse<List<CloudStorageProviderFileBase>>> ListFiles()
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "DropboxStorageProvider ListFiles.");

                List<CloudStorageProviderFileBase> files = new List<CloudStorageProviderFileBase>();
                ListFolderArg arg = new ListFolderArg(String.Empty, true);
                ListFolderResult listFolderResult = await _client.Files.ListFolderAsync(arg);
                foreach(Metadata curFile in listFolderResult.Entries)
                {
                    DropboxStorageProviderFile file = DropboxStorageProviderFile.FromMetadata(curFile);
                    if(!file.IsFolder)
                    {
                        if(file.Path.ToLower().EndsWith(".vault"))
                        {
                            files.Add(file);
                        }
                    }
                }
                return (new CloudProviderResponse<List<CloudStorageProviderFileBase>>(files));
            }
            catch (AuthException ae)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An authorisation error occurred whilst attempting to list files from DropboxStorageProvider. {0}", ae.Message);
                return (new CloudProviderResponse<List<CloudStorageProviderFileBase>>(CloudProviderResponse<List<CloudStorageProviderFileBase>>.Response.AuthenticationError, ae));
            }
            catch (DropboxException de)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An error occurred whilst attempting to list files from DropboxStorageProvider. {0}", de.Message);
                if (de.Message.ToLower().Contains("not_found"))
                {
                    return (new CloudProviderResponse<List<CloudStorageProviderFileBase>>(CloudProviderResponse<List<CloudStorageProviderFileBase>>.Response.NotFound, de));
                }
                else
                {
                    return (new CloudProviderResponse<List<CloudStorageProviderFileBase>>(CloudProviderResponse<List<CloudStorageProviderFileBase>>.Response.UnknownError, de));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting to list files from DropboxStorageProvider. {1}", ex.Message);
                return (new CloudProviderResponse<List<CloudStorageProviderFileBase>>(CloudProviderResponse<List<CloudStorageProviderFileBase>>.Response.UnknownError, ex));
            }
        }

        #endregion

    }

}

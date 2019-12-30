using devoctomy.DFramework.Logging.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class OneDriveStorageProvider : CloudStorageProviderBase
    {

        #region private objects

        private HttpClient _client;
        private string _apiBase = "https://graph.microsoft.com/v1.0";
        private string _accessToken = String.Empty;

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
                return ("OneDrive");
            }
        }

        #endregion

        #region constructor / destructor

        public OneDriveStorageProvider(IDLogger logger,
            string accessToken) : base(logger)
        {
            _accessToken = accessToken;
            _client = new HttpClient();
        }

        private async Task AuthDelegate(HttpRequestMessage request)
        {
            await Task.Yield();
            request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _accessToken);
        }

        #endregion

        #region private methods

        private async Task<bool> EnumerateAllMyAppFilesAndFolders(List<CloudStorageProviderFileBase> files)
        {
            files.Clear();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, String.Format("{0}{1}", _apiBase, "/me/drive/special/approot/children"));
            await AuthDelegate(request);
            HttpResponseMessage response = await _client.SendAsync(request);
            if(response.IsSuccessStatusCode)
            {
                return(await EnumerateFilesAndFolders("/", files, response));
            }
            else
            {
                return (false);
            }
        }

        private async Task<bool> EnumerateFilesAndFolders(
            string curPath,
            List<CloudStorageProviderFileBase> files,
            HttpResponseMessage response)
        {
            string responseString = await response.Content.ReadAsStringAsync();
            JObject responseJSON = JObject.Parse(await response.Content.ReadAsStringAsync());
            JArray items = responseJSON["value"].Value<JArray>();
            foreach(JObject curItem in items)
            {
                string id = curItem["id"].Value<string>();
                string name = curItem["name"].Value<string>();
                string curItemPath = String.Format("{0}{1}", curPath, name);
                OneDriveStorageProviderFile curFile = null;
                try
                {
                    curFile = OneDriveStorageProviderFile.FromJSON(
                        curItem,
                        curItemPath);
                }
                catch(InvalidOperationException ioex)
                {
                    Logger.Log(LoggerMessageType.Exception, "OneDriveStorageProvider experienced an issue whilst parsing file meta data from '{0}'. {1}", curItemPath, ioex.Message);
                }
                if (curFile != null)
                {
                    files.Add(curFile);
                    if (curFile.IsFolder)
                    {
                        string nextPath = String.Format("{0}{1}/", curPath, name);
                        HttpRequestMessage nextRequest = new HttpRequestMessage(HttpMethod.Get, String.Format("{0}{1}{2}/children", _apiBase, "/me/drive/items/", id));
                        await AuthDelegate(nextRequest);
                        HttpResponseMessage nextResponse = await _client.SendAsync(nextRequest);
                        if (nextResponse.IsSuccessStatusCode)
                        {
                            if (!await EnumerateFilesAndFolders(
                                nextPath,
                                files,
                                nextResponse))
                            {
                                return (false);
                            };
                        }
                        else
                        {
                            return (false);
                        }
                    }
                }
            }
            return (true);
        }

        #endregion

        #region public methods

        public override async Task<CloudProviderResponse<CloudStorageProviderUserBase>> GetAccountUser()
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "OneDriveStorageProvider GetAccountUser.");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me/");
                await AuthDelegate(request);
                using (HttpResponseMessage response = await _client.SendAsync(request))
                {
                    String meString = await response.Content.ReadAsStringAsync();
                    JObject meJSON = JObject.Parse(meString);
                    OneDriveStorageProviderUser me = OneDriveStorageProviderUser.FromJSON(meJSON);
                    return (new CloudProviderResponse<CloudStorageProviderUserBase>(me));
                }
            }
            catch (Exception e)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An error occurred whilst attempting to get account info from DropboxStorageProvider. {1}", e.Message);
                return (new CloudProviderResponse<CloudStorageProviderUserBase>(CloudProviderResponse<CloudStorageProviderUserBase>.Response.UnknownError, e));
            }
        }

        public override async Task<CloudProviderResponse<CloudStorageProviderFileBase>> GetFileInfo(string path)
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "DropboxStorageProvider GetFileInfo '{0}'.", path);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, String.Format("{0}{1}{2}", _apiBase, "/me/drive/special/approot:", path));
                await AuthDelegate(request);
                HttpResponseMessage response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string fileString = await response.Content.ReadAsStringAsync();
                    JObject file = JObject.Parse(fileString);
                    string id = file["id"].Value<string>();
                    string name = file["name"].Value<string>();
                    string curItemPath = String.Format("{0}{1}", path, name);
                    try
                    {
                        OneDriveStorageProviderFile curFile = OneDriveStorageProviderFile.FromJSON(
                            file,
                            curItemPath);
                        return (new CloudProviderResponse<CloudStorageProviderFileBase>(curFile));
                    }
                    catch (InvalidOperationException ioex)
                    {
                        Logger.Log(LoggerMessageType.Exception, "OneDriveStorageProvider experienced an issue whilst parsing file meta data from '{0}'. {1}", curItemPath, ioex.Message);
                        throw;
                    }
                }
                else
                {
                    return (new CloudProviderResponse<CloudStorageProviderFileBase>(CloudProviderResponse<CloudStorageProviderFileBase>.Response.NotFound, (Exception)null));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting to get file info for '{0}' from OneDriveStorageProvider. {1}", path, ex.Message);
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
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "OneDriveStorageProvider GetFileInMemory '{0}'.", path);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, String.Format("{0}{1}{2}", _apiBase, "/me/drive/special/approot:", path));
                await AuthDelegate(request);
                HttpResponseMessage response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string fileString = await response.Content.ReadAsStringAsync();
                    JObject file = JObject.Parse(fileString);
                    string id = file["id"].Value<string>();

                    using (MemoryStream memoryStream = new MemoryStream(data))
                    {
                        string existingFileURI = String.Format("{0}{1}{2}/content", _apiBase, "/me/drive/items/", id);
                        HttpRequestMessage putFileRequest = new HttpRequestMessage(HttpMethod.Put, existingFileURI);
                        putFileRequest.Content = new StreamContent(memoryStream);
                        putFileRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        putFileRequest.Content.Headers.ContentLength = memoryStream.Length;
                        await AuthDelegate(putFileRequest);
                        HttpResponseMessage putFileResponse = await _client.SendAsync(putFileRequest);
                        if (putFileResponse.IsSuccessStatusCode)
                        {
                            return (new CloudProviderResponse<bool>(true));
                        }
                        else
                        {
                            throw new Exception(String.Format("Failed to upload new file. Status code {0}.", putFileResponse.StatusCode));
                        }
                    }
                }
                else
                {
                    string appRootURI = String.Format("{0}{1}", _apiBase, "/me/drive/special/approot");
                    HttpRequestMessage getAppRootRequest = new HttpRequestMessage(HttpMethod.Get, appRootURI);
                    await AuthDelegate(getAppRootRequest);
                    HttpResponseMessage getAppRootResponse = await _client.SendAsync(getAppRootRequest);
                    if (getAppRootResponse.IsSuccessStatusCode)
                    {
                        string fileString = await getAppRootResponse.Content.ReadAsStringAsync();
                        JObject file = JObject.Parse(fileString);
                        string id = file["id"].Value<string>();

                        using (MemoryStream memoryStream = new MemoryStream(data))
                        {
                            string newFileURI = String.Format("{0}{1}{2}:{3}:/content", _apiBase, "/me/drive/items/", id, path);
                            HttpRequestMessage putFileRequest = new HttpRequestMessage(HttpMethod.Put, newFileURI);
                            putFileRequest.Content = new StreamContent(memoryStream);
                            putFileRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            putFileRequest.Content.Headers.ContentLength = memoryStream.Length;
                            await AuthDelegate(putFileRequest);
                            HttpResponseMessage putFileResponse = await _client.SendAsync(putFileRequest);
                            if (putFileResponse.IsSuccessStatusCode)
                            {
                                return (new CloudProviderResponse<bool>(true));
                            }
                            else
                            {
                                throw new Exception(String.Format("Failed to upload new file. Status code {0}.", putFileResponse.StatusCode));
                            }
                        }
                    }
                    else
                    {
                        throw new Exception(String.Format("Failed to get special folder approot. Status code {0}.", getAppRootResponse.StatusCode));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting to put file '{0}' to OneDriveStorageProvider. {1}", path, ex.Message);
                return (new CloudProviderResponse<bool>(CloudProviderResponse<bool>.Response.UnknownError, ex));
            }
        }

        public override async Task<CloudProviderResponse<byte[]>> GetFileInMemory(string path)
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "OneDriveStorageProvider GetFileInMemory '{0}'.", path);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, String.Format("{0}{1}{2}", _apiBase, "/me/drive/special/approot:", path));
                await AuthDelegate(request);
                HttpResponseMessage response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string fileString = await response.Content.ReadAsStringAsync();
                    JObject file = JObject.Parse(fileString);
                    string downloadURL = file["@microsoft.graph.downloadUrl"].Value<string>();

                    HttpRequestMessage downloadRequest = new HttpRequestMessage(HttpMethod.Get, downloadURL);
                    await AuthDelegate(downloadRequest);
                    HttpResponseMessage downloadResponse = await _client.SendAsync(downloadRequest);
                    if(downloadResponse.IsSuccessStatusCode)
                    {
                        MemoryStream memoryStream = new MemoryStream();
                        Stream downloadStream = await downloadResponse.Content.ReadAsStreamAsync();
                        await downloadStream.CopyToAsync(memoryStream);
                        return (new CloudProviderResponse<byte[]>(memoryStream.ToArray()));
                    }
                }
                return (new CloudProviderResponse<byte[]>(CloudProviderResponse<byte[]>.Response.NotFound, (Exception)null));
            }
            catch (Exception ex)
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "An unknown error occurred whilst attempting to get file '{0}' from OneDriveStorageProvider. {1}", path, ex.Message);
                return (new CloudProviderResponse<byte[]>(CloudProviderResponse<byte[]>.Response.UnknownError, ex));
            }
        }

        public override async Task<CloudProviderResponse<List<CloudStorageProviderFileBase>>> ListFiles()
        {
            try
            {
                Logger.Log(LoggerMessageType.Information | LoggerMessageType.VerboseHigh, "OneDriveStorageProvider ListFiles.");

                List<CloudStorageProviderFileBase> files = new List<CloudStorageProviderFileBase>();
                await EnumerateAllMyAppFilesAndFolders(files);

                return (new CloudProviderResponse<List<CloudStorageProviderFileBase>>(files));
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cachywebfunctions.Entities;
using Dropbox.Api;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cachywebfunctions
{

    public static class OAuthFunctions
    {

        [FunctionName("AuthenticateBegin")]
        public static async Task<HttpResponseMessage> AuthenticateBegin(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            string provider = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "provider", true) == 0)
                .Value;

            string requestData = await req.Content.ReadAsStringAsync();
            JObject requestJSON = JObject.Parse(requestData);
            string publicRSAKey = requestJSON["PublicRSAKey"].Value<string>();

            OAuthSessionEntity session = new OAuthSessionEntity(provider)
            {
                PublicRSAKey = publicRSAKey
            };

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient client = storageAccount.CreateCloudTableClient();

            CloudTable table = client.GetTableReference("AuthenticationSessions");
            await table.CreateIfNotExistsAsync();

            TableOperation insertOperation = TableOperation.Insert(session);
            table.Execute(insertOperation);

            return (new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    session.ToJSON(true),
                    Encoding.UTF8,
                    "application/json")
            });
        }

        [FunctionName("AuthenticateContinue")]
        public static async Task<HttpResponseMessage> AuthenticateContinue(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            string id = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
                .Value;

            string provider = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "provider", true) == 0)
                .Value;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient client = storageAccount.CreateCloudTableClient();

            CloudTable table = client.GetTableReference("AuthenticationSessions");
            await table.CreateIfNotExistsAsync();

            TableOperation retrieveOperation = TableOperation.Retrieve<OAuthSessionEntity>(provider, id);

            OAuthSessionEntity updatedSession = null;
            bool complete = false;
            while (!complete)
            {
                Thread.Sleep(1000);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                updatedSession = result.Result as OAuthSessionEntity;
                complete = updatedSession.State != "new";
            }

            //Create the response JSON and delete the session from table storage
            string retValJSON = updatedSession.ToJSON(false);
            //TableOperation removeOperation = TableOperation.Delete(updatedSession);
            //await table.ExecuteAsync(removeOperation);

            return (new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    retValJSON,
                    Encoding.UTF8,
                    "application/json")
            });
        }

        [FunctionName("OneDriveOAuthRedirect")]
        public static async Task<HttpResponseMessage> OneDriveOAuthRedirect(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            StringBuilder test = new StringBuilder();

            string state = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "state", true) == 0)
                .Value;

            string code = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "code", true) == 0)
                .Value;

            try
            {
                HttpClient httpClient = new HttpClient();
                string tokenRequestURI = "https://login.microsoftonline.com/consumers/oauth2/v2.0/token";  //"https://login.live.com/oauth20_token.srf";

                Dictionary<string, string> formParams = new Dictionary<string, string>
                {
                    { "client_id", Environment.GetEnvironmentVariable("OneDriveAppKey") },
                    { "redirect_uri", "https://cachywebfunctions20190202044830.azurewebsites.net/api/OneDriveOAuthRedirect" },
                    { "client_secret", Environment.GetEnvironmentVariable("OneDriveAppSecret") },
                    { "code", code },
                    { "grant_type", "authorization_code" }
                };

                FormUrlEncodedContent content = new FormUrlEncodedContent(formParams.ToArray());

                HttpResponseMessage response = await httpClient.PostAsync(tokenRequestURI, content);
                string tokenResponse = await response.Content.ReadAsStringAsync();

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                CloudTableClient client = storageAccount.CreateCloudTableClient();

                CloudTable table = client.GetTableReference("AuthenticationSessions");
                await table.CreateIfNotExistsAsync();

                TableOperation retrieveOperation = TableOperation.Retrieve<OAuthSessionEntity>("onedrive", state);
                TableResult retrieveResult = table.Execute(retrieveOperation);

                bool success = false;
                if (retrieveResult.HttpStatusCode == (int)HttpStatusCode.OK)
                {
                    OAuthSessionEntity session = retrieveResult.Result as OAuthSessionEntity;
                    session.Complete(tokenResponse);
                    TableOperation replaceOperation = TableOperation.Replace(session);
                    TableResult replaceResult = table.Execute(replaceOperation);
                    success = (replaceResult.HttpStatusCode >= 200) && (replaceResult.HttpStatusCode <= 299);
                }

                if (success)
                {
                    return (new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                            "Please wait while cachy authenticates with OneDrive",
                            Encoding.UTF8,
                            "text/plain")
                    });
                }
                else
                {
                    return (new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(
                            "Authentication with cachy failed.",
                            Encoding.UTF8,
                            "text/plain")
                    });
                }
            }
            catch (Exception)
            {
                //set session fail state

                return (new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        "Authentication with cachy failed.",
                        Encoding.UTF8,
                        "text/plain")
                });
            }
        }

        [FunctionName("DropboxOAuthRedirect")]
        public static async Task<HttpResponseMessage> DropboxOAuthRedirect(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            StringBuilder test = new StringBuilder();

            string state = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "state", true) == 0)
                .Value;

            string code = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "code", true) == 0)
                .Value;

            try
            {
                HttpClient httpClient = new HttpClient();
                string tokenRequestURI = String.Format("https://api.dropbox.com/oauth2/token?&grant_type={0}&code={1}&redirect_uri={2}&client_id={3}&client_secret={4}",
                    "authorization_code",
                    code,
                    "https://cachywebfunctions20190202044830.azurewebsites.net/api/DropboxOAuthRedirect",
                    Environment.GetEnvironmentVariable("DropboxAppKey"),
                    Environment.GetEnvironmentVariable("DropboxAppSecret"));

                HttpResponseMessage response = await httpClient.PostAsync(tokenRequestURI, new StringContent(""));
                string tokenResponse = await response.Content.ReadAsStringAsync();

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                CloudTableClient client = storageAccount.CreateCloudTableClient();

                CloudTable table = client.GetTableReference("AuthenticationSessions");
                await table.CreateIfNotExistsAsync();

                TableOperation retrieveOperation = TableOperation.Retrieve<OAuthSessionEntity>("dropbox", state);
                TableResult retrieveResult = table.Execute(retrieveOperation);

                bool success = false;
                if (retrieveResult.HttpStatusCode == (int)HttpStatusCode.OK)
                {
                    OAuthSessionEntity session = retrieveResult.Result as OAuthSessionEntity;
                    session.Complete(tokenResponse);
                    TableOperation replaceOperation = TableOperation.Replace(session);
                    TableResult replaceResult = table.Execute(replaceOperation);
                    success = (replaceResult.HttpStatusCode >= 200) && (replaceResult.HttpStatusCode <= 299);
                }

                if (success)
                {
                    return (new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                            "Please wait while cachy authenticates with Dropbox",
                            Encoding.UTF8,
                            "text/plain")
                    });
                }
                else
                {
                    return (new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(
                            "Authentication with cachy failed.",
                            Encoding.UTF8,
                            "text/plain")
                    });
                }
            }
            catch(Exception)
            {
                return (new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        "Authentication with cachy failed.",
                        Encoding.UTF8,
                        "text/plain")
                });
            }
        }

        [FunctionName("GoogleDriveOAuthRedirect")]
        public static async Task<HttpResponseMessage> GoogleDriveOAuthRedirect(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            StringBuilder test = new StringBuilder();

            string state = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "state", true) == 0)
                .Value;

            string code = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "code", true) == 0)
                .Value;

            try
            {
                HttpClient httpClient = new HttpClient();
                string tokenRequestURI = String.Format("https://www.googleapis.com/oauth2/v4/token?&grant_type={0}&code={1}&redirect_uri={2}&client_id={3}&client_secret={4}",
                    "authorization_code",
                    code,
                    "https://cachywebfunctions20190202044830.azurewebsites.net/api/GoogleDriveOAuthRedirect",
                    Environment.GetEnvironmentVariable("GoogleDriveAppKey"),
                    Environment.GetEnvironmentVariable("GoogleDriveAppSecret"));

                HttpResponseMessage response = await httpClient.PostAsync(tokenRequestURI, new StringContent(""));
                string tokenResponse = await response.Content.ReadAsStringAsync();

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
                CloudTableClient client = storageAccount.CreateCloudTableClient();

                CloudTable table = client.GetTableReference("AuthenticationSessions");
                await table.CreateIfNotExistsAsync();

                TableOperation retrieveOperation = TableOperation.Retrieve<OAuthSessionEntity>("googledrive", state);
                TableResult retrieveResult = table.Execute(retrieveOperation);

                bool success = false;
                if (retrieveResult.HttpStatusCode == (int)HttpStatusCode.OK)
                {
                    OAuthSessionEntity session = retrieveResult.Result as OAuthSessionEntity;
                    session.Complete(tokenResponse);
                    TableOperation replaceOperation = TableOperation.Replace(session);
                    TableResult replaceResult = table.Execute(replaceOperation);
                    success = (replaceResult.HttpStatusCode >= 200) && (replaceResult.HttpStatusCode <= 299);
                }

                if (success)
                {
                    return (new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                            "Please wait while cachy authenticates with Dropbox",
                            Encoding.UTF8,
                            "text/plain")
                    });
                }
                else
                {
                    return (new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent(
                            "Authentication with cachy failed.",
                            Encoding.UTF8,
                            "text/plain")
                    });
                }
            }
            catch (Exception)
            {
                return (new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        "Authentication with cachy failed.",
                        Encoding.UTF8,
                        "text/plain")
                });
            }
        }

    }

}

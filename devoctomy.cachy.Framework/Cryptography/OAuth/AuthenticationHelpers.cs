using devoctomy.DFramework.Core.SystemExtensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace devoctomy.cachy.Framework.Cryptography.OAuth
{

    public static class AuthenticationHelpers
    {

        public static async Task<Dictionary<string, object>> BeginOAuthAuthentication(string provider)
        {
            RSA rsa = (RSA)RSA.Create();
            string id = String.Empty;
            using (HttpClient client = new HttpClient())
            {
                RSAParameters parameters = rsa.ExportParameters(false);
                XDocument xml = new XDocument();
                XElement rsaKeyValue = new XElement("RSAKeyValue");
                rsaKeyValue.Add(new XElement("Modulus", Convert.ToBase64String(parameters.Modulus)));
                rsaKeyValue.Add(new XElement("Exponent", Convert.ToBase64String(parameters.Exponent)));
                string publicRSAKeyXML = rsaKeyValue.ToString();
                string publicRSAKeyHex = publicRSAKeyXML.ToHexString();
                JObject session = new JObject
                {
                    { "PublicRSAKey", new JValue(publicRSAKeyHex) }
                };
                string authURI = String.Format("https://cachywebfunctions20190202044830.azurewebsites.net/api/AuthenticateBegin?provider={0}", provider.ToLower());
                using (HttpResponseMessage beginSession = await client.PostAsync(authURI, new StringContent(session.ToString())))
                {
                    if(beginSession.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string beginResponseString = await beginSession.Content.ReadAsStringAsync();
                        JObject beginResponse = JObject.Parse(beginResponseString);
                        id = beginResponse["ID"].Value<string>();
                    }
                    else
                    {
                        rsa.Dispose();
                        rsa = null;
                        return (null);
                    }
                }
            }
            Dictionary<string, object> retVal = new Dictionary<string, object>
            {
                { "ProviderID", provider },
                { "Title", String.Format("Login to {0}", provider) },
                { "RSA", rsa },
                { "SessionID", id }
            };
            return (retVal);
        }

        public static async Task<string> ContinueOAuthAuthentication(
            string provider,
            string sessionID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string authURI = String.Format("https://cachywebfunctions20190202044830.azurewebsites.net/api/AuthenticateContinue?provider={0}&id={1}", provider.ToLower(), sessionID);
                    HttpResponseMessage continueSession = await client.GetAsync(authURI);
                    string continueResponseString = await continueSession.Content.ReadAsStringAsync();
                    return (continueResponseString);
                }
            }
            catch(Exception)
            {
                return (String.Empty);
            }
        }

        public static string CompleteOAuthAutentication(Dictionary<string, object> parameters)
        {
            string responseString = (string)parameters["AuthResponse"];
            JObject authResponse = JObject.Parse(responseString);
            int statusCode = (int)authResponse["statusCode"].Value<int>();
            if(statusCode == 200)
            {
                string encryptedAESKeyString = authResponse["EncryptedAESKey"].Value<string>();
                string encryptedTokenString = authResponse["EncryptedToken"].Value<string>();
                string accessToken = String.Empty;
                RSA rsa = parameters["RSA"] as RSA;
                using (rsa)
                {
                    string[] keyParts = encryptedAESKeyString.Split('.');
                    byte[] ivBytes = keyParts[0].BytesFromHexString();
                    byte[] encryptedAESKeyBytes = keyParts[1].BytesFromHexString();
                    byte[] aesKey = rsa.Decrypt(encryptedAESKeyBytes, RSAEncryptionPadding.Pkcs1);
                    using (Aes aes = Aes.Create())
                    {
                        aes.IV = ivBytes;
                        aes.Key = aesKey;
                        aes.Padding = PaddingMode.PKCS7;
                        using (ICryptoTransform decrypt = aes.CreateDecryptor())
                        {
                            byte[] encryptedTokenBytes = encryptedTokenString.BytesFromHexString();
                            byte[] decryptedTokenBytes = decrypt.TransformFinalBlock(encryptedTokenBytes, 0, encryptedTokenBytes.Length);
                            if (decryptedTokenBytes.Length > 0)
                            {
                                string accessTokenString = Encoding.UTF8.GetString(decryptedTokenBytes);
                                JObject accessTokenJSON = JObject.Parse(accessTokenString);
                                accessToken = accessTokenJSON["access_token"].Value<string>();
                            }
                        }
                    }
                }
                return (accessToken);
            }
            else
            {
                return (String.Empty);
            }
        }
    }

}

using System;
using System.Collections.Generic;
using cachy.Native.Cryptography.Interfaces;
using Newtonsoft.Json.Linq;

namespace cachy.Droid.Cryptography
{

    public class NativeAndroidKeyStorePasswordVault : INativePasswordVault
    {

        #region private constants

        public const string PASSWORDCONFIG_FILENAME = "cachy.keystore.json";

        #endregion

        #region private objects

        private Config.Config _configuration;

        #endregion

        #region private properties

        private Config.Config Configuration
        {
            get
            {
                if(_configuration == null)
                {
                    _configuration = Config.Config.Open(PASSWORDCONFIG_FILENAME);
                }
                return (_configuration);
            }
        }

        #endregion

        #region public methods

        public Dictionary<string, string> GetCredential(string key)
        {
            string aliasKey = String.Format("cachy.{0}", key);
            string credsCipherText = Configuration.GetValue<string>(key, string.Empty);
            if(!String.IsNullOrEmpty(credsCipherText))
            {
                string credsPlainText = AndroidKeyStore.DecodeBase64AndDecrypt(aliasKey, credsCipherText);
                if(!String.IsNullOrEmpty(credsPlainText))
                {
                    JObject credsJSON = JObject.Parse(credsPlainText);
                    Dictionary<string, string> creds = new Dictionary<string, string>
                    {
                        { "UserName", credsJSON["UserName"].Value<string>() },
                        { "Password", credsJSON["Password"].Value<string>() }
                    };
                    return (creds);
                }
                else
                {
                    return (null);
                }
            }
            else
            {
                return (null);
            }
        }

        public int RemovePassword(string key)
        {
            string aliasKey = String.Format("cachy.{0}", key);
            if(Configuration.RemoveValue(aliasKey))
            {
                Configuration.Save();
                return (1);
            }
            else
            {
                return (0);
            }
        }

        public void StoreCredential(
            string key,
            string username,
            string password,
            bool removeFirst)
        {
            if(removeFirst)
            {
                RemovePassword(key);
            }
            string aliasKey = String.Format("cachy.{0}", key);
            JObject credsJSON = new JObject
            {
                { "UserName", new JValue(username) },
                { "Password", new JValue(password) }
            };
            string credsPlainText = credsJSON.ToString(Newtonsoft.Json.Formatting.None);
            string cipherText = AndroidKeyStore.EncryptAndBase64Encode(aliasKey, credsPlainText);
            Configuration.SetValue<string>(key, cipherText);
        }

        public int RemoveAll()
        {
            return (-1);
        }

        #endregion

    }

}
using cachy.Native.Cryptography.Interfaces;
using devoctomy.DFramework.Logging;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Security.Credentials;

namespace cachy.UWP.Cryptography
{

    public class NativePasswordVault : INativePasswordVault
    {

        #region public methods

        public Dictionary<string, string> GetCredential(string key)
        {
            PasswordVault vault = new PasswordVault();
            IReadOnlyList<PasswordCredential> matches = null;
            try
            {
                matches = vault.FindAllByResource(key);
            }
            catch (System.Exception ex)
            {
                DLoggerManager.Instance.Logger.Log(
                    devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception,
                    "An exception occurred whilst getting the credential with the key '{0}'.\r{1}",
                    key,
                    ex.ToString());
                return (null);
            }
            if (matches.Count > 0)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                PasswordCredential credential = vault.Retrieve(key, matches[0].UserName);
                result.Add("UserName", credential.UserName);
                result.Add("Password", credential.Password);
                return (result);
            }
            else
            {
                DLoggerManager.Instance.Logger.Log(
                    devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Warning,
                    "No matches found for the credential with the key '{0}'.",
                    key);
                return (null);
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
            PasswordVault vault = new PasswordVault();
            PasswordCredential credential = new PasswordCredential(
                key,
                username,
                password);
            vault.Add(credential);
        }

        public int RemovePassword(string key)
        {
            PasswordVault vault = new PasswordVault();
            IReadOnlyList<PasswordCredential> matches = null;
            try
            {
                matches = vault.FindAllByResource(key);
            }
            catch(System.Exception ex)
            {
                DLoggerManager.Instance.Logger.Log(
                    devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception,
                    "An exception occurred whilst removing the credential with the key '{0}'.\r{1}",
                    key,
                    ex.ToString());
                return (0);
            }
            if (matches.Count > 0)
            {
                foreach(PasswordCredential curCredential in matches)
                {
                    vault.Remove(curCredential);
                }
                return (matches.Count);
            }
            else
            {
                //DLoggerManager.Instance.Logger.Log(
                //    devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Warning,
                //    "No matches found for the credential with the key '{0}'.",
                //    key);
                return (0);
            }
        }

        public int RemoveAll()
        {
            int count = 0;
            PasswordVault vault = new PasswordVault();
            IReadOnlyList<PasswordCredential> matches = null;
            try
            {
                matches = vault.RetrieveAll();
                foreach(PasswordCredential curCredential in matches)
                {
                    vault.Remove(curCredential);
                    count += 1;
                }
            }
            catch (System.Exception ex)
            {
                DLoggerManager.Instance.Logger.Log(
                    devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Exception,
                    "An exception occurred whilst removing all credentials.\r{0}",
                    ex.ToString());
            }
            return (count);
        }

        #endregion

    }

}

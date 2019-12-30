using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using cachy.Native.Cryptography.Interfaces;

namespace cachy.Droid.Cryptography
{

    public class NativeAppKeyStorePasswordVault : INativePasswordVault
    {

        #region public methods

        public Dictionary<string, string> GetCredential(string key)
        {
            string usernameKey = String.Format("{0}.username", key);
            string passwordKey = String.Format("{0}.password", key);

            IEnumerable<string> usernames = AppKeyStore.Instance.FindAccountsForService(usernameKey);
            if(usernames.Any())
            {
                IEnumerable<string> passwords = AppKeyStore.Instance.FindAccountsForService(passwordKey);
                if(passwords.Any())
                {
                    Dictionary<string, string> result = new Dictionary<string, string>
                    {
                        { "UserName", usernames.First() },
                        { "Password", passwords.First() }
                    };
                    return (result);
                }
            }
            return (null);
        }

        public void StoreCredential(
            string key,
            string username,
            string password,
            bool removeFirst)
        {
            if (removeFirst)
            {
                RemovePassword(key);
            }
            string usernameKey = String.Format("{0}.username", key);
            AppKeyStore.Instance.Add(username, usernameKey);
            string passwordKey = String.Format("{0}.password", key);
            AppKeyStore.Instance.Add(password, passwordKey);
        }

        public int RemovePassword(string key)
        {
            string usernameKey = String.Format("{0}.username", key);
            IEnumerable<string> usernames = AppKeyStore.Instance.FindAccountsForService(usernameKey);
            if (usernames.Any())
            {
                AppKeyStore.Instance.Delete(usernameKey);
            }
            string passwordKey = String.Format("{0}.password", key);
            IEnumerable<string> passwords = AppKeyStore.Instance.FindAccountsForService(passwordKey);
            if (passwords.Any())
            {
                AppKeyStore.Instance.Delete(passwordKey);
            }
            return (usernames.Count());
        }

        public int RemoveAll()
        {
            return (-1);
        }

        #endregion

    }

}
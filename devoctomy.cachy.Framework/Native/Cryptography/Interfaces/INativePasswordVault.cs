using System;
using System.Collections.Generic;
using System.Text;

namespace cachy.Native.Cryptography.Interfaces
{

    public interface INativePasswordVault
    {

        #region public methods

        Dictionary<string, string> GetCredential(string key);

        void StoreCredential(
            string key,
            string username,
            string password,
            bool removeFirst);

        int RemovePassword(string key);

        int RemoveAll();

        #endregion

    }

}

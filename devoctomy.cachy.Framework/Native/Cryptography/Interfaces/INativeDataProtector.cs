using System;
using System.Threading.Tasks;

namespace cachy.Native.Cryptography.Interfaces
{

    public interface INativeDataProtector
    {

        #region methods

        Task<string> ProtectData(string plainText);

        Task<string> UnprotectData(string cipherText);

        #endregion

    }

}

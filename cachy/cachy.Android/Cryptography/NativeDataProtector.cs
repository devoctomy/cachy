using System;
using System.Threading.Tasks;
using cachy.Native.Cryptography.Interfaces;

namespace cachy.Droid.Cryptography
{

    public class NativeDataProtector : INativeDataProtector
    {

        #region private constants

        private const string ANDROIDKEYSTORE_ALIAS = "cachy.Android";

        #endregion

        #region public methods

        public Task<string> ProtectData(string plainText)
        {
            return Task<string>.Run(() =>
            {
                string cipherText = AndroidKeyStore.EncryptAndBase64Encode(
                    ANDROIDKEYSTORE_ALIAS, 
                    plainText);
                return (cipherText);
            });
        }

        public Task<string> UnprotectData(string cipherText)
        {
            return Task<string>.Run(() =>
            {
                string plainText = AndroidKeyStore.DecodeBase64AndDecrypt(
                    ANDROIDKEYSTORE_ALIAS, 
                    cipherText);
                return (plainText);
            });
        }

        #endregion

    }

}
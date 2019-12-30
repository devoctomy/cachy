using cachy.Native.Cryptography.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;

namespace cachy.UWP.Cryptography
{

    public class NativeDataProtector : INativeDataProtector
    {

        #region public methods

        public async Task<string> ProtectData(string plainText)
        {
            DataProtectionProvider provider = new DataProtectionProvider("LOCAL=user");
            IBuffer buffMsg = CryptographicBuffer.ConvertStringToBinary(plainText, BinaryStringEncoding.Utf8);
            IBuffer buffProtected = await provider.ProtectAsync(buffMsg);
            string base64Encoded = CryptographicBuffer.EncodeToBase64String(buffProtected);
            return (base64Encoded);
        }

        public async Task<string> UnprotectData(string cipherText)
        {
            DataProtectionProvider provider = new DataProtectionProvider();
            IBuffer @protected = CryptographicBuffer.DecodeFromBase64String(cipherText);
            IBuffer buffUnprotected = await provider.UnprotectAsync(@protected);
            String strClearText = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffUnprotected);
            return (strClearText);
        }

        #endregion

    }

}

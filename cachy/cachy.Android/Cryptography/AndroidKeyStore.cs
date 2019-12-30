using Android.Security.Keystore;
using Java.Security;
using Javax.Crypto;
using Javax.Crypto.Spec;
using System;
using static Java.Security.KeyStore;

namespace cachy.Droid.Cryptography
{

    public class AndroidKeyStore
    {

        #region public methods

        public static string EncryptAndBase64Encode(
            string alias,
            string plainText)
        {
            Cipher cipher = Cipher.GetInstance("AES/CBC/PKCS7Padding");
            cipher.Init(CipherMode.EncryptMode, GenerateSecretKey(alias));
            byte[] iv = cipher.GetIV();
            byte[] cipherBytes = cipher.DoFinal(System.Text.Encoding.UTF8.GetBytes(plainText));
            byte[] full = new byte[iv.Length + cipherBytes.Length];
            Array.ConstrainedCopy(iv, 0, full, 0, iv.Length);
            Array.ConstrainedCopy(cipherBytes, 0, full, iv.Length, cipherBytes.Length);
            string base64 = Convert.ToBase64String(full);
            return (base64);
        }

        public static string DecodeBase64AndDecrypt(
            string alias,
            string cipherText)
        {
            byte[] full = Convert.FromBase64String(cipherText);
            byte[] iv = new byte[16];
            byte[] cipherBytes = new byte[full.Length - 16];
            Array.Copy(full, iv, iv.Length);
            Array.Copy(full, 16, cipherBytes, 0, cipherBytes.Length);
            KeyStore keyStore = KeyStore.GetInstance("AndroidKeyStore");
            keyStore.Load(null);
            Cipher cipher = Cipher.GetInstance("AES/CBC/PKCS7Padding");
            IvParameterSpec spec = new IvParameterSpec(iv);
            ISecretKey key = GetSecretKey(keyStore, alias);
            if(key != null)
            {
                cipher.Init(CipherMode.DecryptMode, key, spec);
                byte[] plainTextBytes = cipher.DoFinal(cipherBytes);
                string plainText = System.Text.Encoding.UTF8.GetString(plainTextBytes);
                return (plainText);
            }
            else
            {
                return (String.Empty);
            }
        }

        #endregion

        #region private methods

        private static ISecretKey GenerateSecretKey(string alias)
        {
            KeyGenerator keyGenerator = KeyGenerator.GetInstance("AES", "AndroidKeyStore");
            keyGenerator.Init(new KeyGenParameterSpec.Builder(
                alias, 
                KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                .SetBlockModes("CBC")
                .SetEncryptionPaddings(KeyProperties.EncryptionPaddingPkcs7)
                .Build());
            return (keyGenerator.GenerateKey());
        }

        private static ISecretKey GetSecretKey(
            KeyStore keyStore,
            string alias)
        {
            SecretKeyEntry entry = (SecretKeyEntry)keyStore.GetEntry(alias, null);
            if(entry != null)
            {
                ISecretKey secretKey = entry.SecretKey;
                return (secretKey);
            }
            else
            {
                return (null);
            }
        }

        #endregion

    }

}
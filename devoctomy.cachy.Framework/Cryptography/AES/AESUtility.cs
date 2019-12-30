using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.cachy.Framework.Cryptography.AES
{

    public class AESUtility
    {

        #region public methods

        public static byte[] EncryptBytes(
            byte[] data, 
            byte[] key, 
            byte[] IV)
        {
            if (data == null || data.Length <= 0) throw new ArgumentNullException("plainText");
            if (key == null || key.Length <= 0) throw new ArgumentNullException("key");
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException("IV");
            byte[] encrypted;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                        csEncrypt.Flush();
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return encrypted;
        }

        public static byte[] DecryptBytes(
            byte[] cipherText, 
            byte[] key, 
            byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0) throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0) throw new ArgumentNullException("key");
            if (IV == null || IV.Length <= 0) throw new ArgumentNullException("IV");
            byte[] plaintext = null;
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream decrypted = new MemoryStream())
                        {
                            csDecrypt.CopyTo(decrypted);
                            decrypted.Flush();
                            plaintext = decrypted.ToArray();
                        }                       
                    }
                }
            }
            return plaintext;
        }

        #endregion

    }

}

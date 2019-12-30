using cachywebfunctions.Extensions;
using cachywebfunctions.SystemExtensions;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;

namespace cachywebfunctions.Entities
{

    public class OAuthSessionEntity : TableEntity
    {

        #region private objects

        private string _publicRSAKey = String.Empty;
        private string _encryptedAESKey = String.Empty;
        private string _state = String.Empty;
        private string _encryptedToken = String.Empty;

        #endregion

        #region public properties

        public string PublicRSAKey
        {
            get
            {
                return (_publicRSAKey);
            }
            set
            {
                _publicRSAKey = value;
            }
        }

        public string EncryptedAESKey
        {
            get
            {
                return (_encryptedAESKey);
            }
            set
            {
                _encryptedAESKey = value;
            }
        }

        public string State
        {
            get
            {
                return (_state);
            }
            set
            {
                _state = value;
            }
        }

        public string EncryptedToken
        {
            get
            {
                return (_encryptedToken);
            }
            set
            {
                _encryptedToken = value;
            }
        }

        #endregion

        #region constructor / destructor

        public OAuthSessionEntity()
        { }

        public OAuthSessionEntity(string partitionKey = "global")
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = partitionKey;
            State = "new";
        }

        #endregion

        #region public methods

        public void Complete(string token)
        {
            //generate aes key and iv
            AesCryptoServiceProvider aes = (AesCryptoServiceProvider)AesCryptoServiceProvider.Create();
            aes.GenerateIV();
            aes.GenerateKey();
            aes.Padding = PaddingMode.PKCS7;

            //encrypt the token and encode as hex
            byte[] unencryptedTokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
            using (ICryptoTransform encrypt = aes.CreateEncryptor())
            {
                byte[] encryptedTokenBytes = encrypt.TransformFinalBlock(unencryptedTokenBytes, 0, unencryptedTokenBytes.Length);
                EncryptedToken = encryptedTokenBytes.ToHexString();
            }

            //hex decode the public rsa key xml
            string publicRSAKeyXML = PublicRSAKey.StringFromHexString();

            //create an initialise an instance of the rsa provider
            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)RSACryptoServiceProvider.Create();
            rsa.FromXmlString(publicRSAKeyXML);

            //encrypt and hex encode the key
            byte[] encryptedAESKeyBytes = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);
            string encryptedAESKey = encryptedAESKeyBytes.ToHexString();

            //hex encode the IV
            string iv = aes.IV.ToHexString();

            //concatenate the iv and encrypted key
            EncryptedAESKey = String.Format("{0}.{1}", iv, encryptedAESKey);

            State = "complete";
        }

        public string ToJSON(bool minimal)
        {
            JObject json = new JObject();
            if (minimal) json.Add("ID", new JValue(RowKey));
            if (!minimal) json.Add("PublicRSAKey", new JValue(PublicRSAKey));
            if (!minimal) json.Add("EncryptedAESKey", new JValue(EncryptedAESKey));
            if (!minimal) json.Add("EncryptedToken", new JValue(EncryptedToken));
            if (!minimal) json.Add("statusCode", new JValue(200));
            return (json.ToString(Newtonsoft.Json.Formatting.None));
        }

        #endregion

    }

}

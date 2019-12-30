using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Data.Graph;
using devoctomy.cachy.Framework.Data.Graph.Nodes;
using devoctomy.cachy.Framework.Serialisers.Interfaces;
using devoctomy.cachy.Framework.Serialisers.JSON;
using Newtonsoft.Json.Linq;
using System;
using devoctomy.cachy.Framework.Cryptography.Random;
using devoctomy.DFramework.Core.SystemExtensions;
using System.Collections.Generic;
using System.Linq;

namespace devoctomy.cachy.Framework.Serialisers.AESEncrypted
{

    public class AESEncryptedVaultSerialiser : ISerialiser
    {

        #region public methods

        public object Read(
            object data, 
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            if (data.GetType() != typeof(Byte[])) throw new ArgumentException("Data must be of type 'Byte[]'.", "data");

            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Reading Vault from AES encrypted byte array.");

            Dictionary<string, string> parametersDict = parameters.ToDictionary(x => x.Key, x => x.Value);

            byte[] dataBytesWithSalt = (Byte[])data;
            byte[] dataBytes;
            byte[] salt;
            dataBytesWithSalt.RemoveFromEnd(16, out dataBytes, out salt);

            GraphBuilder decryptGraph = new GraphBuilder("AESEncryptedVaultSerialiser.Read");
            GraphIO<Byte[]> encryptedInput = new GraphIO<Byte[]>(dataBytes);
            GraphIO<Byte[]> saltInput = new GraphIO<Byte[]>(salt);

            IGraphNode keyDerivationNode = null;

            if(parametersDict.ContainsKey("KeyDerivationFunction"))
            {
                string keyDerivationFunction = parametersDict["KeyDerivationFunction"];
                switch (keyDerivationFunction)
                {
                    case "PBKDF2":
                        {
                            int iterationCount = int.Parse(parametersDict["IterationCount"]);
                            keyDerivationNode = new Rfc2898DeriveBytesNode(new GraphIO<String>(String.Empty), new GraphIO<Int32>(32), saltInput, new GraphIO<int>(iterationCount));
                            decryptGraph.AddNode("deriveNode", keyDerivationNode);
                            break;
                        }
                    case "SCRYPT":
                        {
                            int iterationCount = int.Parse(parametersDict["IterationCount"]);
                            int blockSize = int.Parse(parametersDict["BlockSize"]);
                            int threadCount = int.Parse(parametersDict["ThreadCount"]);
                            keyDerivationNode = new SCryptDeriveBytesNode(new GraphIO<String>(String.Empty), new GraphIO<Int32>(32), saltInput, new GraphIO<int>(iterationCount), new GraphIO<int>(blockSize), new GraphIO<int>(threadCount));
                            decryptGraph.AddNode("deriveNode", keyDerivationNode);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException(String.Format("Unknown key derivation function '{0}'.", keyDerivationFunction));
                        }
                }
            }
            else
            {
                keyDerivationNode = new Rfc2898DeriveBytesNode(new GraphIO<String>(String.Empty), new GraphIO<Int32>(32), saltInput, new GraphIO<Int32>(10000));
                decryptGraph.AddNode("deriveNode", keyDerivationNode);
            }

            AESDecryptNode decryptNode = new AESDecryptNode(encryptedInput, new GraphIO<Byte[]>(null), keyDerivationNode.GetBytesIO("DerivedKey"));

            decryptGraph.AddNode("decryptNode", decryptNode);
            decryptGraph.CreateRoute("decrypt",
                keyDerivationNode.GetStringIO("Password"),
                decryptNode.UnencryptedData,
                "deriveNode",
                new String[] { "deriveNode", "decryptNode" });

            Object output = null;
            if (decryptGraph.Process(true, "decrypt", masterPassphrase, out output))
            {
                String decryptedJSON = (String)output;
                JObject json = JObject.Parse(decryptedJSON);

                JSONVaultSerialiser jsonSerialiser = new JSONVaultSerialiser();
                Vault vault = (Vault)jsonSerialiser.Read(json, String.Empty);
                return (vault);
            }
            else
            {
                throw new Exception("Failed to decrypt Vault.");
            }
        }

        public object Write(
            object data, 
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            if (data.GetType() != typeof(Vault)) throw new ArgumentException("Data must be of type 'Vault'.", "data");

            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Writing Vault to AES encrypted byte array.");

            Dictionary<string, string> parametersDict = parameters.ToDictionary(x => x.Key, x => x.Value);

            Vault dataVault = (Vault)data;

            JSONVaultSerialiser jsonSerialiser = new JSONVaultSerialiser();
            JObject json = (JObject)jsonSerialiser.Write(dataVault, String.Empty, parameters);

            String jsonString = json.ToString(Newtonsoft.Json.Formatting.None, null);
            Byte[] salt = SimpleRandomGenerator.QuickGetRandomBytes(16);

            GraphBuilder decryptGraph = new GraphBuilder("AESEncryptedVaultSerialiser.Read");
            GraphIO<String> unencryptedInput = new GraphIO<String>(jsonString);
            GraphIO<Byte[]> saltInput = new GraphIO<Byte[]>(salt);

            //create derive node here
            IGraphNode keyDerivationNode = null;

            if (parametersDict.ContainsKey("KeyDerivationFunction"))
            {
                string keyDerivationFunction = parametersDict["KeyDerivationFunction"];
                switch (keyDerivationFunction)
                {
                    case "PBKDF2":
                        {
                            int iterationCount = int.Parse(parametersDict["IterationCount"]);
                            keyDerivationNode = new Rfc2898DeriveBytesNode(new GraphIO<String>(String.Empty), new GraphIO<Int32>(32), saltInput, new GraphIO<int>(iterationCount));
                            decryptGraph.AddNode("deriveNode", keyDerivationNode);
                            break;
                        }
                    case "SCRYPT":
                        {
                            int iterationCount = int.Parse(parametersDict["IterationCount"]);
                            int blockSize = int.Parse(parametersDict["BlockSize"]);
                            int threadCount = int.Parse(parametersDict["ThreadCount"]);
                            keyDerivationNode = new SCryptDeriveBytesNode(new GraphIO<String>(String.Empty), new GraphIO<Int32>(32), saltInput, new GraphIO<int>(iterationCount), new GraphIO<int>(blockSize), new GraphIO<int>(threadCount));
                            decryptGraph.AddNode("deriveNode", keyDerivationNode);
                            break;
                        }
                    default:
                        {
                            throw new InvalidOperationException(String.Format("Unknown key derivation function '{0}'.", keyDerivationFunction));
                        }
                }
            }
            else
            {
                //default setup
                keyDerivationNode = new Rfc2898DeriveBytesNode(new GraphIO<String>(String.Empty), new GraphIO<Int32>(32), saltInput, new GraphIO<int>(10000));
                decryptGraph.AddNode("deriveNode", keyDerivationNode);
            }

            AESEncryptNode encryptNode = new AESEncryptNode(unencryptedInput, new GraphIO<Byte[]>(null), keyDerivationNode.GetBytesIO("DerivedKey"));

            decryptGraph.AddNode("encryptNode", encryptNode);
            decryptGraph.CreateRoute("encrypt",
                keyDerivationNode.GetStringIO("Password"),
                encryptNode.EncryptedData,
                "deriveNode",
                new String[] { "deriveNode", "encryptNode" });

            Object output = null;
            if (decryptGraph.Process(true, "encrypt", masterPassphrase, out output))
            {
                byte[] encrypted = (byte[])output;
                byte[] encryptedWithSalt = encrypted.AppendBytes(salt);
                return (encryptedWithSalt);
            }
            else
            {
                throw new Exception("Failed to encrypt Vault.");
            }

            throw new NotImplementedException();
        }

        #endregion

    }

}

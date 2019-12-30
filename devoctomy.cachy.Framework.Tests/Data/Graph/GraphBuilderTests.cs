using System;
using System.Linq;
using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Cryptography.Random;
using devoctomy.cachy.Framework.Data.Graph;
using devoctomy.cachy.Framework.Data.Graph.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace devoctomy.cachy.Framework.Tests.Data.Graph
{

    [TestClass]
    public class GraphBuilderTests
    {

        #region private methods

        private static Boolean ResolvePath(String token,
            out String resolvedPath)
        {
            switch(token)
            {
                case "{AppData}":
                    {
                        resolvedPath = @"C:\ProgramData\devoctomy\cachy\Logging\Output";
                        return (true);
                    }
                default:
                    {
                        throw new NotImplementedException(String.Format("Path token '{0}' has not been handled by the logging host.", token));
                    }
            }
        }

        #endregion

        #region public methods

        [TestInitialize]
        public void Initialise()
        {
            DLoggerManager.PathDelimiter = "\\";
            Directory.ResolvePath = ResolvePath;
        }

        [TestMethod]
        public void AESEncryptEncodeDecodeDecryptNodeTest()
        {
            Byte[] salt = SimpleRandomGenerator.QuickGetRandomBytes(16);
            String password = "This is my passphrase!";
            String plainText = "Testing testing, 123.";

            GraphBuilder encryptGraph = new GraphBuilder();
            GraphIO<String> passwordInput = new GraphIO<String>(plainText);
            GraphIO<Int32> keyLengthInput = new GraphIO<Int32>(32);
            GraphIO<Byte[]> saltInput = new GraphIO<Byte[]>(salt);
            Rfc2898DeriveBytesNode deriveEncryptKeyNode = new Rfc2898DeriveBytesNode(passwordInput, keyLengthInput, saltInput, new GraphIO<Int32>(10000));
            AESEncryptNode encryptNode = new AESEncryptNode(new GraphIO<String>(plainText), new GraphIO<Byte[]>(null), deriveEncryptKeyNode.DervivedKey);
            Base64EncodeNode encodeNode = new Base64EncodeNode(encryptNode.EncryptedData);

            encryptGraph.AddNode("deriveNode", deriveEncryptKeyNode);
            encryptGraph.AddNode("encryptNode", encryptNode);
            encryptGraph.AddNode("encodeNode", encodeNode);
            encryptGraph.CreateRoute("encrypt",
                deriveEncryptKeyNode.Password, 
                encodeNode.EncodedData, 
                "deriveNode",
                new String[] { "deriveNode", "encryptNode", "encodeNode" });

            Object output = null;
            if (encryptGraph.Process(true, "encrypt", password, out output))
            {
                String encryptedEncoded = (String)output;

                GraphBuilder decryptGraph = new GraphBuilder();
                GraphIO<String> encryptedEncodedInput = new GraphIO<String>(encryptedEncoded);
                Rfc2898DeriveBytesNode deriveDecryptKeyNode = new Rfc2898DeriveBytesNode(new GraphIO<String>(String.Empty), new GraphIO<Int32>(32), new GraphIO<Byte[]>(salt), new GraphIO<Int32>(10000));
                Base64DecodeNode decodeNode = new Base64DecodeNode(encryptedEncodedInput);
                AESDecryptNode decryptNode = new AESDecryptNode(decodeNode.UnencodedData, new GraphIO<Byte[]>(null), deriveDecryptKeyNode.DervivedKey);

                decryptGraph.AddNode("deriveNode", deriveDecryptKeyNode);
                decryptGraph.AddNode("decodeNode", decodeNode);
                decryptGraph.AddNode("decryptNode", decryptNode);
                decryptGraph.CreateRoute("decrypt",
                    deriveDecryptKeyNode.Password,
                    decryptNode.UnencryptedData,
                    "deriveNode",
                    new String[] { "deriveNode", "decodeNode", "decryptNode" });

                if (decryptGraph.Process(true, "decrypt", password, out output))
                {
                    String decodedDecrypted = (String)output;
                    Assert.IsTrue(decodedDecrypted == plainText);
                }
            }
        }

        [TestMethod]
        public void DeriveKeyNodeTest()
        {
            Byte[] salt = SimpleRandomGenerator.QuickGetRandomBytes(16);
            Rfc2898DeriveBytesNode node = new Rfc2898DeriveBytesNode(new GraphIO<String>(String.Empty), new GraphIO<Int32>(32), new GraphIO<Byte[]>(salt), new GraphIO<Int32>(10000));
            Byte[] key1 = null;
            Byte[] key2 = null;
            Object output = null;
            if(GraphBuilder.ExecuteIsolatedNode(node, node.Password, node.DervivedKey, "This is my passphrase!", out output))
            {
                key1 = (Byte[])output;
                if (GraphBuilder.ExecuteIsolatedNode(node, node.Password, node.DervivedKey, "This is my passphrase!", out output))
                {
                    key2 = (Byte[])output;
                    Assert.IsTrue(key1.SequenceEqual(key2));
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }


        #endregion

    }

}

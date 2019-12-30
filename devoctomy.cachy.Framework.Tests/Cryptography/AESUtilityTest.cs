using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using devoctomy.DFramework.Logging;
using devoctomy.DFramework.Core.IO;
using devoctomy.cachy.Framework.Cryptography.AES;
using devoctomy.cachy.Framework.Cryptography.Random;

namespace devoctomy.cachy.Tests.Cryptography
{
    /// <summary>
    /// Summary description for AESUtilityTest
    /// </summary>
    [TestClass]
    public class AESUtilityTest
    {

        #region private objects

        private Random _rnd;

        #endregion

        #region private methods

        private static Boolean ResolvePath(String token,
            out String resolvedPath)
        {
            switch (token)
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
            _rnd = new Random(Environment.TickCount);
        }

        [TestMethod]
        public void EncryptDecryptSimpleStringTest()
        {
            string plainText = "Hello World!";
            byte[] plainBytes = System.Text.Encoding.UTF8.GetBytes(plainText);

            byte[] iv = SimpleRandomGenerator.QuickGetRandomBytes(16);
            byte[] key = SimpleRandomGenerator.QuickGetRandomBytes(32);

            byte[] cipherBytes = AESUtility.EncryptBytes(plainBytes, key, iv);

            byte[] decryptedBytes = AESUtility.DecryptBytes(cipherBytes, key, iv);
            string decryptedPlainText = System.Text.Encoding.UTF8.GetString(decryptedBytes);

            Assert.AreEqual(plainText, decryptedPlainText);
        }

        #endregion

    }

}

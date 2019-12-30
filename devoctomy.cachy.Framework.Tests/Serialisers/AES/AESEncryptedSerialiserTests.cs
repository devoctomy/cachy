using System;
using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Serialisers.AESEncrypted;
using devoctomy.cachy.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace devoctomy.cachy.Tests.Serialisers.AES
{

    [TestClass]
    public class AESEncryptedSerialiserTests
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
        public void AESEncryptedVaultSerialiserTest()
        {
            String masterPassphrase = "This is my passphrase!";
            Vault vault = VaultTests.CreateRandomVault(_rnd);
            AESEncryptedVaultSerialiser serialiser = new AESEncryptedVaultSerialiser();
            Byte[] encrypted = (Byte[])serialiser.Write(vault, masterPassphrase);
            Vault decryptedVault = (Vault)serialiser.Read(encrypted, masterPassphrase);
            Assert.IsTrue(vault.CompareTo(decryptedVault) == 0);
        }

        #endregion

    }

}

using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Serialisers.Interfaces;
using devoctomy.cachy.Framework.Serialisers.JSON;
using devoctomy.cachy.Framework.Serialisers.JSON.Versions;
using devoctomy.cachy.Tests.Data;
using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace devoctomy.cachy.Tests.Serialisers
{

    [TestClass]
    public class FormatVersionsTests
    {

        #region private objects

        private static Random _rng;

        #endregion

        #region private methods

        [TestInitialize]
        public void Initialise()
        {
            _rng = new Random(Environment.TickCount);
            Directory.ResolvePath = ResolvePath;
            DLoggerManager.PathDelimiter = "\\";
        }

        private static Boolean ResolvePath(String token,
            out String resolvedPath)
        {
            switch (token)
            {
                case "{AppData}":
                    {
                        String appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                        resolvedPath = appData;
                        return (true);
                    }
                case "{LocalVaults}":
                    {
                        String appData = String.Empty;
                        if (ResolvePath("{AppData}", out appData))
                        {
                            if (!appData.EndsWith("\\")) appData += "\\";
                            resolvedPath = appData + "LocalVaults\\";
                            return (true);
                        }
                        else
                        {
                            resolvedPath = String.Empty;
                            return (false);
                        }
                    }
                default:
                    {
                        throw new NotImplementedException(String.Format("Path token '{0}' has not been handled by the logging host.", token));
                    }
            }
        }

        #endregion

        #region public methods

        [TestMethod]
        public void SequentialUpgradeTest()
        {
            Vault testVault = VaultTests.CreateRandomVault(_rng);
            IEnumerable<FormatVersionAttribute> vaultSerialisers = FormatVersions.Instance.VersionSerialisers.Keys.Where(fva => fva.ObjectType == typeof(Vault)).OrderBy(fva => double.Parse(fva.Version));
            FormatVersionAttribute[] filteredOrderedSerialisers = vaultSerialisers.ToArray();
            object curSerialised = null;
            Vault curVault = null;
            string lastVersion = String.Empty;
            for (int curVersion = 0; curVersion < filteredOrderedSerialisers.Length; curVersion++)
            {
                if(curSerialised != null)
                {
                    FormatVersionAttribute prevSerialiserAttrib = filteredOrderedSerialisers[curVersion - 1];
                    ISerialiser prevSerialiser = FormatVersions.Instance.GetSerialiser(prevSerialiserAttrib.Version, typeof(Vault));
                    curVault = (Vault)prevSerialiser.Read(curSerialised, String.Empty);
                }
                else
                {
                    curVault = testVault;
                }
                FormatVersionAttribute nextSerialiserAttrib = filteredOrderedSerialisers[curVersion];
                lastVersion = nextSerialiserAttrib.Version;
                ISerialiser nextSerialiser = FormatVersions.Instance.GetSerialiser(nextSerialiserAttrib.Version, typeof(Vault));
                curSerialised = nextSerialiser.Write(curVault, String.Empty);
            }
            Assert.IsTrue(lastVersion == Framework.Serialisers.JSON.Common.LATEST_VAULT_VERSION);
            ISerialiser latestSerialiser = FormatVersions.GetLatestSerialiser(typeof(Vault));
            curVault = (Vault)latestSerialiser.Read(curSerialised, String.Empty);
            Assert.IsTrue(curVault.CompareTo(testVault) == 0);
        }

        #endregion

    }

}

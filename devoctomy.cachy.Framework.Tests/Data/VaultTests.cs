using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Serialisers.JSON;
using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace devoctomy.cachy.Tests.Data
{

    [TestClass]
    public class VaultTests
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

        public static Vault CreateRandomVault(Random rng)
        {
            String id = String.Empty;
            String name = String.Empty;
            String description = String.Empty;
            DateTime createdAt = DateTime.MinValue;
            DateTime lastUpdatedAt = DateTime.MinValue;
            Credential[] credentials = null;
            AuditLogEntry[] auditLogEntries = null;

            Vault vault = CreateRandomVault(rng,
                out id,
                out name,
                out description,
                out createdAt,
                out lastUpdatedAt,
                out credentials,
                out auditLogEntries);

            return (vault);
        }

        public static Vault CreateRandomVault(Random rng,
            out String id,
            out String name,
            out String description,
            out DateTime createdAt,
            out DateTime lastUpdatedAt,
            out Credential[] credentials,
            out AuditLogEntry[] auditLogEntries)
        {
            id = Guid.NewGuid().ToString();
            name = Guid.NewGuid().ToString();
            description = Guid.NewGuid().ToString();
            createdAt = new DateTime(1982, 4, 3, rng.Next(0, 24), rng.Next(0, 60), rng.Next(0, 60));
            lastUpdatedAt = createdAt.Add(new TimeSpan(rng.Next(0, 101), rng.Next(0, 24), rng.Next(0, 60), rng.Next(0, 60)));
            List<Credential> credentialsList = new List<Credential>();
            while (credentialsList.Count < 10)
            {
                credentialsList.Add(CredentialTests.CreateRandomCredential(rng));
            }
            credentials = credentialsList.ToArray();
            List<AuditLogEntry> auditLogEntriesList = new List<AuditLogEntry>();
            while(auditLogEntriesList.Count < 100)
            {
                auditLogEntriesList.Add(AuditLogEntryTests.CreateRandomAuditLogEntry(rng));
            }
            auditLogEntries = auditLogEntriesList.ToArray();

            Vault vault = new Vault(id,
                name,
                description,
                createdAt,
                lastUpdatedAt,
                credentials,
                auditLogEntries);

            return (vault);
        }

        [TestMethod]
        public void Instantiation()
        {
            String id = String.Empty;
            String name = String.Empty;
            String description = String.Empty;
            DateTime createdAt = DateTime.MinValue;
            DateTime lastUpdatedAt = DateTime.MinValue;
            Credential[] credentials = null;
            AuditLogEntry[] auditLogEntries = null;

            Vault vault = CreateRandomVault(_rng,
                out id,
                out name,
                out description,
                out createdAt,
                out lastUpdatedAt,
                out credentials,
                out auditLogEntries);

            Assert.IsTrue(id == vault.ID);
            Assert.IsTrue(name == vault.Name);
            Assert.IsTrue(description == vault.Description);
            Assert.IsTrue(createdAt == vault.CreatedAt);
            Assert.IsTrue(lastUpdatedAt == vault.LastUpdatedAt);
            Assert.IsTrue(credentials.OrderBy(i => i).SequenceEqual(vault.Credentials.OrderBy(i => i)));
            if(auditLogEntries.Length == vault.AuditLogEntries.Count)
            {
                for(Int32 curEntry = 0; curEntry < auditLogEntries.Length; curEntry++)
                {
                    Assert.IsTrue(auditLogEntries[curEntry].CompareTo(vault.AuditLogEntries[curEntry]) == 0);
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void JSONSerialisation()
        {
            String id = String.Empty;
            String name = String.Empty;
            String description = String.Empty;
            DateTime createdAt = DateTime.MinValue;
            DateTime lastUpdatedAt = DateTime.MinValue;
            Credential[] credentials = null;
            AuditLogEntry[] auditLogEntries = null;

            Vault vault = CreateRandomVault(_rng,
                out id,
                out name,
                out description,
                out createdAt,
                out lastUpdatedAt,
                out credentials,
                out auditLogEntries);

            JSONVaultSerialiser serialiser = new JSONVaultSerialiser();
            JObject json = (JObject)serialiser.Write(vault, String.Empty);
            Vault vaultReloaded = (Vault)serialiser.Read(json, String.Empty);
            Assert.IsTrue(vault.CompareTo(vaultReloaded) == 0);
        }

        #endregion

    }

}

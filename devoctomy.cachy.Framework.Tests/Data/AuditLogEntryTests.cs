using System;
using System.Collections.Generic;
using System.Linq;
using devoctomy.cachy.Framework.Cryptography.Random;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Serialisers.Interfaces;
using devoctomy.cachy.Framework.Serialisers.JSON;
using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace devoctomy.cachy.Tests.Data
{
    [TestClass]
    public class AuditLogEntryTests
    {

        #region private objects

        private Random _rng;

        #endregion

        #region private methods

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

        [TestInitialize]
        public void Initialise()
        {
            _rng = new Random(Environment.TickCount);
            Directory.ResolvePath = ResolvePath;
            DLoggerManager.PathDelimiter = "\\";
        }

        public static AuditLogEntry CreateRandomAuditLogEntry(Random rng)
        {
            String name = String.Empty;
            String type = String.Empty;
            AuditLogEntry.EntryType entryType = AuditLogEntry.EntryType.None;

            AuditLogEntry entry = CreateRandomAuditLogEntry(rng,
                out name,
                out type,
                out entryType);

            return (entry);
        }

        public static AuditLogEntry CreateRandomAuditLogEntry(Random rng,
            out String name,
            out String type,
            out AuditLogEntry.EntryType entryType)
        {
            name = SimpleRandomGenerator.QuickGetRandomString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", 6);
            type = rng.Next(0, 100) >= 50 ? "vault" : "credential";
            entryType = SimpleRandomGenerator.QuickGetRandomEnum<AuditLogEntry.EntryType>(1, AuditLogEntry.EntryType.None);
            Dictionary<String, String> parameters = new Dictionary<String, String>();
            parameters.Add("Name", name);
            parameters.Add("Type", type);

            AuditLogEntry entry = new AuditLogEntry(entryType,
                parameters.ToArray());

            return (entry);
        }

        [TestMethod]
        public void Instantiation()
        {
            String name = String.Empty;
            String type = String.Empty;
            AuditLogEntry.EntryType entryType = AuditLogEntry.EntryType.None;

            AuditLogEntry random = CreateRandomAuditLogEntry(_rng,
                out name,
                out type,
                out entryType);

            Assert.IsTrue(random.Parameters["Name"] == name);
            Assert.IsTrue(random.Parameters["Type"] == type);
            Assert.IsTrue(random.TypeOfEntry == entryType);
        }

        [TestMethod]
        public void ToAndFromJSON()
        {
            String name = String.Empty;
            String type = String.Empty;
            AuditLogEntry.EntryType entryType = AuditLogEntry.EntryType.None;

            AuditLogEntry random = CreateRandomAuditLogEntry(_rng,
                out name,
                out type,
                out entryType);

            DateTime dateTime = random.DateTime;

            ISerialiser auditLogEntrySerialiser = FormatVersions.Instance.GetSerialiser(devoctomy.cachy.Framework.Serialisers.JSON.Common.LATEST_VAULT_VERSION, typeof(AuditLogEntry));
            JObject json = (JObject)auditLogEntrySerialiser.Write(random, String.Empty);
            AuditLogEntry fromJSON = (AuditLogEntry)auditLogEntrySerialiser.Read(json, String.Empty);

            Assert.IsTrue(random.CompareTo(fromJSON) == 0);
        }

        #endregion

    }
}

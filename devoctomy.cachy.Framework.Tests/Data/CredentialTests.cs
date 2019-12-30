using devoctomy.cachy.Framework.Cryptography.Random;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Serialisers.Interfaces;
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
    public class CredentialTests
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

        public static Credential CreateRandomCredential(Random rng)
        {
            string id = String.Empty;
            string glyphKey = String.Empty;
            string glyphColour = String.Empty;
            string name = String.Empty;
            string description = String.Empty;
            string website = String.Empty;
            DateTime createdAt = DateTime.MinValue;
            DateTime lastUpdatedAt = DateTime.MinValue;
            DateTime passwordLastModifiedAt = DateTime.MinValue;
            string username = String.Empty;
            string password = String.Empty;
            string[] tags = null;
            string notes = String.Empty;
            AuditLogEntry[] auditLogEntries;

            Credential credential = CreateRandomCredential(rng,
                out id,
                out glyphKey,
                out glyphColour,
                out name,
                out description,
                out website,
                out createdAt,
                out lastUpdatedAt,
                out passwordLastModifiedAt,
                out username,
                out password,
                out tags,
                out notes,
                out auditLogEntries);

            return (credential);
        }

        public static Credential CreateRandomCredential(
            Random rng,
            out string id,
            out string glyphKey,
            out string glyphColour,
            out string name,
            out string description,
            out string website,
            out DateTime createdAt,
            out DateTime lastModifiedAt,
            out DateTime passwordLastModifiedAt,
            out string username,
            out string password,
            out string[] tags,
            out string notes,
            out AuditLogEntry[] auditLogEntries)
        {
            id = Guid.NewGuid().ToString();
            glyphKey = "None";
            glyphColour = "Black";
            name = Guid.NewGuid().ToString();
            description = Guid.NewGuid().ToString();
            website = Guid.NewGuid().ToString();
            createdAt = new DateTime(1982, 4, 3, rng.Next(0, 24), rng.Next(0, 60), rng.Next(0, 60));
            lastModifiedAt = createdAt.Add(new TimeSpan(rng.Next(0, 101), rng.Next(0, 24), rng.Next(0, 60), rng.Next(0, 60)));
            passwordLastModifiedAt = createdAt.Add(new TimeSpan(rng.Next(0, 101), rng.Next(0, 24), rng.Next(0, 60), rng.Next(0, 60)));
            username = SimpleRandomGenerator.QuickGetRandomString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 12) + "@" + SimpleRandomGenerator.QuickGetRandomString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 6) + ".com";
            password = SimpleRandomGenerator.QuickGetRandomString(SimpleRandomGenerator.CharSelection.All, 16, true);
            List<String> tagsList = new List<String>();
            while (tagsList.Count < 10)
            {
                tagsList.Add(Guid.NewGuid().ToString());
            }
            tags = tagsList.ToArray();
            notes = SimpleRandomGenerator.QuickGetRandomString("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 256);
            List<AuditLogEntry> auditLogEntriesList = new List<AuditLogEntry>();
            while (auditLogEntriesList.Count < 100)
            {
                auditLogEntriesList.Add(AuditLogEntryTests.CreateRandomAuditLogEntry(rng));
            }
            auditLogEntries = auditLogEntriesList.ToArray();

            Credential credential = new Credential(id,
                glyphKey,
                glyphColour,
                name,
                description,
                website,
                createdAt,
                lastModifiedAt,
                passwordLastModifiedAt,
                username,
                password,
                tags,
                notes,
                auditLogEntries);

            return (credential);
        }

        [TestMethod]
        public void Instantiation()
        {
            string id = String.Empty;
            string glyphKey = String.Empty;
            string glyphColour = String.Empty;
            string name = String.Empty;
            string description = String.Empty;
            string website = String.Empty;
            DateTime createdAt = DateTime.MinValue;
            DateTime lastUpdatedAt = DateTime.MinValue;
            DateTime passwordLastModifiedAt = DateTime.MinValue;
            string username = String.Empty;
            string password = String.Empty;
            string[] tags = null;
            string notes = String.Empty;
            AuditLogEntry[] auditLogEntries = null;

            Credential credential = CreateRandomCredential(_rng,
                out id,
                out glyphKey,
                out glyphColour,
                out name,
                out description,
                out website,
                out createdAt,
                out lastUpdatedAt,
                out passwordLastModifiedAt,
                out username,
                out password,
                out tags,
                out notes,
                out auditLogEntries);

            Assert.IsTrue(id == credential.ID);
            Assert.IsTrue(glyphKey == credential.GlyphKey);
            Assert.IsTrue(glyphColour == credential.GlyphColour);
            Assert.IsTrue(name == credential.Name);
            Assert.IsTrue(description == credential.Description);
            Assert.IsTrue(website == credential.Website);
            Assert.IsTrue(createdAt == credential.CreatedAt);
            Assert.IsTrue(lastUpdatedAt == credential.LastModifiedAt);
            Assert.IsTrue(username == credential.Username);
            Assert.IsTrue(password == credential.Password);
            Assert.IsTrue(tags.OrderBy(i => i).SequenceEqual(credential.Tags.OrderBy(i => i)));
            Assert.IsTrue(notes == credential.Notes);
            if (auditLogEntries.Length == credential.AuditLogEntries.Count)
            {
                for (Int32 curEntry = 0; curEntry < auditLogEntries.Length; curEntry++)
                {
                    Assert.IsTrue(auditLogEntries[curEntry].CompareTo(credential.AuditLogEntries[curEntry]) == 0);
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void JSONSerialiser()
        {
            string id = String.Empty;
            string glyphKey = String.Empty;
            string glyphColour = String.Empty;
            string name = String.Empty;
            string description = String.Empty;
            string website = String.Empty;
            DateTime createdAt = DateTime.MinValue;
            DateTime lastUpdatedAt = DateTime.MinValue;
            DateTime passwordLastModifiedAt = DateTime.MinValue;
            string username = String.Empty;
            string password = String.Empty;
            string[] tags = null;
            string notes = String.Empty;
            AuditLogEntry[] auditLogEntries = null;

            Credential credential = CreateRandomCredential(_rng,
                out id,
                out glyphKey,
                out glyphColour,
                out name,
                out description,
                out website,
                out createdAt,
                out lastUpdatedAt,
                out passwordLastModifiedAt,
                out username,
                out password,
                out tags,
                out notes,
                out auditLogEntries);

            ISerialiser serialiser = FormatVersions.GetLatestSerialiser(typeof(Credential));
            JObject json = (JObject)serialiser.Write(credential, String.Empty);
            Credential credentialReloaded = (Credential)serialiser.Read(json, String.Empty);
            Assert.IsTrue(credential.CompareTo(credentialReloaded) == 0);
        }

        #endregion

    }

}

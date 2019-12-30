using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Serialisers.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace devoctomy.cachy.Framework.Serialisers.JSON.Versions
{

    [FormatVersion("0.1", typeof(Credential))]
    public class JSONCredentialSerialiser_Version0_1 : ISerialiser
    {

        #region public methods

        public object Read(
            object data, 
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            FormatVersionAttribute formatVersion = FormatVersionAttribute.GetAttributeFromType(this.GetType());

            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Writing Credential from JSON using serialiser '{0}'.", formatVersion);

            JObject dataJSON = (JObject)data;
            String id = JSONHelpers.ReadString(dataJSON, "ID");
            String glyphKey = JSONHelpers.ReadString(dataJSON, "GlyphKey");
            String glyphColour = JSONHelpers.ReadString(dataJSON, "GlyphColour");
            String name = JSONHelpers.ReadString(dataJSON, "Name");
            String description = JSONHelpers.ReadString(dataJSON, "Description");
            String website = JSONHelpers.ReadString(dataJSON, "Website");
            DateTime createdAt = JSONHelpers.ReadDateTime(dataJSON, "CreatedAt");
            DateTime lastUpdatedAt = JSONHelpers.ReadDateTime(dataJSON, "LastUpdatedAt");
            DateTime passwordLastModifiedAt = JSONHelpers.ReadDateTime(dataJSON, "PasswordLastModifiedAt", createdAt);
            String username = JSONHelpers.ReadString(dataJSON, "Username");
            String password = JSONHelpers.ReadString(dataJSON, "Password");
            JArray tagsArray = JSONHelpers.ReadJArray(dataJSON, "Tags", true);;
            List<String> tags = new List<String>();
            foreach (JValue curValue in tagsArray)
            {
                tags.Add(curValue.Value<String>());
            }
            String notes = JSONHelpers.ReadString(dataJSON, "Notes");
            List<AuditLogEntry> auditLogEntriesList = new List<AuditLogEntry>();
            if(dataJSON.ContainsKey("AuditLogEntries"))
            {
                ISerialiser auditLogEntrySerialiser = FormatVersions.Instance.GetSerialiser(formatVersion.Version, typeof(AuditLogEntry));
                JArray auditLogEntries = JSONHelpers.ReadJArray(dataJSON, "AuditLogEntries", true);
                foreach (JObject curEntry in auditLogEntries)
                {
                    AuditLogEntry entry = (AuditLogEntry)auditLogEntrySerialiser.Read(curEntry, String.Empty);
                    auditLogEntriesList.Add(entry);
                }
                if (auditLogEntriesList.Count > 0) auditLogEntriesList = auditLogEntriesList.OrderByDescending(ale => ale.DateTime).ToList();
            }
            return (new Credential(id,
                glyphKey,
                glyphColour,
                name,
                description,
                website,
                createdAt,
                lastUpdatedAt,
                passwordLastModifiedAt,
                username,
                password,
                tags.ToArray(),
                notes,
                auditLogEntriesList.ToArray()));
        }

        public object Write(
            object data,
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            FormatVersionAttribute formatVersion = FormatVersionAttribute.GetAttributeFromType(this.GetType());

            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Writing Credential to JSON using serialiser '{0}'.", formatVersion);

            Credential credential = (Credential)data;
            JObject json = new JObject();
            json.Add("ID", new JValue(credential.ID));
            json.Add("GlyphKey", new JValue(credential.GlyphKey));
            json.Add("GlyphColour", new JValue(credential.GlyphColour));
            json.Add("Name", new JValue(credential.Name));
            json.Add("Description", new JValue(credential.Description));
            json.Add("Website", new JValue(credential.Website));
            json.Add("CreatedAt", new JValue(credential.CreatedAt.ToString(Common.DATETIMESERIALISATIONFORMAT)));
            json.Add("LastUpdatedAt", new JValue(credential.LastModifiedAt.ToString(Common.DATETIMESERIALISATIONFORMAT)));
            json.Add("PasswordLastModifiedAt", new JValue(credential.PasswordLastModifiedAt.ToString(Common.DATETIMESERIALISATIONFORMAT)));
            json.Add("Username", new JValue(credential.Username));
            json.Add("Password", new JValue(credential.Password));
            JArray tagsArray = new JArray();
            foreach(String curTag in credential.Tags)
            {
                tagsArray.Add(new JValue(curTag));
            }
            json.Add("Tags", tagsArray);
            ISerialiser auditLogEntrySerialiser = FormatVersions.Instance.GetSerialiser(formatVersion.Version, typeof(AuditLogEntry));
            JArray auditLogEntries = new JArray();
            foreach (AuditLogEntry curEntry in credential.AuditLogEntries)
            {
                auditLogEntries.Add(auditLogEntrySerialiser.Write(curEntry, String.Empty));
            }
            json.Add("Notes", credential.Notes);
            json.Add("AuditLogEntries", auditLogEntries);
            return (json);
        }

        #endregion

    }

}

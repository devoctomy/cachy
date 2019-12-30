using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Serialisers.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace devoctomy.cachy.Framework.Serialisers.JSON.Versions
{

    [FormatVersion("0.2", typeof(Vault))]
    public class JSONVaultSerialiser_Version0_2 : ISerialiser
    {

        #region public methods

        public object Read(
            object data, 
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            FormatVersionAttribute formatVersion = FormatVersionAttribute.GetAttributeFromType(this.GetType());

            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Reading Vault from JSON using serialiser '{0}'.", formatVersion);

            JObject dataJSON = (JObject)data;

            String id = JSONHelpers.ReadString(dataJSON, "ID");
            String name = JSONHelpers.ReadString(dataJSON, "Name");
            String description = JSONHelpers.ReadString(dataJSON, "Description");
            DateTime createdAt = JSONHelpers.ReadDateTime(dataJSON, "CreatedAt");
            DateTime lastUpdatedAt = JSONHelpers.ReadDateTime(dataJSON, "LastUpdatedAt");
            List<Credential> credentialList = new List<Credential>();

            ISerialiser credentialSerialiser = FormatVersions.Instance.GetSerialiser(formatVersion.Version, typeof(Credential));
            JArray credentials = JSONHelpers.ReadJArray(dataJSON, "Credentials", true);
            foreach(JObject curCredential in credentials)
            {
                Credential credential = (Credential)credentialSerialiser.Read(curCredential, String.Empty);
                credentialList.Add(credential);
            }
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
            return (new Vault(id,
                name,
                description,
                createdAt,
                lastUpdatedAt,
                credentialList.ToArray(),
                auditLogEntriesList.ToArray()));
        }

        public object Write(
            object data, 
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            FormatVersionAttribute formatVersion = FormatVersionAttribute.GetAttributeFromType(this.GetType());

            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Writing Vault to JSON using serialiser '{0}'.", formatVersion);

            Dictionary<string, string> parametersDict = parameters.ToDictionary(x => x.Key, x => x.Value);

            Vault vault = (Vault)data;
            JObject json = new JObject();

            JObject header = new JObject();
            header.Add("FormatVersion", new JValue(formatVersion.Version));
            json.Add("Header", header);

            json.Add("ID", new JValue(vault.ID));
            json.Add("Name", new JValue(vault.Name));
            json.Add("Description", new JValue(vault.Description));
            json.Add("CreatedAt", new JValue(vault.CreatedAt.ToString(Common.DATETIMESERIALISATIONFORMAT)));
            json.Add("LastUpdatedAt", new JValue(vault.LastUpdatedAt.ToString(Common.DATETIMESERIALISATIONFORMAT)));

            ISerialiser credentialSerialiser = FormatVersions.Instance.GetSerialiser(formatVersion.Version, typeof(Credential));
            JArray credentialJSON = new JArray();
            foreach(Credential curCredential in vault.Credentials)
            {
                credentialJSON.Add(credentialSerialiser.Write(curCredential, String.Empty));
            }

            json.Add("Credentials", credentialJSON);
            ISerialiser auditLogEntrySerialiser = FormatVersions.Instance.GetSerialiser(formatVersion.Version, typeof(AuditLogEntry));
            JArray auditLogEntries = new JArray();
            foreach(AuditLogEntry curEntry in vault.AuditLogEntries)
            {
                auditLogEntries.Add(auditLogEntrySerialiser.Write(curEntry, String.Empty));
            }
            json.Add("AuditLogEntries", auditLogEntries);
            return (json);
        }

        #endregion

    }

}

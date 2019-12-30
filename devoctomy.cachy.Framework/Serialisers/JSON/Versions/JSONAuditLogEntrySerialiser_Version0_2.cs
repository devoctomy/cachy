using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Serialisers.Interfaces;
using devoctomy.DFramework.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static devoctomy.cachy.Framework.Data.AuditLogEntry;

namespace devoctomy.cachy.Framework.Serialisers.JSON.Versions
{

    [FormatVersion("0.2", typeof(AuditLogEntry))]
    public class JSONAuditLogEntrySerialiser_Version0_2 : ISerialiser
    {

        #region public methods

        public object Read(
            object data,
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            FormatVersionAttribute formatVersion = FormatVersionAttribute.GetAttributeFromType(this.GetType());

            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Reading AuditLogEntry from JSON using serialiser '{0}'.", formatVersion);

            JObject dataJSON = (JObject)data;
            EntryType entryType = (EntryType)Enum.Parse(typeof(EntryType), JSONHelpers.ReadString(dataJSON, "TypeOfEntry"), true);
            DateTime dateTime = JSONHelpers.ReadDateTime(dataJSON, "DateTime", Common.DATETIMESERIALISATIONFORMAT);
            Dictionary<String, String> paramsDictionary = new Dictionary<String, String>();
            JObject entyryParams = dataJSON["Parameters"].Value<JObject>();
            foreach (JToken curToken in entyryParams.Children())
            {
                JProperty curProperty = (JProperty)curToken;
                paramsDictionary.Add(curProperty.Name,
                    curProperty.Value.ToString());
            }
            AuditLogEntry entry = new AuditLogEntry(dateTime,
                entryType,
                paramsDictionary.ToArray());
            return (entry);
        }

        public object Write(
            object data,
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            FormatVersionAttribute formatVersion = FormatVersionAttribute.GetAttributeFromType(this.GetType());

            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Writing AuditLogEntry to JSON using serialiser '{0}'.", formatVersion);

            AuditLogEntry auditLogEntry = (AuditLogEntry)data;
            JObject json = new JObject();
            json.Add("DateTime", auditLogEntry.DateTime.ToString(Common.DATETIMESERIALISATIONFORMAT));
            json.Add("TypeOfEntry", auditLogEntry.TypeOfEntry.ToString());
            JObject paramaters = new JObject();
            foreach (String curKey in auditLogEntry.Parameters.Keys)
            {
                String curValue = auditLogEntry.Parameters[curKey];
                paramaters.Add(curKey, new JValue(curValue));
            }
            json.Add("Parameters", paramaters);
            return (json);
        }

        #endregion

    }

}

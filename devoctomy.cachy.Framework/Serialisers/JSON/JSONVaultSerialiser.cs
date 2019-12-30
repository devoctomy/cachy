using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Serialisers.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace devoctomy.cachy.Framework.Serialisers.JSON
{

    public class JSONVaultSerialiser : ISerialiser
    {

        #region public methods

        public object Read(
            object data, 
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Reading Vault from JObject.");

            JObject dataJSON = (JObject)data;
            JObject header = JSONHelpers.ReadJObject(dataJSON, "Header", true);
            string formatVersion = header["FormatVersion"].Value<String>();

            ISerialiser serialiser = FormatVersions.Instance.GetSerialiser(formatVersion, typeof(Vault));
            return (serialiser.Read(
                data,
                masterPassphrase, parameters));
        }

        public object Write(
            object data, 
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Writing JObject to Vault.");

            ISerialiser serialiser = FormatVersions.Instance.GetSerialiser(Common.LATEST_VAULT_VERSION, typeof(Vault));
            return(serialiser.Write(
                data,
                masterPassphrase,
                parameters));
        }

        #endregion

    }

}

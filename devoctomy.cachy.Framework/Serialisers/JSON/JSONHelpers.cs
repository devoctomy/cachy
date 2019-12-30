using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace devoctomy.cachy.Framework.Serialisers.JSON
{

    public sealed class JSONHelpers
    {

        #region public methods

        public static string ReadString(
            JObject json,
            string key,
            string defaultValue)
        {
            if (json.ContainsKey(key))
            {
                string value = json[key].Value<string>();
                return (value);
            }
            else
            {
                return (defaultValue);
            }
        }

        public static string ReadString(
            JObject json,
            string key)
        {
            if (json.ContainsKey(key))
            {
                return (ReadString(json, key, String.Empty));
            }
            else
            {
                throw new KeyNotFoundException(String.Format("The key '{0}' was not found in the JSON data.", key));
            }
        }

        public static DateTime ReadDateTime(
            JObject json,
            string key,
            DateTime defaultValue,
            string format = "dd-MM-yyyy HH:mm:ss")
        {
            if(json.ContainsKey(key))
            {
                try
                {
                    string value = json[key].Value<string>();
                    DateTime dateTime = DateTime.ParseExact(
                        value,
                        format,
                        System.Globalization.CultureInfo.InvariantCulture);
                    return (DateTime.SpecifyKind(dateTime, DateTimeKind.Utc));
                }
                catch (Exception)
                {
                    return (defaultValue);
                }
            }
            else
            {
                return (defaultValue);
            }
        }

        public static DateTime ReadDateTime(
            JObject json,
            string key,
            string format = "dd-MM-yyyy HH:mm:ss")
        {
            if (json.ContainsKey(key))
            {
                return (ReadDateTime(json, key, DateTime.MinValue, format));
            }
            else
            {
                throw new KeyNotFoundException(String.Format("The key '{0}' was not found in the JSON data.", key));
            }
        }

        public static JObject ReadJObject(
            JObject json,
            string key,
            bool throwExceptionWhenNotFound)
        {
            if (json.ContainsKey(key))
            {
                return (json[key].Value<JObject>());
            }
            else
            {
                if (throwExceptionWhenNotFound)
                {
                    throw new KeyNotFoundException(String.Format("The key '{0}' was not found in the JSON data.", key));
                }
                else
                {
                    return (null);
                }
            }
        }

        public static JArray ReadJArray(
            JObject json,
            string key,
            bool throwExceptionWhenNotFound)
        {
            if (json.ContainsKey(key))
            {
                return (json[key].Value<JArray>());
            }
            else
            {
                if (throwExceptionWhenNotFound)
                {
                    throw new KeyNotFoundException(String.Format("The key '{0}' was not found in the JSON data.", key));
                }
                else
                {
                    return (null);
                }
            }
        }

        #endregion

    }

}

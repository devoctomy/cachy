using devoctomy.cachy.Framework.Serialisers.JSON;
using devoctomy.DFramework.Core.SystemExtensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace devoctomy.cachy.Framework.Data
{

    public class AuditLogEntry : IComparable<AuditLogEntry>
    {

        #region public enums

        public enum EntryType
        {
            None = 0,
            CreatedVault = 1,
            CreatedCredential = 2,
            ModifyName = 3,
            ModifyDescription = 4,
            AddCredential = 5,
            ModifyGlyphKey = 6,
            ModifyGlyphColour = 7,
            ModifyNotes = 8,
            ModifyUsername = 9,
            ModifyPassword = 10,
            ModifyTags = 11,
            RemovedCredential = 12,
            Saved = 13
        }

        #endregion

        #region private objects

        private DateTime _dateTime;
        private EntryType _type = EntryType.None;
        private Dictionary<String, String> _parameters;

        #endregion

        #region public properties

        public DateTime DateTime
        {
            get
            {
                return (_dateTime);
            }
        }

        public String DateTimeAsLocalString
        {
            get
            {
                return (DateTime.ToLocalTime().ToString("HH:mm:ss dd-MM-yyyy"));
            }
        }

        public EntryType TypeOfEntry
        {
            get
            {
                return (_type);
            }
        }

        public String TypeOfEntryAsString
        {
            get
            {
                return (EntryTypeToReadableString(TypeOfEntry));
            }
        }

        public Dictionary<String, String> Parameters
        {
            get
            {
                return (_parameters);
            }
        }

        #endregion

        #region constructor / destructor

        public AuditLogEntry(DateTime dateTime,
            EntryType type,
            params KeyValuePair<String, String>[] parameters)
        {
            _dateTime = dateTime;
            _type = type;

            _parameters = parameters.ToDictionary(kvp => kvp.Key,
                kvp => kvp.Value);
        }

        public AuditLogEntry(EntryType type,
            params KeyValuePair<String, String>[] parameters)
        {
            _dateTime = DateTime.UtcNow;
            _dateTime = new DateTime(
                _dateTime.Year, 
                _dateTime.Month, 
                _dateTime.Day, 
                _dateTime.Hour, 
                _dateTime.Minute, 
                _dateTime.Second);  //All this to remove the milliseconds, although I'm not sure that really matters...
            _type = type;
            
            _parameters = parameters.ToDictionary(kvp => kvp.Key,
                kvp => kvp.Value);
        }

        #endregion

        #region private methods

        private void CheckParametersExist(params String[] keys)
        {
            foreach(String curKey in keys)
            {
                if(!_parameters.ContainsKey(curKey))
                {
                    throw new Exception(String.Format("Audit log entry is missing the parameter '{0}'.", curKey));
                }
            }
        }

        private String ReplaceProperties(String message)
        {
            String replacedMessage = message;
            foreach (String curKey in _parameters.Keys)
            {
                String curParam = "{" + String.Format("{0}", curKey) + "}";
                replacedMessage = replacedMessage.Replace(curParam, _parameters[curKey]);
            }
            return (replacedMessage);
        }

        #endregion

        #region public methods

        public String EntryTypeToReadableString(EntryType type)
        {
            switch (type)
            {
                case EntryType.CreatedVault:
                    {
                        CheckParametersExist("Version");

                        return (ReplaceProperties("Vault created with library version '{Version}'."));
                    }
                case EntryType.CreatedCredential:
                    {
                        return (ReplaceProperties("Credential created."));
                    }
                case EntryType.AddCredential:
                    {
                        CheckParametersExist("Name");

                        return (ReplaceProperties("Added credential '{Name}'"));
                    }
                case EntryType.ModifyDescription:
                    {
                        CheckParametersExist("Name");
                        CheckParametersExist("Type");
                        CheckParametersExist("PrevValue");

                        return (ReplaceProperties("Modified description for {Type} from '{PrevValue}' to '{Name}'."));
                    }
                case EntryType.ModifyGlyphKey:
                    {
                        CheckParametersExist("Name");

                        return (ReplaceProperties("Modified glyph for credential '{Name}'."));
                    }
                case EntryType.ModifyName:
                    {
                        CheckParametersExist("Name");
                        CheckParametersExist("Type");
                        CheckParametersExist("PrevValue");

                        return (ReplaceProperties("Modified name for {Type} from '{PrevValue}' to '{Name}'."));
                    }
                case EntryType.ModifyNotes:
                    {
                        CheckParametersExist("Name");

                        return (ReplaceProperties("Modified notes for credential '{Name}'."));
                    }
                case EntryType.ModifyPassword:
                    {
                        CheckParametersExist("Name");

                        return (ReplaceProperties("Modified password for credential '{Name}'."));
                    }
                case EntryType.ModifyTags:
                    {
                        CheckParametersExist("Name");

                        return (ReplaceProperties("Modified tags for credential '{Name}'."));
                    }
                case EntryType.ModifyUsername:
                    {
                        CheckParametersExist("Name");

                        return (ReplaceProperties("Modified username for credential '{Name}'."));
                    }
                case EntryType.RemovedCredential:
                    {
                        CheckParametersExist("Name");

                        return (ReplaceProperties("Removed credential '{Name}'."));
                    }
                case EntryType.Saved:
                    {
                        CheckParametersExist("Version");

                        return (ReplaceProperties("Vault saved with library version '{Version}'."));
                    }
                default:
                    {
                        return (String.Format("Unknown event '{0}'", type.ToString()));
                    }
            } 
        }

        #endregion

        #region icomparable

        public int CompareTo(AuditLogEntry other)
        {
            Int32 dateTimeCompare = DateTime.CompareTo(other.DateTime);
            if (dateTimeCompare != 0) return (dateTimeCompare);

            Int32 typeOfEntryCompare = TypeOfEntry.CompareTo(other.TypeOfEntry);
            if (dateTimeCompare != 0) return (dateTimeCompare);

            Int32 parametersCompare = Parameters.CompareTo(other.Parameters);
            if (parametersCompare != 0) return (parametersCompare);

            return (0);
        }

        #endregion

    }

}

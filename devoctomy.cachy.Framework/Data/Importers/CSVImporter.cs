using cachy.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.cachy.Framework.Data.Importers
{

    public class CSVImporter
    {

        #region private objects

        private string _csvData = String.Empty;
        private List<string> _headers;

        #endregion

        #region public properties

        public string CSVData
        {
            get
            {
                return (_csvData);
            }
        }

        public List<string> Headers
        {
            get
            {
                if (_headers == null)
                {
                    throw new InvalidOperationException("Must call ReadHeaders first.");
                }
                return (_headers);
            }
        }

        #endregion

        #region constructor / destructor

        public CSVImporter(string csvData)
        {
            _csvData = csvData;
        }

        #endregion

        #region public methods

        public async Task<List<string>> ReadHeaders()
        {
            if (_headers == null)
            {
                using (StringReader reader = new StringReader(CSVData))
                {
                    string headerValues = await reader.ReadLineAsync();
                    _headers = ParseCSVRow(headerValues);
                }
            }
            return (Headers);
        }

        public async Task<List<Dictionary<string, string>>> ReadRows()
        {
            List<Dictionary<string, string>> allRows = new List<Dictionary<string, string>>();
            using (StringReader reader = new StringReader(CSVData))
            {
                string headerRow = await reader.ReadLineAsync();
                _headers = ParseCSVRow(headerRow);

                string curRow = reader.ReadLine();
                while (!String.IsNullOrEmpty(curRow))
                {
                    Dictionary<string, string> row = new Dictionary<string, string>();
                    List<string> curRowValues = ParseCSVRow(curRow);
                    if(curRowValues.Count == Headers.Count)
                    {
                        for (int i = 0; i < Headers.Count; i++)
                        {
                            row.Add(Headers[i], curRowValues[i]);
                        }
                        allRows.Add(row);
                    }
                    curRow = reader.ReadLine();
                }
            }
            return (allRows);
        }

        public async Task ImportToVault(
            Vault vault,
            List<ImportFieldMapping> mappings)
        {
            List<Dictionary<string, string>> allRows = await ReadRows();
            foreach(Dictionary<string, string> curRow in allRows)
            {
                Credential credential = CreateCredentialFromRecord(
                    vault,
                    curRow,
                    mappings);
                credential.AddToVault(true);
            }              
        }

        public async Task<Common.CSVFormat> DetermineFormat()
        {
            List<string> headers = await ReadHeaders();

            List<string> keePassCSV1_x_Headers = new List<string>(new string[] { "Account", "Login Name", "Password", "Web Site", "Comments" });

            if (headers.SequenceEqual(keePassCSV1_x_Headers)) return (Common.CSVFormat.KeePassCSV1_x);

            return (Common.CSVFormat.Unknown);
        }

        public static List<ImportFieldMapping> CreateMappings(Common.CSVFormat format)
        {
            List<ImportFieldMapping> mappings = new List<ImportFieldMapping>();
            switch (format)
            {
                case Common.CSVFormat.KeePassCSV1_x:
                    {
                        mappings.Add(new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Name"), "Account"));
                        mappings.Add(new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Username"), "Login Name"));
                        mappings.Add(new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Password"), "Password"));
                        mappings.Add(new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Website"), "Web Site"));
                        mappings.Add(new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Notes"), "Comments"));
                        break;
                    }
            }
            return (mappings);
        }

        #endregion

        #region private methods

        private string CSVUnescape(string value)
        {
            string unescaped = value;
            unescaped = unescaped.Replace("\\\"", "\"");
            unescaped = unescaped.Replace("\\\\", "\\");
            return (unescaped);
        }

        private List<string> ParseCSVRow(string row)
        {
            List<string> values = new List<string>();
            StringBuilder curValue = new StringBuilder();
            bool readingValue = false;
            char? prevChar = null;
            bool quoted = false;
            for (int i = 0; i < row.Length; i++)
            {
                bool lastChar = i == row.Length - 1;
                char curChar = row[i];
                switch (curChar)
                {
                    case '\"':
                        {
                            if (curValue.Length == 0)
                            {
                                quoted = true;
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(curValue.ToString()))
                                {
                                    curValue.Clear();
                                    quoted = true;
                                }
                            }
                            if (quoted)
                            {
                                if (readingValue)
                                {
                                    bool endReadingValue = lastChar || (prevChar.HasValue && prevChar != '\\');
                                    if (endReadingValue)
                                    {
                                        values.Add(CSVUnescape(curValue.ToString()));
                                        curValue.Clear();
                                        readingValue = false;
                                        quoted = false;
                                    }
                                    else
                                    {
                                        curValue.Append(curChar);
                                    }
                                }
                                else
                                {
                                    readingValue = true;
                                }
                            }
                            else
                            {
                                curValue.Append(curChar);
                            }

                            break;
                        }
                    case ',':
                        {
                            if (quoted)
                            {
                                if (readingValue)
                                {
                                    curValue.Append(curChar);
                                }
                            }
                            else
                            {
                                if (curValue.Length > 0)
                                {
                                    values.Add(CSVUnescape(curValue.ToString().Trim(' ')));
                                }
                                curValue.Clear();
                            }
                            break;
                        }
                    default:
                        {
                            if (quoted)
                            {
                                if (readingValue)
                                {
                                    curValue.Append(curChar);
                                }
                            }
                            else
                            {
                                curValue.Append(curChar);
                            }
                            break;
                        }
                }
                prevChar = curChar;
            }
            if (curValue.Length > 0)
            {
                values.Add(CSVUnescape(curValue.ToString()));
            }
            return (values);
        }

        private Credential CreateCredentialFromRecord(
            Vault vault,
            Dictionary<string, string> record,
            List<ImportFieldMapping> mappings)
        {
            Credential credential = vault.CreateCredential();
            foreach (ImportFieldMapping curMapping in mappings)
            {
                string sourceValue = record[curMapping.ImportFieldName];
                string propertyName = curMapping.Attribute.Name;
                PropertyInfo targetproperty = typeof(Credential).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                targetproperty.SetValue(credential, sourceValue);
            }
            return (credential);
        }

        #endregion

    }

}

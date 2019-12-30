using cachy.Data;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace devoctomy.cachy.Framework.Data.Exporters
{

    public class VaultExporter
    {

        #region private objects

        private Common.ExportFormat _exportFormat = Common.ExportFormat.None;
        private Common.ExportWrapping _exportWrapping = Common.ExportWrapping.None;

        #endregion

        #region public properties

        public Common.ExportFormat ExportFormat
        {
            get
            {
                return (_exportFormat);
            }
        }

        public Common.ExportWrapping ExportWrapping
        {
            get
            {
                return (_exportWrapping);
            }
        }

        #endregion

        #region constructor / destructor

        public VaultExporter(
            Common.ExportFormat format,
            Common.ExportWrapping wrapping)
        {
            _exportFormat = format;
            _exportWrapping = wrapping;
        }

        #endregion

        #region private methods

        private string CSVEscape(string value)
        {
            string escaped = value;
            escaped = escaped.Replace("\\", "\\\\");
            escaped = escaped.Replace("\"", "\\\"");
            return (escaped);
        }

        private string CreateCSV(Vault vault)
        {
            StringBuilder output = new StringBuilder();
            List<StandardFieldAttribute> standardFields = Credential.StandardFields;
            IEnumerable<string> fieldNames = standardFields.Select(sfa => String.Format("\"{0}\"", sfa.Name));
            string headers = String.Join(",", fieldNames.ToArray());
            output.AppendLine(headers);
            foreach (Credential curCredential in vault.Credentials)
            {
                List<string> credentialProperties = new List<string>();
                foreach(StandardFieldAttribute curField in standardFields)
                {
                    PropertyInfo property = typeof(Credential).GetProperty(curField.Name);
                    object value = property.GetValue(curCredential);
                    string valueString = String.Format("\"{0}\"", CSVEscape(value.ToString()));
                    credentialProperties.Add(valueString);
                }
                string row = String.Join(",", credentialProperties);
                output.AppendLine(row);
            }
            return (output.ToString());
        }

        private MemoryStream CreateToMemoryStream(
            MemoryStream memStreamIn, 
            string zipEntryName,
            string password)
        {
            MemoryStream outputMemStream = new MemoryStream();
            ZipOutputStream zipStream = new ZipOutputStream(outputMemStream);
            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
            zipStream.Password = password;
            ZipEntry newEntry = new ZipEntry(zipEntryName);
            newEntry.DateTime = DateTime.Now;
            zipStream.PutNextEntry(newEntry);
            StreamUtils.Copy(memStreamIn, zipStream, new byte[4096]);
            zipStream.CloseEntry();
            zipStream.IsStreamOwner = false;
            zipStream.Close();
            outputMemStream.Position = 0;
            return outputMemStream;
        }

        #endregion

        #region public methods

        public byte[] Export(
            Vault vault,
            params KeyValuePair<string, string>[] parameters)
        {
            string rawCSV = CreateCSV(vault);
            switch(ExportWrapping)
            {
                case Common.ExportWrapping.Raw:
                    {
                        Byte[] exportData = Encoding.UTF8.GetBytes(rawCSV);
                        return (exportData);
                    }
                case Common.ExportWrapping.PasswordProtectedZip:
                    {
                        Dictionary<string, string> parametersDict = parameters.ToDictionary(x => x.Key, x => x.Value);
                        if (!parametersDict.ContainsKey("password")) throw new ArgumentException("Cannot generate password protected ZIP without a password parameter.");

                        string password = parametersDict["password"];

                        Byte[] exportData = Encoding.UTF8.GetBytes(rawCSV);
                        using (MemoryStream exportStream = new MemoryStream(exportData))
                        {
                            using (MemoryStream zipOutput = CreateToMemoryStream(
                                exportStream, 
                                "credentials.csv",
                                password))
                            {
                                return (zipOutput.ToArray());
                            }
                        }

                        throw new NotImplementedException("Export wrapping mode 'PasswordProtectedZip' not yet implemented.");
                    }
                default:
                    {
                        throw new NotSupportedException(String.Format("Export wrapping mode '{0}' not yet implemented.", ExportWrapping.ToString()));
                    }
            }
        }

        public static List<ImportFieldMapping> GetFieldMappings()
        {
            List<ImportFieldMapping> mappings = new List<ImportFieldMapping>();

            ImportFieldMapping name = new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Name"), "Name");
            ImportFieldMapping description = new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Description"), "Description");
            ImportFieldMapping website = new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Website"), "Website");
            ImportFieldMapping notes = new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Notes"), "Notes");
            ImportFieldMapping username = new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Username"), "Username");
            ImportFieldMapping password = new ImportFieldMapping(StandardFieldAttribute.Get(typeof(Credential), "Password"), "Password");

            mappings.AddRange(new ImportFieldMapping[]
            {
                name,
                description,
                website,
                notes,
                username,
                password
            });

            return (mappings);
        }

        #endregion

    }

}

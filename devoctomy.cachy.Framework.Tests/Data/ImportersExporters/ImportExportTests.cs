using System;
using System.Threading.Tasks;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Data.Exporters;
using devoctomy.cachy.Framework.Data.Importers;
using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace devoctomy.cachy.Tests.Data.ImportersExporters
{

    [TestClass]
    public class ImportExportTests
    {

        #region private objects

        private static Random _rng;

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

        [TestMethod]
        public async Task CreateVaultExportThenImport()
        {
            Vault sourceVault = new Vault();

            int count = 5000;

            for(int i = 0; i < count; i++)
            {
                Credential credential = CredentialTests.CreateRandomCredential(_rng);
                credential.AddToVault(sourceVault, false);
            }

            VaultExporter exporter = new VaultExporter(
                Common.ExportFormat.cachyCSV1_0, 
                Common.ExportWrapping.Raw);

            byte[] csv = exporter.Export(sourceVault);
            string csvString = System.Text.Encoding.UTF8.GetString(csv);

            Vault destVault = new Vault();

            CSVImporter importer = new CSVImporter(csvString);
            await importer.ImportToVault(destVault, VaultExporter.GetFieldMappings());

            for (int i = 0; i < count; i++)
            {
                Credential sourceCredential = sourceVault.Credentials[i];
                Credential destCredential = destVault.Credentials[i];
                Assert.IsTrue(sourceCredential.SimpleCompare(destCredential) == 0);
            }
        }

        #endregion

    }

}

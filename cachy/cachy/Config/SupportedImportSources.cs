using cachy.Fonts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace cachy.Config
{

    public class SupportedImportSources
    {

        #region private objects

        private static ObservableCollection<ImportSource> _supportedSources;

        #endregion

        #region public properties

        public static ObservableCollection<ImportSource> SupportedSources
        {
            get
            {
                if (_supportedSources == null)
                {
                    Initialise();
                }
                return (_supportedSources);
            }
        }

        #endregion


        #region private methods

        private static void Initialise()
        {
            _supportedSources = new ObservableCollection<ImportSource>();
            _supportedSources.Add(new ImportSource(devoctomy.cachy.Framework.Data.Common.CSVFormat.KeePassCSV1_x, "KeePass CSV", "1.x CSV File"));
            _supportedSources.Add(new ImportSource(devoctomy.cachy.Framework.Data.Common.CSVFormat.Unknown, "Custom CSV", "CSV with custom mappings"));
        }

        #endregion

    }

}

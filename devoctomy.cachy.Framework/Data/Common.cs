namespace devoctomy.cachy.Framework.Data
{

    public static class Common
    {

        #region enums

        public enum LoadResult
        {
            None = 0,
            Success = 1,
            UnknownError = 2,
            FileNotFound = 3
        }

        public enum SaveResult
        {
            None = 0,
            Success = 1,
            UnknownError = 2
        }

        public enum  SyncMode
        {
            None = 0,
            LocalOnly = 1,
            CloudProvider = 2
        }

        public enum PasswordDerivationMode
        {
            None = 0,
            PBKDF2 = 1,
            SCrypt = 2
        }

        public enum CSVFormat
        {
            None = 0,
            KeePassCSV1_x = 1,
            Unknown = 2
        }

        public enum ExportFormat
        {
            None = 0,
            cachyCSV1_0 = 1
        }

        public enum ExportWrapping
        {
            None = 0,
            Raw = 1,
            PasswordProtectedZip = 2
        }

        #endregion

    }

}

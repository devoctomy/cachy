using cachy.Native.Cryptography.Interfaces;
using devoctomy.cachy.Framework.Native.IO.Interfaces;
using devoctomy.cachy.Framework.Native.Web;

namespace devoctomy.cachy.Framework.Native
{

    public static class Native
    {

        #region public properties

        public static INativeFileHandler FileHandler { get; set; }

        public static INativeDataProtector DataProtector { get; set; }

        public static INativePasswordVault PasswordVault { get; set; }

        public static INativeWebUtility WebUtility { get; set; }

        #endregion

    }

}

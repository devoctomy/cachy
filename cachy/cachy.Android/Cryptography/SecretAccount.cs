using Javax.Crypto;

namespace cachy.Droid.Cryptography
{

    public class SecretAccount : Java.Lang.Object, ISecretKey
    {

        #region private objects

        byte[] bytes;

        #endregion

        #region public properties

        public string Algorithm
        {
            get
            {
                return "RAW";
            }
        }
        public string Format
        {
            get
            {
                return "RAW";
            }
        }

        #endregion

        #region constructor / destructor

        public SecretAccount(string password)
        {
            bytes = System.Text.Encoding.UTF8.GetBytes(password);
        }

        #endregion

        #region public methods

        public byte[] GetEncoded()
        {
            return bytes;
        }

        #endregion

        #region base class overrides

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

    }

}
using System;


namespace cachy.Data
{

    public class ClipboardObfuscatorGeneratedFakeEventArgs : EventArgs
    {

        #region private objects

        private string _value;

        #endregion

        #region public properties

        public string Value
        {
            get
            {
                return (_value);
            }
        }

        #endregion

        #region constructor / destructor

        public ClipboardObfuscatorGeneratedFakeEventArgs(string value)
        {
            _value = value;
        }

        #endregion

    }

}

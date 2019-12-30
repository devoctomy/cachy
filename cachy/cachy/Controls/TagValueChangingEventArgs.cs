using System;

namespace cachy.Controls
{

    public class TagValueChangingEventArgs : EventArgs
    {

        #region private objects

        private string _oldValue = String.Empty;
        private string _newValue = String.Empty;

        #endregion

        #region public properties

        public string OldValue
        {
            get
            {
                return (_oldValue);
            }
        }

        public string NewValue
        {
            get
            {
                return (_newValue);
            }
        }

        #endregion

        #region constructor / destructor

        public TagValueChangingEventArgs(
            string oldValue,
            string newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        #endregion

    }

}

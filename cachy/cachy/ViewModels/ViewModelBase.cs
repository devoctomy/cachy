using System;
using System.ComponentModel;

namespace cachy.ViewModels
{

    public class ViewModelBase : INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private bool _isValid;

        #endregion

        #region public properties

        public bool IsValid
        {
            get
            {
                return (_isValid);
            }
            set
            {
                if(_isValid != value)
                {
                    _isValid = value;
                    NotifyPropertyChanged("IsValid");
                }
                IsValidChanged(_isValid);
            }
        }

        #endregion

        #region public methods

        public void Validate()
        {
            IsValid = OnValidate();
        }

        #endregion

        #region protected methods

        protected virtual bool OnValidate()
        {
            return (true);
        }

        protected virtual void IsValidChanged(bool isValid)
        {
            //do nothing
        }

        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if(propertyName != "IsValid")
            {
                Validate();
            }
        }

        #endregion

    }

}

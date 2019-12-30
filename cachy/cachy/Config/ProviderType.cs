using cachy.Controls.Interfaces;
using System;
using System.ComponentModel;

namespace cachy.Config
{

    public class ProviderType : INotifyPropertyChanged, ISelectableItem
    {

        #region public enums

        public enum AuthenticationType
        {
            None = 0,
            OAuth = 1,
            Amazon = 2
        }

        #endregion

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private AuthenticationType _authType = AuthenticationType.None;
        private string _glyphText = String.Empty;
        private string _name = String.Empty;
        private string _website = String.Empty;
        private bool _isSelected;

        #endregion

        #region public properties

        public AuthenticationType AuthType
        {
            get
            {
                return (_authType);
            }
            set
            {
                if(_authType != value)
                {
                    _authType = value;
                    NotifyPropertyChanged("AuthType");
                }
            }
        }

        public string GlyphText
        {
            get
            {
                return (_glyphText);
            }
            set
            {
                if(_glyphText != value)
                {
                    _glyphText = value;
                    NotifyPropertyChanged("GlyphText");
                }
            }
        }

        public string Name
        {
            get
            {
                return (_name);
            }
            set
            {
                if(_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Website
        {
            get
            {
                return (_website);
            }
            set
            {
                if(_website != value)
                {
                    _website = value;
                    NotifyPropertyChanged("Website");
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return (_isSelected);
            }
            set
            {
                if(_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        #endregion

        #region constructor / destructor

        public ProviderType(
            AuthenticationType authType,
            string glyphText,
            string name,
            string website)
        {
            _authType = authType;
            _glyphText = glyphText;
            _name = name;
            _website = website;
        }

        #endregion

        #region private methods

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

}

using cachy.Controls.Interfaces;
using cachy.Data;
using devoctomy.cachy.Framework.Data;
using System;
using System.ComponentModel;

namespace cachy.Config
{

    public class ImportSource : INotifyPropertyChanged, ISelectableItem
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private Common.CSVFormat _format = Common.CSVFormat.None;
        private string _name = String.Empty;
        private string _description = String.Empty;
        private bool _isSelected;

        #endregion

        #region public properties

        public Common.CSVFormat Format
        {
            get
            {
                return (_format);
            }
            set
            {
                if (_format != value)
                {
                    _format = value;
                    NotifyPropertyChanged("Format");
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
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public string Description
        {
            get
            {
                return (_description);
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
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

        public ImportSource(
            Common.CSVFormat format,
            string name,
            string description)
        {
            _format = format;
            _name = name;
            _description = description;
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

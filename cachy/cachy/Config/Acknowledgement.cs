using cachy.Controls.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace cachy.Config
{

    public class Acknowledgement : ISelectableItem, INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private string _name = String.Empty;
        private string _description = String.Empty;
        private string _website = String.Empty;
        private bool _isSelected;

        #endregion

        #region public properties

        public string Name
        {
            get
            {
                return (_name);
            }
        }

        public string Description
        {
            get
            {
                return (_description);
            }
        }

        public string Website
        {
            get
            {
                return (_website);
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
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        #endregion

        #region constructor / destructor

        private Acknowledgement(
            string name,
            string description,
            string website)
        {
            _name = name;
            _description = description;
            _website = website;
        }

        #endregion

        #region public methods

        public static Acknowledgement FromJSON(JObject json)
        {
            string name = json["Name"].Value<string>();
            string description = json["Description"].Value<string>();
            string website = json["Website"].Value<string>();
            return (new Acknowledgement(
                name,
                description,
                website));
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

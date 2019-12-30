using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.Controls
{

    public class ColourPickerItem : INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private string _name = String.Empty;
        private Color _colour;
        private ICommand _selectCommand;

        #endregion

        #region public properties

        public string Name
        {
            get
            {
                return (_name);
            }
        }

        public Color Colour
        {
            get
            {
                return (_colour);
            }
        }

        public ICommand SelectCommand
        {
            get
            {
                return (_selectCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public ColourPickerItem(string name,
            Color colour,
            ICommand selectCommand)
        {
            _name = name;
            _colour = colour;
            _selectCommand = selectCommand;
        }

        #endregion

        #region private methods

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region base class overrides

        public override string ToString()
        {
            return (Name);
        }

        #endregion

    }

}

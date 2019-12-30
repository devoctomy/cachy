using cachy.Controls;
using cachy.Fonts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.Navigation.BurgerMenu
{

    public class BurgerMenuItem : INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private BurgerMenuHostPage _hostPage;
        private string _id = Guid.NewGuid().ToString();
        private string _key = String.Empty;
        private string _menuTitle = String.Empty;
        private CachyFont.Glyph _glyph = CachyFont.Glyph.None;
        private bool _isVisible;
        private bool _isEnabled = true;
        private ObservableCollection<BurgerMenuItem> _childItems;
        private ICommand _menuItemCommand;
        private GlyphButton.ButtonMode _buttonMode = GlyphButton.ButtonMode.GlyphOnly;

        #endregion

        #region public properties

        public BurgerMenuHostPage HostPage
        {
            get
            {
                return (_hostPage);
            }
        }

        public string ID
        {
            get
            {
                return (_id);
            }
        }

        public string Key
        {
            get
            {
                return (_key);
            }
        }

        public string MenuTitle
        {
            get
            {
                return (_menuTitle);
            }
            set
            {
                if (_menuTitle != value)
                {
                    _menuTitle = value;
                    NotifyPropertyChanged("MenuTitle");
                }
            }
        }

        public CachyFont.Glyph Glyph
        {
            get
            {
                return (_glyph);
            }
            set
            {
                if(_glyph != value)
                {
                    _glyph = value;
                    NotifyPropertyChanged("Glyph");
                }
            }
        }

        public bool IsVisible
        {
            get
            {
                return (_isVisible);
            }
            set
            {
                if(_isVisible != value)
                {
                    _isVisible = value;
                    NotifyPropertyChanged("IsVisible");
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                return (_isEnabled);
            }
            set
            {
                if(_isEnabled != value)
                {
                    _isEnabled = value;
                    NotifyPropertyChanged("IsEnabled");
                }
            }
        }

        public ObservableCollection<BurgerMenuItem> ChildItems
        {
            get
            {
                return (_childItems);
            }
        }

        public ICommand MenuItemCommand
        {
            get
            {
                return (_menuItemCommand);
            }
        }

        public GlyphButton.ButtonMode ButtonMode
        {
            get
            {
                return (_buttonMode);
            }
            set
            {
                if(_buttonMode != value)
                {
                    _buttonMode = value;
                    NotifyPropertyChanged("ButtonMode");
                    NotifyPropertyChanged("ButtonHorizontalOptions");
                }
            }
        }

        public LayoutOptions ButtonHorizontalOptions
        {
            get
            {
                return (ButtonMode == GlyphButton.ButtonMode.GlyphOnly ? LayoutOptions.Start : LayoutOptions.Fill);
            }
        }

        #endregion

        #region constructor / destructor

        public BurgerMenuItem(string key,
            string menuTitle,
            CachyFont.Glyph glyph,
            bool isVisible)
        {
            _key = key;
            _menuTitle = menuTitle;
            _glyph = glyph;
            _isVisible = isVisible;
            _childItems = new ObservableCollection<BurgerMenuItem>();
        }

        #endregion

        #region public methods

        public BurgerMenuItem GetChildItemByKey(string key)
        {
            IEnumerable<BurgerMenuItem> matchingChildren = ChildItems.Where(bmi => bmi.Key.EndsWith(key));
            if(matchingChildren.Any())
            {
                return (matchingChildren.First());
            }
            else
            {
                return (null);
            }
        }

        public void AttachCommand(BurgerMenuHostPage hostPage,
            ICommand menuItemCommand)
        {
            _hostPage = hostPage;
            _menuItemCommand = menuItemCommand;
            NotifyPropertyChanged("IsCommon");
            NotifyPropertyChanged("MenuItemCommand");
        }

        #endregion

        #region protected methods

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

}

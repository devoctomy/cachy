using cachy.Fonts;
using cachy.Views;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.Navigation.BurgerMenu
{

    public class BurgerMenuViewItem : BurgerMenuItem
    {

        #region private objects

        private string _pageTitle = String.Empty;
        private Type _pageViewType;
        private View _pageViewInstance;
        private bool _clearInstanceOnNavigate;

        #endregion

        #region public properties

        public string PageTitle
        {
            get
            {
                return (_pageTitle);
            }
            set
            {
                if (_pageTitle != value)
                {
                    _pageTitle = value;
                    NotifyPropertyChanged("PageTitle");
                }
            }
        }

        public Type PageViewType
        {
            get
            {
                return (_pageViewType);
            }
        }

        public View PageViewInstance
        {
            get
            {
                if (_pageViewInstance == null)
                {
                    _pageViewInstance = App.Controller.CreateViewInstance(PageViewType);
                    PageNavigationAwareView view = _pageViewInstance as PageNavigationAwareView;
                    if (view != null)
                    {
                        view.Parameters.Add("BurgerMenuHostPage", HostPage);
                    }
                }
                return (_pageViewInstance);
            }
        }

        public bool ClearInstanceOnNavigate
        {
            get
            {
                return (_clearInstanceOnNavigate);
            }
        }

        #endregion

        #region constructor / destructor

        public BurgerMenuViewItem(string key,
            string menuTitle,
            CachyFont.Glyph glyph,
            string pageTitle,
            Type pageViewType,
            bool clearInstanceOnNavigate) :
            base(key, menuTitle, glyph, true)
        {
            _pageTitle = pageTitle;
            _pageViewType = pageViewType;
            _clearInstanceOnNavigate = clearInstanceOnNavigate;
        }

        public BurgerMenuViewItem(string key,
            string pageTitle,
            Type pageViewType,
            bool clearInstanceOnNavigate) :
            base(key, String.Empty, CachyFont.Glyph.None, false)
        {
            _pageTitle = pageTitle;
            _pageViewType = pageViewType;
            _clearInstanceOnNavigate = clearInstanceOnNavigate;
        }

        #endregion

        #region public methods

        public void ClearInstance()
        {
            _pageViewInstance = null;
        }

        public BurgerMenuCommandItem AddChildCommandItem(string key,
            string menuTitle,
            CachyFont.Glyph glyph,
            string methodName)
        {
            BurgerMenuCommandItem commandItem = new BurgerMenuCommandItem(string.Format("{0}.{1}", Key, key),
                menuTitle,
                glyph,
                this,
                methodName);
            ChildItems.Add(commandItem);
            return (commandItem);
        }

        public BurgerMenuCommandItem AddChildCommandItem(BurgerMenuCommandItem item)
        {
            ChildItems.Add(item);
            return (item);
        }

        public BurgerMenuViewItem AddChildViewItem(string key,
            string menuTitle,
            CachyFont.Glyph glyph,
            string pageTitle,
            Type pageViewType,
            bool clearInstanceOnNavigate)
        {
            BurgerMenuViewItem viewItem = new BurgerMenuViewItem(string.Format("{0}.{1}", Key, key),
                menuTitle,
                glyph,
                pageTitle,
                pageViewType,
                clearInstanceOnNavigate);
            ChildItems.Add(viewItem);
            return (viewItem);
        }

        public BurgerMenuViewItem AddChildViewItem(BurgerMenuViewItem item)
        {
            ChildItems.Add(item);
            return (item);
        }

        public BurgerMenuPopupViewItem AddChildPopupItem(string key,
            Type pageViewType)
        {
            BurgerMenuPopupViewItem popupItem = new BurgerMenuPopupViewItem(string.Format("{0}.{1}", Key, key),
                pageViewType);
            ChildItems.Add(popupItem);
            return (popupItem);
        }

        #endregion

    }

}

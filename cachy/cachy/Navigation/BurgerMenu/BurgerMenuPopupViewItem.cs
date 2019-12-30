using cachy.Fonts;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.Navigation.BurgerMenu
{

    public class BurgerMenuPopupViewItem : BurgerMenuItem
    {

        #region private objects

        private Type _popupViewType;
        private View _popupViewInstance;

        #endregion

        #region public properties

        public Type PopupViewType
        {
            get
            {
                return (_popupViewType);
            }
        }

        public View PopupViewInstance
        {
            get
            {
                if (_popupViewInstance == null)
                {
                    _popupViewInstance = App.Controller.CreateViewInstance(PopupViewType);
                }
                return (_popupViewInstance);
            }
        }

        #endregion

        #region constructor / destructor

        public BurgerMenuPopupViewItem(string key,
            string menuTitle,
            Type pageViewType) :
            base(key, menuTitle, CachyFont.Glyph.None, true)
        {
            _popupViewType = pageViewType;
        }

        public BurgerMenuPopupViewItem(string key,
            Type pageViewType) :
            base(key, String.Empty, CachyFont.Glyph.None, false)
        {
            _popupViewType = pageViewType;
        }

        #endregion

    }


}

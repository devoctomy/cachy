using cachy.Navigation.BurgerMenu;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace cachy.Navigation
{

    public class NavigationManager
    {

        #region private objects

        private static NavigationManager _instance;
        private Stack<BurgerMenuViewItem> _viewItemHistory;
        private BurgerMenuViewItem _previousViewItem;

        #endregion

        #region public properties

        public static NavigationManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new NavigationManager();
                }
                return (_instance);
            }
        }

        public BurgerMenuViewItem CurrentPage
        {
            get
            {
                return (_viewItemHistory.Count > 0 ? _viewItemHistory.Peek() : null);
            }
        }

        public BurgerMenuViewItem PreviousPage
        {
            get
            {
                return (_previousViewItem);
            }
        }

        #endregion

        #region constructor / destructor

        public NavigationManager()
        {
            _viewItemHistory = new Stack<BurgerMenuViewItem>();
        }

        #endregion

        #region public methods

        public void NavigatedTo(BurgerMenuViewItem item)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "NavigationManager pushing item to history.");

            if (_viewItemHistory.Count > 0) _previousViewItem = _viewItemHistory.Peek();
            if(_previousViewItem != null)
            {
                if (_previousViewItem.HostPage.CommonItems.Contains(_previousViewItem) &&
                    item.HostPage.CommonItems.Contains(item))
                {
                    _viewItemHistory.Pop();                         //Let's remove the previous item
                    if (_viewItemHistory.Count > 0) _previousViewItem = _viewItemHistory.Peek();
                }
            }
            _viewItemHistory.Push(item);
        }

        public BurgerMenuViewItem GoBack()
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "NavigationManager going back.");

            if (_viewItemHistory.Count > 1)
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "NavigationManager popping previous page from history.");
                _viewItemHistory.Pop();
                return (_viewItemHistory.Peek());
            }
            else
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "NavigationManager has no pages in history to go back to.");
                return (null);
            }
        }

        #endregion

    }

}

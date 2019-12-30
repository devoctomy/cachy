using cachy.Navigation;
using cachy.Navigation.BurgerMenu;
using cachy.Views;
using devoctomy.DFramework.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace cachy
{

    public class AppController<MainPageType> where MainPageType : BurgerMenuHostPage
    {

        #region private objects

        private App _app;
        private MainPageType _mainPageInstance;
        private Func<MainPageType> _setupMainPage;
        private Dictionary<Type, Object> _pageInstances;

        #endregion

        #region public properties

        public MainPageType MainPageInstance
        {
            get
            {
                return (_mainPageInstance);
            }
        }

        #endregion

        #region constructor / destructor

        public AppController(App app,
            Func<MainPageType> setupMainPage)
        {
            _app = app;
            _setupMainPage = setupMainPage;
            _pageInstances = new Dictionary<Type, Object>();
        }

        #endregion

        #region private methods

        private void Initialise()
        {
            Task.Run(() => Config.Dictionaries.Initialise());    //Pre-load the dictionaries
            DisplayMainPage();
        }

        private BaseType CreateInstance<BaseType>(Type derviedType)
        {
            Object instance = Activator.CreateInstance(derviedType);
            return ((BaseType)instance);
        }

        #endregion

        #region public methods

        public void NavigateTo(string key)
        {
            NavigateTo(key, null);
        }

        public void DisplayMainPage()
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "AppController DisplayMainMenu.");

            _mainPageInstance = (MainPageType)_setupMainPage();
            _app.MainPage = _mainPageInstance;
        }

        public PageNavigationAwareView CreateViewInstance(Type viewType)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "AppController CreateViewInstance of type '{0}'.", viewType.Name);

            PageNavigationAwareView viewInstance = null;
            if (viewType.IsSubclassOf(typeof(PageNavigationAwareView)))
            {
                viewInstance = CreateInstance<PageNavigationAwareView>(viewType);
            }
            else
            {
                throw new NotImplementedException(String.Format("View type of '{0}' is not supported.", viewType.Name));
            }
            return (viewInstance);
        }

        public void NavigateTo(string key, params KeyValuePair<String, Object>[] parameters)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "AppController OnNavigateTo '{0}'.", key);

            BurgerMenuViewItem item;
            _mainPageInstance.DisplayPage(key, out item);
            if (item != null)
            {
                if(item.ClearInstanceOnNavigate)
                {
                    item.ClearInstance();
                }
                PageNavigationAwareView view = (PageNavigationAwareView)item.PageViewInstance;
                view.InitialiseBindingContext();
                if (parameters != null && parameters.Length > 0)
                {
                    view.SetParameters(parameters);
                }
                view.OnNavigateTo(item);
            }
        }

        public void ShowPopup(string key, params KeyValuePair<String, Object>[] parameters)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "AppController ShowPopup '{0}'.", key);

            BurgerMenuPopupViewItem item;
            _mainPageInstance.DisplayPopup(key, out item);
            if (item != null)
            {
                PageNavigationAwareView view = (PageNavigationAwareView)item.PopupViewInstance;
                view.InitialiseBindingContext();
                if (parameters != null && parameters.Length > 0)
                {
                    view.SetParameters(parameters);
                }
                view.OnNavigateTo(_mainPageInstance.SelectedItem, false);
            }
        }

        public void ClosePopup(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "AppController ClosePopup.");

            BurgerMenuPopupViewItem popupItem = _mainPageInstance.SelectedPopupItem;
            if(popupItem != null)
            {
                PageNavigationAwareView view = (PageNavigationAwareView)popupItem.PopupViewInstance;
                view.OnNavigateFrom();

                _mainPageInstance.ClosePopup();

                if (_mainPageInstance.PopupsVisible)
                {
                    PageNavigationAwareView selectedPopup = _mainPageInstance.SelectedPopupItem.PopupViewInstance as PageNavigationAwareView;
                    selectedPopup.OnClosePopup(popupItem.PopupViewInstance, parameter);
                }
                else
                {
                    PageNavigationAwareView selectedView = _mainPageInstance.SelectedItem.PageViewInstance as PageNavigationAwareView;
                    selectedView.OnClosePopup(popupItem.PopupViewInstance, parameter);
                }
            }
        }

        public void GoBack()
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "AppController GoBack.");

            BurgerMenuViewItem prevItem = NavigationManager.Instance.GoBack();
            if(prevItem != null)
            {
                _mainPageInstance.SelectItem(prevItem);
                PageNavigationAwareView view = (PageNavigationAwareView)prevItem.PageViewInstance;
                view.OnGoBack();
            }
        }

        public void Start(params KeyValuePair<String,Object>[] parameters)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "AppController Start.");

            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "AppController:Start");
            Initialise();
            BurgerMenuItem startItem = ((BurgerMenuHostPage)_mainPageInstance).StartItem;
            NavigateTo(startItem.Key, parameters);
        }

        public void Sleep()
        {
            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "AppController:Sleep");
        }

        public void Resume()
        {
            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "AppController:Resume");
        }

        #endregion

    }

}

using cachy.Navigation;
using cachy.Navigation.BurgerMenu;
using cachy.ViewModels;
using cachy.ViewModels.Interfaces;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace cachy.Views
{

    public class PageNavigationAwareView : ContentView
    {

        #region private objects

        private Dictionary<string, object> _parameters;

        #endregion

        #region public properties

        public Dictionary<string, object> Parameters
        {
            get
            {
                return (_parameters);
            }
        }

        #endregion

        #region constructor / destructor

        public PageNavigationAwareView()
        {
            _parameters = new Dictionary<string, object>();
        }

        #endregion

        #region private methods

        private void DoNavigateFrom()
        {
            BurgerMenuViewItem previousPage = NavigationManager.Instance.CurrentPage;
            if(previousPage != null)
            {
                PageNavigationAwareView view = previousPage.PageViewInstance as PageNavigationAwareView;
                if (view != null)
                {
                    view.OnNavigateFrom();
                }
            }
        }

        #endregion

        #region public methods

        public void InitialiseBindingContext()
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "PageNavigationAwareView InitialiseBindingContext.");

            BindingContext = ViewModelManager.Instance.GetViewModel(this);
        }

        public void OnClosePopup(View item, object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "PageNavigationAwareView OnClosePopup.");

            IPageNavigationAware navigationAware = BindingContext as IPageNavigationAware;
            if (navigationAware != null)
            {
                navigationAware.OnClosePopup(item, parameter);
            }
        }

        public void OnNavigateTo(BurgerMenuViewItem item)
        {
            OnNavigateTo(item, true);
        }

        public void OnNavigateTo(BurgerMenuViewItem item,
            bool passToNavigationManager)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "PageNavigationAwareView OnNavigateTo.");

            DoNavigateFrom();
            if (passToNavigationManager) NavigationManager.Instance.NavigatedTo(item);
            IPageNavigationAware navigationAware = BindingContext as IPageNavigationAware;
            if (navigationAware != null)
            {
                navigationAware.OnNavigateTo(this, item);
            }
        }

        public void OnNavigateFrom()
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "PageNavigationAwareView OnNavigateFrom.");

            IPageNavigationAware navigationAware = BindingContext as IPageNavigationAware;
            if (navigationAware != null)
            {
                navigationAware.OnNavigateFrom(this, null);
            }
        }

        public void OnGoBack()
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "PageNavigationAwareView OnGoBack.");

            IPageNavigationAware navigationAware = BindingContext as IPageNavigationAware;
            if (navigationAware != null)
            {
                navigationAware.OnGoBack(this, null);
            }
        }

        public void SetParameters(params KeyValuePair<String,Object>[] parameters)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "PageNavigationAwareView SetParameters.");

            IPageNavigationAware navigationAware = BindingContext as IPageNavigationAware;
            if (navigationAware != null)
            {
                navigationAware.SetParameters(parameters);
            }
        }

        public void SetParameter(String key,
            Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh, "PageNavigationAwareView SetParameter '{0}'.", key);

            IPageNavigationAware navigationAware = BindingContext as IPageNavigationAware;
            if (navigationAware != null)
            {
                navigationAware.SetParameter(key, parameter);
            }
        }

        #endregion

    }
}

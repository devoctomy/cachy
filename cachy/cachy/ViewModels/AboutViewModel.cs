using cachy.ViewModels.Interfaces;
using cachy.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class AboutViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private AboutView _view;

        private ICommand _helpCenterCommand;

        #endregion

        #region public properties

        public AboutView View
        {
            get
            {
                return (_view);
            }
        }

        public string VersionText
        {
            get
            {
                return (string.Format("Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            }
        }

        public ICommand HelpCenterCommand
        {
            get
            {
                return (_helpCenterCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public AboutViewModel(AboutView view)
        {
            _view = view;
            _helpCenterCommand = new Command(new Action<object>(HelpCenterCommandAction));
        }

        #endregion

        #region public methods

        public void Acknowledgements(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Acknowledgements invoked.");

            App.Controller.ShowPopup("about.acknowledgements");
        }

        public void Changes(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Changes invoked.");

            App.Controller.ShowPopup("about.changes");
        }

        public void Cancel(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Cancel invoked.");

            App.Controller.GoBack();
        }

        #endregion

        #region commands

        private void HelpCenterCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Help center command invoked.");

            Device.OpenUri(new Uri(AppConstants.DEVOCTOMY_SUPPORT_URL));
        }

        #endregion

        #region base class overrides

        protected override bool OnValidate()
        {
            return(true);
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            //No parameters to set in this page
        }

        public void SetParameters(params KeyValuePair<String, Object>[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                foreach (KeyValuePair<String, Object> curParameter in parameters)
                {
                    SetParameter(curParameter.Key,
                        curParameter.Value);
                }
            }
        }

        public void OnClosePopup(View item, object parameter)
        {

        }

        public void OnNavigateTo(View view, object parameter)
        {

        }

        public void OnNavigateFrom(View view, object parameter)
        {

        }

        public void OnGoBack(View view, object parameter)
        {

        }

        #endregion

    }

}

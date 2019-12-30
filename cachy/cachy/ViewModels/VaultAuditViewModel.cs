using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class VaultAuditViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private VaultAuditView _view;
        private Vault _vault;

        private ICommand _backCommand;
        private ICommand _clearAuditLogCommand;

        #endregion

        #region public properties

        public VaultAuditView View
        {
            get
            {
                return (_view);
            }
        }

        public Vault Vault
        {
            get
            {
                return (_vault);
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return (_backCommand);
            }
        }

        public ICommand ClearAuditLogCommand
        {
            get
            {
                return (_clearAuditLogCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public VaultAuditViewModel(VaultAuditView view)
        {
            _view = view;

            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _clearAuditLogCommand = new Command(new Action<Object>(ClearAuditLogCommandAction));
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key,
            Object parameter)
        {
            switch (key)
            {
                case "Vault":
                    {
                        _vault = (Vault)parameter;
                        NotifyPropertyChanged("Vault");
                        break;
                    }
            }
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

        #region public methods

        public void BackCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Back command invoked.");

            App.Controller.ClosePopup(parameter);
        }

        public async void ClearAuditLogCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Clear audit log command invoked.");

            if (await App.Controller.MainPageInstance.DisplayAlert("Clear Audit Log",
                "Are you sure you want to clear the vault audit log? Please note, you will no longer see audit history before this point.",
                "Yes", "No"))
            {
                Vault.ClearAuditEntries();
                App.Controller.ClosePopup(parameter);
            }
        }

        #endregion

    }

}

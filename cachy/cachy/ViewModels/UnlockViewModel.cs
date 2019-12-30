using cachy.Controls;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class UnlockViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private UnlockView _view;
        private VaultIndex _vaultIndex;
        private String _masterPassphrase = String.Empty;
        private bool _showMasterPassphrase;

        private ICommand _backCommand;
        private ICommand _acceptCommand;
        private ICommand _completedCommand;

        #endregion

        #region public properties

        public VaultIndex VaultIndex
        {
            get
            {
                return (_vaultIndex);
            }
        }

        public String MasterPassphrase
        {
            get
            {
                return (_masterPassphrase);
            }
            set
            {
                if (_masterPassphrase != value)
                {
                    _masterPassphrase = value;
                    NotifyPropertyChanged("MasterPassphrase");
                }
            }
        }

        public bool ShowMasterPassphrase
        {
            get
            {
                return (_showMasterPassphrase);
            }
            set
            {
                if(_showMasterPassphrase != value)
                {
                    _showMasterPassphrase = value;
                    NotifyPropertyChanged("ShowMasterPassphrase");
                }
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return (_backCommand);
            }
        }

        public ICommand AcceptCommand
        {
            get
            {
                return (_acceptCommand);
            }
        }

        public ICommand CompletedCommand
        {
            get
            {
                return (_completedCommand);
            }
        }

        public UnlockView View
        {
            get
            {
                return (_view);
            }
        }

        #endregion

        #region constructor / destructor

        public UnlockViewModel(UnlockView view)
        {
            _view = view;

            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acceptCommand = new Command(new Action<Object>(AcceptCommandAction));
            _completedCommand = new Command(new Action<Object>(CompletedCommandAction));
        }

        #endregion

        #region base class overrides

        protected override bool OnValidate()
        {
            if (String.IsNullOrEmpty(MasterPassphrase)) return (false);

            return (true);
        }

        protected override void IsValidChanged(bool isValid)
        {
            EnableDisableAccept(isValid);
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key,
            Object parameter)
        {
            switch (key)
            {
                case "VaultIndex":
                    {
                        _vaultIndex = (VaultIndex)parameter;
                        ((App)App.Current).OpenedVault = _vaultIndex;
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
            EnableDisableAccept(IsValid);
        }

        public void OnNavigateTo(View view, object parameter)
        {
            ((App)App.Current).AndroidSetSoftInputMode(true);
            EnableDisableAccept(IsValid);
        }

        public void OnNavigateFrom(View view, object parameter)
        {
            ((cachy.App)App.Current).AndroidSetSoftInputMode(false);
        }

        public void OnGoBack(View view, object parameter)
        {

        }

        #endregion

        #region private methods

        private void EnableDisableAccept(bool enabled)
        {
            GlyphButton accept = View.FindByName<GlyphButton>("AcceptButton");
            if (accept != null)
            {
                accept.IsEnabled = enabled;
            }
        }

        private async Task PerformUnlock()
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Attempting Vault unlock.");

            if (_vaultIndex != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("VaultIndex", VaultIndex);
                parameters.Add("MasterPassphrase", MasterPassphrase);
                App.Controller.ClosePopup(parameters);
            }
            else
            {
                await App.Controller.MainPageInstance.DisplayAlert("Unlock Vault",
                    "No vault has been selected by the viewmodel to unlock.",
                    "OK");
            }
        }

        #endregion

        #region commands

        public async void CompletedCommandAction(Object paramater)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Completed command invoked.");

            await PerformUnlock();
        }

        public void BackCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Back command invoked.");

            App.Controller.ClosePopup(null);
        }

        public async void AcceptCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Accept command invoked.");

            await PerformUnlock();
        }

        #endregion

    }

}

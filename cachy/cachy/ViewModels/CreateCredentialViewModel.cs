using cachy.Config;
using cachy.Controls;
using cachy.Navigation.BurgerMenu;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class CreateCredentialViewModel : ViewModelBase, IPageNavigationAware
    {

        #region public enums

        public enum EditorMode
        {
            None = 0,
            Create = 1,
            Edit = 2
        }

        #endregion

        #region private objects

        private CreateCredentialView _view;
        private String _confirmPassword = String.Empty;
        private bool _showMasterPassphrase;
        private Credential _credential;
        private EditorMode _mode = EditorMode.Create;

        private ICommand _selectGlyphCommand;
        private ICommand _generateCommand;
        private ICommand _addNewTagCommand;
        private ICommand _clearTagsCommand;

        #endregion

        #region public properties

        public String PageTitle
        {
            get
            {
                switch(_mode)
                {
                    case EditorMode.Create:
                        {
                            return ("Create Credential");
                        }
                    case EditorMode.Edit:
                        {
                            return ("Edit Credential");
                        }
                    default:
                        {
                            throw new InvalidOperationException(String.Format("Unknown editor mode '{0}'.", _mode.ToString()));
                        }
                }
            }
        }

        public String ConfirmPassword
        {
            get
            {
                return (_confirmPassword);
            }
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
                    NotifyPropertyChanged("ConfirmPassword");
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

        public ICommand SelectGlyphCommand
        {
            get
            {
                return (_selectGlyphCommand);
            }
        }

        public ICommand GenerateCommand
        {
            get
            {
                return (_generateCommand);
            }
        }

        public CreateCredentialView View
        {
            get
            {
                return (_view);
            }
        }

        public Credential Credential
        {
            get
            {
                return (_credential);
            }
        }

        public ICommand AddNewTagCommand
        {
            get
            {
                return (_addNewTagCommand);
            }
        }

        public ICommand ClearTagsCommand
        {
            get
            {
                return (_clearTagsCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public CreateCredentialViewModel(CreateCredentialView view)
        {
            _view = view;

            _selectGlyphCommand = new Command(new Action<Object>(ChangeGlyphIcon));
            _generateCommand = new Command(new Action<Object>(GenerateCommandAction));
            _addNewTagCommand = new Command<object>(new Action<object>(AddNewTagCommandAction));
            _clearTagsCommand = new Command<object>(new Action<object>(ClearTagsCommandAction));
        }

        #endregion

        #region base class overrides

        protected override bool OnValidate()
        {
            if (String.IsNullOrEmpty(Credential.Name)) return (false);
            if (String.IsNullOrEmpty(Credential.Password)) return (false);
            if (String.IsNullOrEmpty(ConfirmPassword)) return (false);
            if (!Credential.Password.Equals(ConfirmPassword)) return (false);

            return (true);
        }

        protected override void IsValidChanged(bool isValid)
        {
            EnableDisableAccept(isValid);
        }

        #endregion

        #region private methods

        private void EnableDisableAccept(bool enabled)
        {
            BurgerMenuItem accept = App.Controller.MainPageInstance.SelectedItem.GetChildItemByKey("accept");
            if (accept != null)
            {
                accept.IsEnabled = enabled;
            }
        }

        private async Task<Common.SaveResult> Save()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("KeyDerivationFunction", AppConfig.Instance.KeyDerivationFunction);
            switch (AppConfig.Instance.KeyDerivationFunction)
            {
                case AppConfig.CACHY_KEYDERIVATIONFUNCTION_PBKDF2:
                    {
                        parameters.Add("IterationCount", AppConfig.Instance.PBKDF2IterationCount.ToString());
                        break;
                    }
                case AppConfig.CACHY_KEYDERIVATIONFUNCTION_SCRYPT:
                    {
                        parameters.Add("IterationCount", AppConfig.Instance.SCryptIterationCount.ToString());
                        parameters.Add("BlockSize", AppConfig.Instance.SCryptBlockSize.ToString());
                        parameters.Add("ThreadCount", AppConfig.Instance.SCryptThreadCount.ToString());
                        break;
                    }
            }

            Common.SaveResult saveResult = await Credential.Vault.Save(parameters.ToArray());
            return (saveResult);
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            switch(key)
            {
                case "Credential":
                    {
                        _credential = (Credential)parameter;
                        if(!String.IsNullOrEmpty(_credential.Password))
                        {
                            ConfirmPassword = _credential.Password;
                        }
                        _credential.PropertyChanged += _credential_PropertyChanged;
                        NotifyPropertyChanged("Credential");
                        break;
                    }
                case "Mode":
                    {
                        _mode = (EditorMode)parameter;
                        NotifyPropertyChanged("PageTitle");
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
            if(item is ColourSelectView)
            {
                if(parameter != null)
                {
                    Credential.GlyphColour = (string)parameter;
                }
            }
            else if(item is GlyphSelectView)
            {
                if (parameter != null)
                {
                    Credential.GlyphKey = (string)parameter;
                }
            }
            EnableDisableAccept(IsValid);
        }

        public void OnNavigateTo(View view, object parameter)
        {
            EnableDisableAccept(IsValid);
        }

        public void OnNavigateFrom(View view, object parameter)
        {

        }

        public void OnGoBack(View view, object parameter)
        {

        }

        #endregion

        #region public methods

        public void Cancel(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Back command invoked.");

            App.Controller.GoBack();
        }

        public async void Accept(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Accept command invoked.");

            PasswordEntryView passwordEntry = View.FindByName<PasswordEntryView>("PasswordEntry");
            if(passwordEntry != null)
            {
                switch(passwordEntry.StrengthCheck)
                {
                    case PasswordEntryView.StrengthCheckResult.InWeakDictionary:
                        {
                            bool? agree = await App.Controller.MainPageInstance.DisplayAlert("Security Alert",
                                "The current password has been found in the internal known weak password database. You are advised to enter another password or use the 'Password Generator'.",
                                "OK",
                                "No, I understand the risk");
                            if (agree.HasValue && agree.Value) return;
                            break;
                        }
                    case PasswordEntryView.StrengthCheckResult.FailComplexityCheck:
                        {
                            bool? agree = await App.Controller.MainPageInstance.DisplayAlert("Security Alert",
                                "The current password has failed the complexity test. You are advised to enter another password or use the 'Password Generator'.",
                                "OK",
                                "No, I understand the risk");
                            if (agree.HasValue && agree.Value) return;
                            break;
                        }
                    case PasswordEntryView.StrengthCheckResult.OK:
                        {
                            bool duplicate = false;
                            switch (_mode)
                            {
                                case EditorMode.Create:
                                    {
                                        duplicate = Credential.Vault.Credentials.Any(cred => cred.Password == Credential.Password);
                                        break;
                                    }
                                case EditorMode.Edit:
                                    {
                                        duplicate = Credential.Vault.Credentials.Where(cred => cred.ID != Credential.ID).Any(cred => cred.Password == Credential.Password);
                                        break;
                                    }
                            }
                            if(duplicate)
                            {
                                bool? agree = await App.Controller.MainPageInstance.DisplayAlert("Security Alert",
                                    "The current password is already in use by another credential in your vault. You are advised to enter another password or use the 'Password Generator'.",
                                    "OK",
                                    "No, I understand the risk");
                                if (agree.HasValue && agree.Value) return;
                            }
                            //Check vault for duplicate usage
                            break;
                        }
                }
            }

            //Add the credential to the vault that it was created for
            switch (_mode)
            {
                case EditorMode.Create:
                    {
                        //This is a temporary fix for now as when typing into the tag
                        //it can create a whole bunch of additional tags erroneously
                        TagEditor tagEditor = View.FindByName<TagEditor>("CredentialTags");
                        if (tagEditor != null)
                        {
                            tagEditor.Recreate();
                        }

                        Credential.AddToVault(true);

                        if (AppConfig.Instance.AutoSave && AppConfig.Instance.AutoSaveOnDuplicatingCred)
                        {
                            Common.SaveResult saveResult = await Save();
                            if (saveResult == Common.SaveResult.Success)
                            {
                                VaultIndexFile.Invalidate();
                            }
                        }

                        App.Controller.NavigateTo("vault",
                            new KeyValuePair<String, Object>("Vault", Credential.Vault));
                        break;
                    }
                case EditorMode.Edit:
                    {
                        //This is a temporary fix for now as when typing into the tag
                        //it can create a whole bunch of additional tags erroneously
                        TagEditor tagEditor = View.FindByName<TagEditor>("CredentialTags");
                        if(tagEditor != null)
                        {
                            tagEditor.Recreate();
                        }

                        Credential originalCredential = Credential.Vault.Credentials.Where(cred => cred.ID == Credential.ID).First();
                        Credential.CopyTo(originalCredential);

                        if (AppConfig.Instance.AutoSave && AppConfig.Instance.AutoSaveOnDuplicatingCred)
                        {
                            Common.SaveResult saveResult = await Save();
                            if (saveResult == Common.SaveResult.Success)
                            {
                                VaultIndexFile.Invalidate();
                            }
                        }

                        App.Controller.NavigateTo("vault",
                            new KeyValuePair<String, Object>("Vault", Credential.Vault));
                        break;
                    }
            }
        }

        public void ChangeGlyphIcon(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Change Glyph icon invoked.");

            App.Controller.ShowPopup("crededit.selectglyph",
                new KeyValuePair<String, Object>("Credential", Credential),
                new KeyValuePair<String, Object>("Mode", _mode));
        }

        public void ChangeGlyphColour(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Change Glyph colour invoked.");

            App.Controller.ShowPopup("crededit.selectcolour",
                new KeyValuePair<String, Object>("Credential", Credential),
                new KeyValuePair<String, Object>("Mode", _mode));
        }

        #endregion

        #region commands

        public void GenerateCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Generate password command invoked.");

            App.Controller.ShowPopup("crededit.genpass",
                new KeyValuePair<String, Object>("Credential", Credential),
                new KeyValuePair<String, Object>("Mode", _mode));
        }

        public void AddNewTagCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "AddNewTagCommand invoked.");

            Credential.Tags.Add("newtag");
        }

        public void ClearTagsCommandAction(object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "ClearTagsCommand invoked.");

            Credential.Tags.Clear();
        }

        #endregion

        #region object events

        private void _credential_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Validate();
        }

        #endregion

    }

}

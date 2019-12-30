using cachy.Config;
using cachy.Navigation.BurgerMenu;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Cryptography.Random;
using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class GeneratePasswordViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private GeneratePasswordView _view;
        private Credential _credential;
        private CreateCredentialViewModel.EditorMode _credentialEditorMode;
        private string _password = String.Empty;
        private int _length = 8;
        private bool _memorable = true;
        private bool _randomFormat = true;
        private string _memorableFormat = "";
        private bool _useLowercase = true;
        private bool _useUppercase = true;
        private bool _useDigits = true;
        private bool _useSpecial;
        private bool _useMinus;
        private bool _useUnderline;
        private bool _useSpace;
        private bool _useBrackets;
        private bool _useOther;
        private bool _atLeastOneOfEach = true;
        private string _allowedChars = String.Empty;

        private ICommand _backCommand;
        private ICommand _acceptCommand;
        private ICommand _generateCommand;
        private ICommand _resetFormatCommand;

        #endregion

        #region public properties

        public string Password
        {
            get
            {
                return (_password);
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyPropertyChanged("Password");
                }
            }
        }

        public int Length
        {
            get
            {
                return (_length);
            }
            set
            {
                if (_length != value)
                {
                    _length = value;
                    NotifyPropertyChanged("Length");
                    Generate();
                }
            }
        }

        public bool Memorable
        {
            get
            {
                return (_memorable);
            }
            set
            {
                if(_memorable != value)
                {
                    _memorable = value;
                    NotifyPropertyChanged("Memorable");
                    NotifyPropertyChanged("UseSpecial");
                    Generate();
                }
            }
        }

        public bool RandomFormat
        {
            get
            {
                return (_randomFormat);
            }
            set
            {
                if(_randomFormat != value)
                {
                    _randomFormat = value;
                    NotifyPropertyChanged("RandomFormat");
                    NotifyPropertyChanged("MemorableAndNotRandomFormat");
                    Generate();
                }
            }
        }

        public bool MemorableAndNotRandomFormat
        {
            get
            {
                return (Memorable && !RandomFormat);
            }
        }

        public string MemorableFormat
        {
            get
            {
                return (_memorableFormat);
            }
            set
            {
                if(_memorableFormat != value)
                {
                    _memorableFormat = value;
                    NotifyPropertyChanged("MemorableFormat");
                    Generate();
                }
            }
        }

        public Boolean UseLowercase
        {
            get
            {
                return (_useLowercase);
            }
            set
            {
                if (_useLowercase != value)
                {
                    _useLowercase = value;
                    NotifyPropertyChanged("UseLower");
                    UpdateAllowedChars();
                    Generate();
                }
            }
        }

        public Boolean UseUppercase
        {
            get
            {
                return (_useUppercase);
            }
            set
            {
                if (_useUppercase != value)
                {
                    _useUppercase = value;
                    NotifyPropertyChanged("UseUppercase");
                    UpdateAllowedChars();
                    Generate();
                }
            }
        }

        public Boolean UseDigits
        {
            get
            {
                return (_useDigits);
            }
            set
            {
                if (_useDigits != value)
                {
                    _useDigits = value;
                    NotifyPropertyChanged("UseDigits");
                    UpdateAllowedChars();
                    Generate();
                }
            }
        }

        public Boolean UseSpecial
        {
            get
            {
                return (_useSpecial && !Memorable);
            }
            set
            {
                if (_useSpecial != value)
                {
                    _useSpecial = value;
                    NotifyPropertyChanged("UseSpecial");
                    UpdateAllowedChars();
                    Generate();
                }
            }
        }

        public Boolean UseMinus
        {
            get
            {
                return (_useMinus);
            }
            set
            {
                if (_useMinus != value)
                {
                    _useMinus = value;
                    NotifyPropertyChanged("UseMinus");
                    UpdateAllowedChars();
                    Generate();
                }
            }
        }

        public Boolean UseUnderline
        {
            get
            {
                return (_useUnderline);
            }
            set
            {
                if (_useUnderline != value)
                {
                    _useUnderline = value;
                    NotifyPropertyChanged("UseUnderline");
                    UpdateAllowedChars();
                    Generate();
                }
            }
        }

        public Boolean UseSpace
        {
            get
            {
                return (_useSpace);
            }
            set
            {
                if (_useSpace != value)
                {
                    _useSpace = value;
                    NotifyPropertyChanged("UseSpace");
                    UpdateAllowedChars();
                    Generate();
                }
            }
        }

        public Boolean UseBrackets
        {
            get
            {
                return (_useBrackets);
            }
            set
            {
                if (_useBrackets != value)
                {
                    _useBrackets = value;
                    NotifyPropertyChanged("UseBrackets");
                    UpdateAllowedChars();
                    Generate();
                }
            }
        }

        public Boolean UseOther
        {
            get
            {
                return (_useOther);
            }
            set
            {
                if (_useOther != value)
                {
                    _useOther = value;
                    NotifyPropertyChanged("UseOther");
                    UpdateAllowedChars();
                    Generate();
                }
            }
        }

        public Boolean AtLeastOneOfEach
        {
            get
            {
                return (_atLeastOneOfEach);
            }
            set
            {
                if(_atLeastOneOfEach != value)
                {
                    _atLeastOneOfEach = value;
                    NotifyPropertyChanged("AtLeastOneOfEach");
                    Generate();
                }
            }
        }

        public String AllowedChars
        {
            get
            {
                return (_allowedChars);
            }
        }

        public Boolean CanGenerate
        {
            get
            {
                return (AllowedChars.Length > 0);
            }
        }

        public GeneratePasswordView View
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

        public ICommand GenerateCommand
        {
            get
            {
                return (_generateCommand);
            }
        }

        public ICommand ResetFormatCommand
        {
            get
            {
                return (_resetFormatCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public GeneratePasswordViewModel(GeneratePasswordView view)
        {
            _view = view;

            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acceptCommand = new Command(new Action<Object>(AcceptCommandAction));
            _generateCommand = new Command(new Action<Object>(GenerateCommandAction));
            _resetFormatCommand= new Command(new Action<Object>(ResetFormatCommandAction));

        }

        #endregion

        #region private methods

        private void UpdateAllowedChars()
        {
            String lowercase = "abcdefghijklmnopqrstuvwxyz";
            String digits = "0123456789";

            StringBuilder allowedChars = new StringBuilder();
            if (UseLowercase) allowedChars.Append(lowercase);
            if (UseUppercase) allowedChars.Append(lowercase.ToUpper());
            if (UseDigits) allowedChars.Append(digits);
            if (UseSpecial)
            {
                if (UseMinus) allowedChars.Append("-");
                if (UseUnderline) allowedChars.Append("_");
                if (UseSpace) allowedChars.Append(" ");
                if (UseBrackets) allowedChars.Append("[]{}()<>");
                if (UseOther) allowedChars.Append(@"!£$%&+@#,.\/");
            }
            _allowedChars = allowedChars.ToString();
            NotifyPropertyChanged("AllowedChars");
            NotifyPropertyChanged("CanGenerate");
        }

        private void Generate()
        {
            if(Memorable)
            {
                try
                {
                    string format = String.Empty;
                    if(RandomFormat)
                    {
                        List<string> formatParts = new List<string>();
                        formatParts.Add("{adjective:rc}");
                        formatParts.Add("{noun:rc}");
                        formatParts.Add("{verb:rc}");
                        formatParts.Add("{special1:1}");
                        formatParts.Add("{int:1-100}");
                        while(formatParts.Count > 0)
                        {
                            if (formatParts.Count > 0)
                            {
                                int randomIndex = SimpleRandomGenerator.QuickGetRandomInt(0, formatParts.Count);
                                format += formatParts[randomIndex];
                                formatParts.RemoveAt(randomIndex);
                            }
                            else
                            {
                                format += formatParts[0];
                                formatParts.RemoveAt(0);
                            }
                        }
                    }
                    else
                    {
                        format = MemorableFormat;
                    }
                    Password = Dictionaries.Instance.GeneratePhrase(format);
                }
                catch (Exception)
                {
                    Password = "Format Error!";
                }
            }
            else
            {
                SimpleRandomGenerator.CharSelection charSelection = SimpleRandomGenerator.CharSelection.None;
                if (UseDigits) charSelection |= SimpleRandomGenerator.CharSelection.Digits;
                if (UseLowercase) charSelection |= SimpleRandomGenerator.CharSelection.Lowercase;
                if (UseUppercase) charSelection |= SimpleRandomGenerator.CharSelection.Uppercase;
                if (UseSpecial && UseBrackets) charSelection |= SimpleRandomGenerator.CharSelection.Brackets;
                if (UseSpecial && UseMinus) charSelection |= SimpleRandomGenerator.CharSelection.Minus;
                if (UseSpecial && UseOther) charSelection |= SimpleRandomGenerator.CharSelection.Other;
                if (UseSpecial && UseSpace) charSelection |= SimpleRandomGenerator.CharSelection.Space;
                if (UseSpecial && UseUnderline) charSelection |= SimpleRandomGenerator.CharSelection.Underline;

                Password = SimpleRandomGenerator.QuickGetRandomString(charSelection, Length, AtLeastOneOfEach);
            }
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
                        break;
                    }
                case "Mode":
                    {
                        _credentialEditorMode = (CreateCredentialViewModel.EditorMode)parameter;
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
            _memorableFormat = AppConfig.Instance.MemorablePasswordFormat;
            NotifyPropertyChanged("MemorableFormat");
            UpdateAllowedChars();
            Generate();
        }

        public void OnNavigateFrom(View view, object parameter)
        {
        }

        public void OnGoBack(View view, object parameter)
        {

        }

        #endregion

        #region commands

        public void BackCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Back command invoked.");

            App.Controller.ClosePopup(parameter);
        }

        public void AcceptCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Accept command invoked.");

            if (Memorable)
            {
                AppConfig.Instance.MemorablePasswordFormat = MemorableFormat;
                AppConfig.Instance.Save();
            }

            Credential.Password = Password;
            App.Controller.NavigateTo("crededit",
                new KeyValuePair<String, Object>("Credential", Credential),
                new KeyValuePair<String, Object>("Mode", _credentialEditorMode));
        }

        public void GenerateCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Generate command invoked.");

            Generate();
        }

        public void ResetFormatCommandAction(Object parameter)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Reset format command invoked.");

            MemorableFormat = AppConstants.DEFAULT_MEMORABLEPASSWORDFORMAT;
        }

        #endregion

    }

}

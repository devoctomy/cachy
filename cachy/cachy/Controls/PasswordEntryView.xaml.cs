using cachy.Config;
using cachy.Fonts;
using devoctomy.cachy.Framework.Cryptography.Random;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PasswordEntryView : ContentView
	{

        #region public enums

        public enum StrengthCheckResult
        {
            None = 0,
            InWeakDictionary = 1,
            FailComplexityCheck = 2,
            OK = 3
        }

        #endregion

        #region bindable properties

        public static BindableProperty TextProperty = BindableProperty.Create("Text",
            typeof(String),
            typeof(PasswordEntryView),
            "",
            BindingMode.Default,
            null,
            TextPropertyChanged);

        public static BindableProperty PlaceholderProperty = BindableProperty.Create("Placeholder",
            typeof(String),
            typeof(PasswordEntryView),
            "",
            BindingMode.Default);

        public static BindableProperty AllowShowPasswordProperty = BindableProperty.Create("AllowShowPassword",
            typeof(Boolean),
            typeof(PasswordEntryView),
            false,
            BindingMode.Default,
            null,
            AllowShowPasswordChanged);

        public static BindableProperty IsPasswordProperty = BindableProperty.Create("IsPassword",
            typeof(Boolean),
            typeof(PasswordEntryView),
            true,
            BindingMode.Default);

        public static BindableProperty ReturnCommandProperty = BindableProperty.Create("ReturnCommand",
            typeof(Command),
            typeof(PasswordEntryView),
            null,
            BindingMode.Default);


        public static BindableProperty ShowStrengthIndicatorProperty = BindableProperty.Create("ShowStrengthIndicator",
            typeof(bool),
            typeof(PasswordEntryView),
            true,
            BindingMode.Default,
            null,
            ShowStrengthIndicatorPropertyChanged);

        #endregion

        #region private objects

        private ICommand _showCommand;
        private Color _strengthIndicatorColor = Color.Gray;
        private double _threshold;
        private StrengthCheckResult _strengthCheck = StrengthCheckResult.None;

        #endregion

        #region public properties

        public string Text
        {
            get
            {
                return ((string)GetValue(TextProperty));
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public bool TextIsSet
        {
            get
            {
                return (!String.IsNullOrEmpty(Text));
            }
        }

        public string Placeholder
        {
            get
            {
                return ((string)GetValue(PlaceholderProperty));
            }
            set
            {
                SetValue(PlaceholderProperty, value);
            }
        }

        public bool AllowShowPassword
        {
            get
            {
                return ((bool)GetValue(AllowShowPasswordProperty));
            }
            set
            {
                SetValue(AllowShowPasswordProperty, value);
            }
        }

        public bool IsPassword
        {
            get
            {
                return ((bool)GetValue(IsPasswordProperty));
            }
            set
            {
                SetValue(IsPasswordProperty, value);
            }
        }

        public GridLength ShowPasswordColumnWidth
        {
            get
            {
                return (new GridLength(AllowShowPassword ? 44 : 0, GridUnitType.Absolute));
            }
        }

        public ICommand ShowCommand
        {
            get
            {
                return (_showCommand);
            }
        }

        public ICommand ReturnCommand
        {
            get
            {
                return ((ICommand)GetValue(ReturnCommandProperty));
            }
            set
            {
                SetValue(ReturnCommandProperty, value);
            }
        }

        public Color StrengthIndicatorBackgroundColour
        {
            get
            {
                return (_strengthIndicatorColor);
            }
            set
            {
                if(_strengthIndicatorColor != value)
                {
                    _strengthIndicatorColor = value;
                    OnPropertyChanged("StrengthIndicatorBackgroundColour");
                }
            }
        }

        public bool ShowStrengthIndicator
        {
            get
            {
                return ((bool)GetValue(ShowStrengthIndicatorProperty));
            }
            set
            {
                SetValue(ShowStrengthIndicatorProperty, value);
            }
        }

        public GridLength StrengthIndicatorColWidth
        {
            get
            {
                return (new GridLength(ShowStrengthIndicator ? (String.IsNullOrEmpty(Text) ? 0 : 28) : 0, GridUnitType.Absolute));
            }
        }

        public StrengthCheckResult StrengthCheck
        {
            get
            {
                return (_strengthCheck);
            }
        }

        public string StrengthIndicatorText
        {
            get
            {
                switch(StrengthCheck)
                {
                    case StrengthCheckResult.OK:
                        {
                            return (CachyFont.GetString(CachyFont.Glyph.Check));
                        }
                    case StrengthCheckResult.FailComplexityCheck:
                        {
                            return (CachyFont.GetString(CachyFont.Glyph.Warning_Message));
                        }
                    case StrengthCheckResult.InWeakDictionary:
                        {
                            return (CachyFont.GetString(CachyFont.Glyph.Circle_Remove_01));
                        }
                    default:
                        {
                            return (String.Empty);
                        }
                }
            }
        }

        #endregion

        #region constructor / destructor

        public PasswordEntryView ()
		{
            _showCommand = new Command(new Action<object>(ShowCommandAction));
            int recommendedCharSelection = SimpleRandomGenerator.GetTotalCharCountForSelection(SimpleRandomGenerator.CharSelection.Lowercase |
                SimpleRandomGenerator.CharSelection.Uppercase |
                SimpleRandomGenerator.CharSelection.Digits |
                SimpleRandomGenerator.CharSelection.Minus |
                SimpleRandomGenerator.CharSelection.Underline);
            _threshold = Math.Pow(12, recommendedCharSelection);
            InitializeComponent();
        }

        #endregion

        #region private methods

        private async Task UpdatePasswordStrengthIndicator(string newValue)
        {
            await Task.Yield();

            string password = (string)newValue;
            if (!String.IsNullOrEmpty(password))
            {
                bool isWeak = Dictionaries.Instance.IsKnownWeak(password);
                if (isWeak)
                {
                    _strengthCheck = StrengthCheckResult.InWeakDictionary;
                    StrengthIndicatorBackgroundColour = Color.Red;
                }
                else
                {
                    double strength = SimpleRandomGenerator.GetStrength(password);
                    _strengthCheck = strength >= _threshold ? StrengthCheckResult.OK : StrengthCheckResult.FailComplexityCheck;
                    StrengthIndicatorBackgroundColour = _strengthCheck == StrengthCheckResult.OK ? Color.Green : Color.Orange;
                }
            }
            else
            {
                _strengthCheck = StrengthCheckResult.None;
                StrengthIndicatorBackgroundColour = Color.Gray;
            }
            OnPropertyChanged("StrengthIndicatorColWidth");
            OnPropertyChanged("StrengthIndicatorText");
            OnPropertyChanged("TextIsSet");
        }

        #endregion

        #region bindable property change callbacks

        private static void AllowShowPasswordChanged(BindableObject bindable, object oldValue, object newValue)
        {
            PasswordEntryView passwordEntry = bindable as PasswordEntryView;
            if(passwordEntry != null)
            {
                passwordEntry.OnPropertyChanged("ShowPasswordColumnWidth");
            }
        }

        private async static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            PasswordEntryView passwordEntry = bindable as PasswordEntryView;
            if (passwordEntry.ShowStrengthIndicator)
            {
                await passwordEntry.UpdatePasswordStrengthIndicator((string)newValue);
            }
        }

        private static void ShowStrengthIndicatorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            PasswordEntryView passwordEntry = bindable as PasswordEntryView;
            passwordEntry.OnPropertyChanged("StrengthIndicatorColWidth");
        }

        #endregion

        #region private methods

        private void ShowCommandAction(object parameter)
        {
            IsPassword = !IsPassword;
        }

        #endregion

    }

}
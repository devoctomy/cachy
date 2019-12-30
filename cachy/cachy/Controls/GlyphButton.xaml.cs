using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using cachy.Fonts;
using System.Windows.Input;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class GlyphButton : ContentView
	{

        #region public events

        public event EventHandler<EventArgs> Clicked;

        #endregion

        #region public enums

        public enum ButtonMode
        {
            None = 0,
            GlyphOnly = 1,
            GlyphAndTextLeftRight = 2,
            TextOnly = 3
        }

        #endregion

        #region bindable properties

        public static BindableProperty BackgroundColourToggledOnProperty = BindableProperty.Create("BackgroundColourToggledOn",
            typeof(Color),
            typeof(GlyphButton),
            Color.Accent,
            BindingMode.Default);

        public static BindableProperty ModeProperty = BindableProperty.Create("Mode",
            typeof(ButtonMode),
            typeof(GlyphButton),
            ButtonMode.GlyphAndTextLeftRight,
            BindingMode.Default,
            null,
            OnModeChanged);

        public static BindableProperty GlyphProperty = BindableProperty.Create("Glyph",
            typeof(CachyFont.Glyph),
            typeof(GlyphButton),
            CachyFont.Glyph.None,
            BindingMode.Default,
            null,
            OnGlyphChanged);

        public static BindableProperty TextProperty = BindableProperty.Create("Text",
            typeof(String),
            typeof(GlyphButton),
            String.Empty,
            BindingMode.Default);

        public static BindableProperty GlyphFontSizeProperty = BindableProperty.Create("GlyphFontSize",
            typeof(Double),
            typeof(GlyphButton),
            36d,
            BindingMode.Default);

        public static BindableProperty LabelFontSizeProperty = BindableProperty.Create("LabelFontSize",
            typeof(Double),
            typeof(GlyphButton),
            20d,
            BindingMode.Default);

        public static BindableProperty LabelFontAttributesProperty = BindableProperty.Create("LabelFontAttributes",
            typeof(FontAttributes),
            typeof(GlyphButton),
            FontAttributes.None,
            BindingMode.Default);

        public static BindableProperty EnabledLabelColourProperty = BindableProperty.Create("EnabledLabelColour",
            typeof(Color),
            typeof(GlyphButton),
            Color.Black,
            BindingMode.Default,
            null,
            EnabledLabelColourChanged);

        public static BindableProperty DisabledLabelColourProperty = BindableProperty.Create("DisabledLabelColour",
            typeof(Color),
            typeof(GlyphButton),
            Color.DarkGray,
            BindingMode.Default,
            null,
            DisabledLabelColourChanged);

        public static BindableProperty EnabledGlyphColourProperty = BindableProperty.Create("EnabledGlyphColour",
            typeof(Color),
            typeof(GlyphButton),
            Color.Accent,
            BindingMode.Default,
            null,
            EnabledGlyphColourChanged);

        public static BindableProperty DisabledGlyphColourProperty = BindableProperty.Create("DisabledGlyphColour",
            typeof(Color),
            typeof(GlyphButton),
            Color.DarkGray,
            BindingMode.Default,
            null,
            DisabledGlyphColourChanged);

        public static BindableProperty GlyphColourToggledOnProperty = BindableProperty.Create("GlyphColourToggledOn",
            typeof(Color),
            typeof(GlyphButton),
            Color.Accent,
            BindingMode.Default);

        public static BindableProperty CommandProperty = BindableProperty.Create("Command",
            typeof(ICommand),
            typeof(GlyphButton),
            null,
            BindingMode.Default);

        public static BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter",
            typeof(Object),
            typeof(GlyphButton),
            null,
            BindingMode.Default);

        public static BindableProperty IsToggleButtonProperty = BindableProperty.Create("IsToggleButton",
            typeof(Boolean),
            typeof(GlyphButton),
            false,
            BindingMode.Default);

        public static BindableProperty IsPressedProperty = BindableProperty.Create("IsPressed",
            typeof(Boolean),
            typeof(GlyphButton),
            false,
            BindingMode.Default,
            propertyChanged: OnIsPressedChanged);

        public static BindableProperty ToggleGroupProperty = BindableProperty.Create("ToggleGroup",
            typeof(String),
            typeof(GlyphButton),
            String.Empty,
            BindingMode.Default,
            propertyChanged: OnToggleGroupChanged);

        public static BindableProperty IconMarginProperty = BindableProperty.Create("IconMargin",
            typeof(Thickness),
            typeof(GlyphButton),
            new Thickness(0),
            BindingMode.Default);

        public static BindableProperty CornerRadiusProperty = BindableProperty.Create("CornerRadius",
            typeof(int),
            typeof(GlyphButton),
            0,
            BindingMode.Default);

        #endregion

        #region private objects

        private static Dictionary<String, List<GlyphButton>> _toggleGroups = new Dictionary<String, List<GlyphButton>>();
        private Boolean _isUpdating;

        private ICommand _commandProxy;

        #endregion

        #region public properties

        public Color BackgroundColourToggledOn
        {
            get
            {
                return ((Color)GetValue(BackgroundColourToggledOnProperty));
            }
            set
            {
                SetValue(BackgroundColourToggledOnProperty, value);
            }
        }

        public ButtonMode Mode
        {
            get
            {
                return ((ButtonMode)GetValue(ModeProperty));
            }
            set
            {
                SetValue(ModeProperty, value);
            }
        }

        public String GlyphText
        {
            get
            {
                return (CachyFont.GetString(Glyph));
            }
        }

        public CachyFont.Glyph Glyph
        {
            get
            {
                return ((CachyFont.Glyph)GetValue(GlyphProperty));
            }
            set
            {
                SetValue(GlyphProperty, value);
                OnPropertyChanged("GlyphText");
            }
        }

        public String Text
        {
            get
            {
                return ((String)GetValue(TextProperty));
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public Double GlyphFontSize
        {
            get
            {
                return ((Double)GetValue(GlyphFontSizeProperty));
            }
            set
            {
                SetValue(GlyphFontSizeProperty, value);
            }
        }

        public Double LabelFontSize
        {
            get
            {
                return ((Double)GetValue(LabelFontSizeProperty));
            }
            set
            {
                SetValue(LabelFontSizeProperty, value);
            }
        }

        public FontAttributes LabelFontAttributes
        {
            get
            {
                return ((FontAttributes)GetValue(LabelFontAttributesProperty));
            }
            set
            {
                SetValue(LabelFontAttributesProperty, value);
            }
        }

        public Color GlyphColour
        {
            get
            {
                return(IsEnabled ? EnabledGlyphColour : DisabledGlyphColour);
            }
        }

        public Color EnabledGlyphColour
        {
            get
            {
                return ((Color)GetValue(EnabledGlyphColourProperty));
            }
            set
            {
                SetValue(EnabledGlyphColourProperty, value);
            }
        }

        public Color DisabledGlyphColour
        {
            get
            {
                return ((Color)GetValue(DisabledGlyphColourProperty));
            }
            set
            {
                SetValue(DisabledGlyphColourProperty, value);
            }
        }

        public Color LabelColour
        {
            get
            {
                return (IsEnabled ? EnabledLabelColour : DisabledLabelColour);
            }
        }

        public Color EnabledLabelColour
        {
            get
            {
                return ((Color)GetValue(EnabledLabelColourProperty));
            }
            set
            {
                SetValue(EnabledLabelColourProperty, value);
            }
        }

        public Color DisabledLabelColour
        {
            get
            {
                return ((Color)GetValue(DisabledLabelColourProperty));
            }
            set
            {
                SetValue(DisabledLabelColourProperty, value);
            }
        }

        public Color GlyphColourToggledOn
        {
            get
            {
                return ((Color)GetValue(GlyphColourToggledOnProperty));
            }
            set
            {
                SetValue(GlyphColourToggledOnProperty, value);
            }
        }

        public bool TextLabelIsVisible
        {
            get
            {
                return (Mode == ButtonMode.GlyphAndTextLeftRight || Mode == ButtonMode.TextOnly);
            }
        }

        public bool IconLabelIsVisible
        {
            get
            {
                return (Mode == ButtonMode.GlyphAndTextLeftRight ||
                    Mode == ButtonMode.GlyphOnly);
            }
        }

        public int CornerRadius
        {
            get
            {
                return ((int)GetValue(CornerRadiusProperty));
            }
            set
            {
                SetValue(CornerRadiusProperty, value);
            }
        }

        public ICommand Command
        {
            get
            {
                return ((ICommand)GetValue(CommandProperty));
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public ICommand CommandProxy
        {
            get
            {
                return (_commandProxy);
            }
            set
            {
                if(_commandProxy != value)
                {
                    _commandProxy = value;
                    OnPropertyChanged("CommandProxy");
                }
            }
        }

        public Object CommandParameter
        {
            get
            {
                return ((Object)GetValue(CommandParameterProperty));
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public Boolean IsToggleButton
        {
            get
            {
                return ((Boolean)GetValue(IsToggleButtonProperty));
            }
            set
            {
                SetValue(IsToggleButtonProperty, value);
            }
        }

        public String ToggleGroup
        {
            get
            {
                return ((String)GetValue(ToggleGroupProperty));
            }
            set
            {
                SetValue(ToggleGroupProperty, value);
            }
        }

        public Boolean IsPressed
        {
            get
            {
                return ((Boolean)GetValue(IsPressedProperty));
            }
            set
            {
                SetValue(IsPressedProperty, value);
            }
        }

        public Thickness IconMargin
        {
            get
            {
                return ((Thickness)GetValue(IconMarginProperty));
            }
            set
            {
                SetValue(IconMarginProperty, value);
            }
        }

        #endregion

        #region constructor / destructor

        public GlyphButton ()
		{
			InitializeComponent ();
            CommandProxy = new Command(new Action<Object>(CommandProxyAction));
        }

        #endregion

        #region private methods

        private static void UnpressAllInGroupExcept(String group, GlyphButton except)
        {
            List<GlyphButton> groupButtons = null;
            if (_toggleGroups.ContainsKey(group))
            {
                groupButtons = _toggleGroups[group];

                foreach (GlyphButton curButton in groupButtons)
                {
                    if (curButton != except)
                    {
                        curButton._isUpdating = true;
                        curButton.IsPressed = false;
                        curButton._isUpdating = false;
                    }
                }
            }
        }

        private static void RemoveFromGroup(GlyphButton button, String group)
        {
            List<GlyphButton> groupButtons = null;
            if (_toggleGroups.ContainsKey(group))
            {
                groupButtons = _toggleGroups[group];
            }
            else
            {
                groupButtons = new List<GlyphButton>();
                _toggleGroups.Add(group, groupButtons);
            }
            if (groupButtons.Contains(button)) groupButtons.Remove(button);
        }

        private static void AddToGroup(GlyphButton button, String group)
        {
            List<GlyphButton> groupButtons = null;
            if (_toggleGroups.ContainsKey(group))
            {
                groupButtons = _toggleGroups[group];
            }
            else
            {
                groupButtons = new List<GlyphButton>();
                _toggleGroups.Add(group, groupButtons);
            }
            groupButtons.Add(button);
        }

        #endregion

        #region bindable property change callbacks

        private static void EnabledGlyphColourChanged(BindableObject bindable, object oldValue, object newValue)
        {
            GlyphButton glyphButton = (GlyphButton)bindable;
            glyphButton.OnPropertyChanged("GlyphColour");
        }

        private static void DisabledGlyphColourChanged(BindableObject bindable, object oldValue, object newValue)
        {
            GlyphButton glyphButton = (GlyphButton)bindable;
            glyphButton.OnPropertyChanged("GlyphColour");
        }

        private static void EnabledLabelColourChanged(BindableObject bindable, object oldValue, object newValue)
        {
            GlyphButton glyphButton = (GlyphButton)bindable;
            glyphButton.OnPropertyChanged("LabelColour");
        }

        private static void DisabledLabelColourChanged(BindableObject bindable, object oldValue, object newValue)
        {
            GlyphButton glyphButton = (GlyphButton)bindable;
            glyphButton.OnPropertyChanged("LabelColour");
        }

        private static void OnModeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            GlyphButton glyphButton = (GlyphButton)bindable;
            ButtonMode newMode = (ButtonMode)newValue;
            switch(newMode)
            {
                case ButtonMode.GlyphOnly:
                    {
                        glyphButton.GlyphIconLabel.HorizontalTextAlignment = TextAlignment.Center;
                        glyphButton.GlyphIconLabel.VerticalTextAlignment = TextAlignment.Center;

                        break;
                    }
                case ButtonMode.GlyphAndTextLeftRight:
                    {
                        glyphButton.GlyphIconLabel.HorizontalTextAlignment = TextAlignment.Center;
                        glyphButton.GlyphIconLabel.VerticalTextAlignment = TextAlignment.Center;
                        glyphButton.GlyphTextLabel.HorizontalTextAlignment = TextAlignment.End;
                        glyphButton.GlyphTextLabel.VerticalTextAlignment = TextAlignment.Center;

                        break;
                    }
                case ButtonMode.TextOnly:
                    {
                        glyphButton.GlyphTextLabel.HorizontalTextAlignment = TextAlignment.Center;
                        glyphButton.GlyphTextLabel.VerticalTextAlignment = TextAlignment.Center;
                        break;
                    }
            }
            glyphButton.OnPropertyChanged("TextLabelIsVisible");
            glyphButton.OnPropertyChanged("IconLabelIsVisible");
            glyphButton.UpdateChildrenLayout();
        }

        private static void OnGlyphChanged(BindableObject bindable, object oldValue, object newValue)
        {
            GlyphButton glyphButton = (GlyphButton)bindable;
            glyphButton.OnPropertyChanged("GlyphText");
        }

        static void OnIsPressedChanged(BindableObject bindable, object oldVal, object newVal)
        {
            GlyphButton glyphButton = (GlyphButton)bindable;
            Boolean newIsPressed = (Boolean)newVal;
            if(newIsPressed)
            {
                glyphButton.LayoutRoot.BackgroundColor = glyphButton.BackgroundColourToggledOn;
                glyphButton.GlyphIconLabel.TextColor = glyphButton.GlyphColourToggledOn;
            }
            else
            {
                glyphButton.LayoutRoot.BackgroundColor = glyphButton.BackgroundColor;
                glyphButton.GlyphIconLabel.TextColor = glyphButton.GlyphColour;
            }
            if (!glyphButton._isUpdating && !String.IsNullOrEmpty(glyphButton.ToggleGroup)) UnpressAllInGroupExcept(glyphButton.ToggleGroup, glyphButton);
        }

        private static void OnToggleGroupChanged(BindableObject bindable, object oldValue, object newValue)
        {
            GlyphButton glyphButton = (GlyphButton)bindable;
            String oldGroup = (String)oldValue;
            String newGroup = (String)newValue;
            if (!String.IsNullOrEmpty(oldGroup)) RemoveFromGroup(glyphButton, oldGroup);
            if (!String.IsNullOrEmpty(newGroup)) AddToGroup(glyphButton, newGroup);
        }

        #endregion

        #region base class overrides

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case "IsEnabled":
                    {
                        OnPropertyChanged("GlyphColour");
                        OnPropertyChanged("LabelColour");
                        break;
                    }
            }
        }

        #endregion

        #region commands

        public void CommandProxyAction(Object parameter)
        {
            if (IsToggleButton)
            {
                IsPressed = !IsPressed;
            }
            if (Command != null && Command.CanExecute(parameter))
            {
                Command.Execute(parameter);
            }
            else
            {
                Clicked?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

    }

}
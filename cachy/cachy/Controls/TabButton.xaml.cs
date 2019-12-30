using devoctomy.DFramework.Logging;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TabButton : ContentView
	{

        #region public events

        public event EventHandler<EventArgs> Clicked;
        public event EventHandler<EventArgs> Pressed;
        public event EventHandler<EventArgs> Unpressed;

        #endregion

        #region bindable properties

        public BindableProperty TextProperty = BindableProperty.Create("Text",
            typeof(String),
            typeof(TabButton),
            String.Empty);

        public BindableProperty IsPressedProperty = BindableProperty.Create("IsPressed",
            typeof(Boolean),
            typeof(TabButton),
            false,
            BindingMode.Default,
            null,
            IsPressedChanged);

        public BindableProperty ButtonGroupProperty = BindableProperty.Create("ButtonGroup",
            typeof(String),
            typeof(TabButton),
            String.Empty,
            BindingMode.Default,
            null,
            ButtonGroupChanged);

        public BindableProperty ButtonBorderColourProperty = BindableProperty.Create("ButtonBorderColour",
            typeof(Color),
            typeof(TabButton),
            Application.Current.Resources["ItemBorderColour"],
            BindingMode.Default,
            null,
            ButtonBorderColourChanged);

        #endregion

        #region private objects

        private static Dictionary<String, List<TabButton>> _buttonGroups = new Dictionary<String, List<TabButton>>();
        private Boolean _isUpdating;

        #endregion

        #region public properties

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

        public String ButtonGroup
        {
            get
            {
                return ((String)GetValue(ButtonGroupProperty));
            }
            set
            {
                SetValue(ButtonGroupProperty, value);
            }
        }

        public Color ButtonBorderColour
        {
            get
            {
                return ((Color)GetValue(ButtonBorderColourProperty));
            }
            set
            {
                SetValue(ButtonBorderColourProperty, value);
            }
        }

        #endregion

        #region constructor / destructor

        public TabButton ()
		{
			InitializeComponent ();
		}

        #endregion

        #region bindable property change callbacks

        private static void IsPressedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TabButton tabButton = (TabButton)bindable;
            Boolean isPressed = (Boolean)newValue;
            tabButton.ButtonBorder.BorderThickness = isPressed ? "0,0,0,8" : "0";
            tabButton.ButtonLabel.FontAttributes = isPressed ? tabButton.ButtonLabel.FontAttributes | FontAttributes.Bold : tabButton.ButtonLabel.FontAttributes & ~FontAttributes.Bold;
            if (!tabButton._isUpdating && !String.IsNullOrEmpty(tabButton.ButtonGroup)) UnpressAllInGroupExcept(tabButton.ButtonGroup, tabButton);
            if(isPressed)
            {
                tabButton.Pressed?.Invoke(tabButton, EventArgs.Empty);
            }
            else
            {
                tabButton.Unpressed?.Invoke(tabButton, EventArgs.Empty);
            }
        }

        private static void ButtonGroupChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TabButton tabButton = (TabButton)bindable;
            String oldGroup = (String)oldValue;
            String newGroup = (String)newValue;
            if(!String.IsNullOrEmpty(oldGroup)) RemoveFromGroup(tabButton, oldGroup);
            if(!String.IsNullOrEmpty(newGroup)) AddToGroup(tabButton, newGroup);
        }

        private static void ButtonBorderColourChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TabButton tabButton = (TabButton)bindable;
            Color oldColour = (Color)oldValue;
            Color newColor = (Color)newValue;
            tabButton.ButtonBorderColour = newColor;
        }

        #endregion

        #region object events

        private void Button_Clicked(object sender, EventArgs e)
        {
            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Tab button '{0}' clicked.", Text);
            IsPressed = true;
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region public methods

        public void SimulatedClick()
        {
            Button_Clicked(null, EventArgs.Empty);
        }

        #endregion

        #region private methods

        private static void UnpressAllInGroupExcept(String group, TabButton except)
        {
            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Unpressing all tab buttons in group '{0}', except '{1}'.", group, except.Text);
            List<TabButton> groupButtons = null;
            if (_buttonGroups.ContainsKey(group))
            {
                groupButtons = _buttonGroups[group];

                foreach(TabButton curButton in groupButtons)
                {
                    if(curButton != except)
                    {
                        curButton._isUpdating = true;
                        curButton.IsPressed = false;
                        curButton._isUpdating = false;
                    }
                }
            }
        }

        private static void RemoveFromGroup(TabButton button, String group)
        {
            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Removing tab button '{0}' from group '{1}'.", button.Text, group);
            List<TabButton> groupButtons = null;
            if (_buttonGroups.ContainsKey(group))
            {
                groupButtons = _buttonGroups[group];
            }
            else
            {
                groupButtons = new List<TabButton>();
                _buttonGroups.Add(group, groupButtons);
            }
            if(groupButtons.Contains(button)) groupButtons.Remove(button);
        }

        private static void AddToGroup(TabButton button, String group)
        {
            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Adding tab button '{0}' to group '{1}'.", button.Text, group);
            List<TabButton> groupButtons = null;
            if (_buttonGroups.ContainsKey(group))
            {
                groupButtons = _buttonGroups[group];
            }
            else
            {
                groupButtons = new List<TabButton>();
                _buttonGroups.Add(group, groupButtons);
            }
            groupButtons.Add(button);
        }

        #endregion

    }
}
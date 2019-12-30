using devoctomy.DFramework.Logging;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SwitchWithLabel : ContentView
	{

        #region bindable properties

        public static BindableProperty LabelProperty = BindableProperty.Create("Label",
            typeof(String),
            typeof(SwitchWithLabel),
            "Switch Label",
            BindingMode.Default);

        public static BindableProperty LabelColourProperty = BindableProperty.Create("LabelColour",
            typeof(Color),
            typeof(SwitchWithLabel),
            Color.Accent,
            BindingMode.Default);

        public static BindableProperty OnColourProperty = BindableProperty.Create("OnColour",
            typeof(Color),
            typeof(SwitchWithLabel),
            Color.Accent,
            BindingMode.Default);

        public static BindableProperty IsSwitchedProperty = BindableProperty.Create("IsSwitched",
            typeof(Boolean),
            typeof(SwitchWithLabel),
            false,
            BindingMode.TwoWay,
            null,
            IsSwitchedChanged);

        public BindableProperty SwitchGroupProperty = BindableProperty.Create("SwitchGroup",
            typeof(String),
            typeof(SwitchWithLabel),
            String.Empty,
            BindingMode.Default,
            null,
            SwitchGroupChanged);

        public static BindableProperty FontAttributesProperty = BindableProperty.Create("FontAttributes",
            typeof(FontAttributes),
            typeof(SwitchWithLabel),
            FontAttributes.Bold,
            BindingMode.Default);

        #endregion

        #region private objects

        private static Dictionary<String, List<SwitchWithLabel>> _switchGroups = new Dictionary<String, List<SwitchWithLabel>>();
        private Boolean _isUpdating;

        #endregion

        #region public properties

        public String Label
        {
            get
            {
                return ((String)GetValue(LabelProperty));
            }
            set
            {
                SetValue(LabelProperty, value);
            }
        }

        public Color LabelColour
        {
            get
            {
                return ((Color)GetValue(LabelColourProperty));
            }
            set
            {
                SetValue(LabelColourProperty, value);
            }
        }

        public Color OnColour
        {
            get
            {
                return ((Color)GetValue(OnColourProperty));
            }
            set
            {
                SetValue(OnColourProperty, value);
            }
        }

        public Boolean IsSwitched
        {
            get
            {
                return ((Boolean)GetValue(IsSwitchedProperty));
            }
            set
            {
                SetValue(IsSwitchedProperty, value);
            }
        }

        public String SwitchGroup
        {
            get
            {
                return ((String)GetValue(SwitchGroupProperty));
            }
            set
            {
                SetValue(SwitchGroupProperty, value);
            }
        }

        public FontAttributes FontAttributes
        {
            get
            {
                return ((FontAttributes)GetValue(FontAttributesProperty));
            }
            set
            {
                SetValue(FontAttributesProperty, value);
            }
        }

        #endregion

        #region constructor / destructor

        public SwitchWithLabel ()
		{
			InitializeComponent ();
		}

        #endregion

        #region bindable property change callbacks

        private static void IsSwitchedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SwitchWithLabel switchWithLabel = (SwitchWithLabel)bindable;
            Boolean isPressed = (Boolean)newValue;
            if (!switchWithLabel._isUpdating && !String.IsNullOrEmpty(switchWithLabel.SwitchGroup)) UnpressAllInGroupExcept(switchWithLabel.SwitchGroup, switchWithLabel);
        }

        private static void SwitchGroupChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SwitchWithLabel switchWithLabel = (SwitchWithLabel)bindable;
            String oldGroup = (String)oldValue;
            String newGroup = (String)newValue;
            if (!String.IsNullOrEmpty(oldGroup)) RemoveFromGroup(switchWithLabel, oldGroup);
            if (!String.IsNullOrEmpty(newGroup)) AddToGroup(switchWithLabel, newGroup);
        }

        #endregion

        #region private methods

        private static void UnpressAllInGroupExcept(String group, SwitchWithLabel except)
        {
            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Unpressing all switches in group '{0}', except '{1}'.", group, except.Label);
            List<SwitchWithLabel> groupButtons = null;
            if (_switchGroups.ContainsKey(group))
            {
                groupButtons = _switchGroups[group];

                foreach (SwitchWithLabel curButton in groupButtons)
                {
                    if (curButton != except)
                    {
                        curButton._isUpdating = true;
                        curButton.IsSwitched = false;
                        curButton._isUpdating = false;
                    }
                }
            }
        }

        private static void RemoveFromGroup(SwitchWithLabel button, String group)
        {
            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Removing switch '{0}' from group '{1}'.", button.Label, group);
            List<SwitchWithLabel> groupButtons = null;
            if (_switchGroups.ContainsKey(group))
            {
                groupButtons = _switchGroups[group];
            }
            else
            {
                groupButtons = new List<SwitchWithLabel>();
                _switchGroups.Add(group, groupButtons);
            }
            if (groupButtons.Contains(button)) groupButtons.Remove(button);
        }

        private static void AddToGroup(SwitchWithLabel button, String group)
        {
            DLoggerManager.Instance.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Adding tab button '{0}' to group '{1}'.", button.Label, group);
            List<SwitchWithLabel> groupButtons = null;
            if (_switchGroups.ContainsKey(group))
            {
                groupButtons = _switchGroups[group];
            }
            else
            {
                groupButtons = new List<SwitchWithLabel>();
                _switchGroups.Add(group, groupButtons);
            }
            groupButtons.Add(button);
        }

        #endregion

    }

}
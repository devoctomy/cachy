using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace cachy.Controls
{

    public class LabelEx : Label
    {

        #region bindable properties

        public static BindableProperty EnabledTextColourProperty = BindableProperty.Create("EnabledTextColour",
            typeof(Color),
            typeof(LabelEx),
            Color.Black,
            BindingMode.Default,
            null,
            ColourPropertyChanged);

        public static BindableProperty DisabledTextColourProperty = BindableProperty.Create("DisabledTextColour",
            typeof(Color),
            typeof(LabelEx),
            Color.DarkGray,
            BindingMode.Default,
            null,
            ColourPropertyChanged);

        #endregion

        #region public properties

        public Color EnabledTextColour
        {
            get
            {
                return ((Color)GetValue(EnabledTextColourProperty));
            }
            set
            {
                SetValue(EnabledTextColourProperty, value);
            }
        }

        public Color DisabledTextColour
        {
            get
            {
                return ((Color)GetValue(DisabledTextColourProperty));
            }
            set
            {
                SetValue(DisabledTextColourProperty, value);
            }
        }

        #endregion

        #region constructor / destructor

        public LabelEx()
        {
            UpdateTextColor();
        }

        #endregion

        #region private methods

        private void UpdateTextColor()
        {
            TextColor = IsEnabled ? EnabledTextColour : DisabledTextColour;
        }

        #endregion

        #region base class overrides

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch(propertyName)
            {
                case "IsEnabled":
                    {
                        UpdateTextColor();
                        break;
                    }
            }
        }

        #endregion

        #region bindable property changed callbacks

        private static void ColourPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            LabelEx labelEx = bindable as LabelEx;
            if(labelEx != null)
            {
                labelEx.UpdateTextColor();
            }
        }

        #endregion

    }

}

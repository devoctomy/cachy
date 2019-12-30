using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NumericUpDown : ContentView
	{

        #region bindable properties

        public static BindableProperty MinValueProperty = BindableProperty.Create("MinValue",
            typeof(Int32),
            typeof(NumericUpDown),
            0,
            BindingMode.Default);

        public static BindableProperty ValueProperty = BindableProperty.Create("Value",
            typeof(Int32),
            typeof(NumericUpDown),
            5,
            BindingMode.TwoWay,
            null,
            ValueChanged);

        public static BindableProperty MaxValueProperty = BindableProperty.Create("MaxValue",
            typeof(Int32),
            typeof(NumericUpDown),
            10,
            BindingMode.Default);

        public static BindableProperty GlyphColourProperty = BindableProperty.Create("GlyphColour",
            typeof(Color),
            typeof(NumericUpDown),
            Color.Accent,
            BindingMode.Default);

        public static BindableProperty DisabledGlyphColourProperty = BindableProperty.Create("DisabledGlyphColour",
            typeof(Color),
            typeof(NumericUpDown),
            Color.DarkGray,
            BindingMode.Default);

        public static BindableProperty ValueColourProperty = BindableProperty.Create("ValueColour",
            typeof(Color),
            typeof(NumericUpDown),
            Color.Accent,
            BindingMode.Default);

        public static BindableProperty DisabledValueColourProperty = BindableProperty.Create("DisabledValueColour",
            typeof(Color),
            typeof(NumericUpDown),
            Color.DarkGray,
            BindingMode.Default);

        #endregion

        #region private objects

        private ICommand _downCommand;
        private ICommand _upCommand;

        #endregion

        #region public properties

        public Int32 MinValue
        {
            get
            {
                return ((Int32)GetValue(MinValueProperty));
            }
            set
            {
                SetValue(MinValueProperty, value);
            }
        }

        public String ValueAsString
        {
            get
            {
                return (Value.ToString());
            }
        }

        public Int32 Value
        {
            get
            {
                return ((Int32)GetValue(ValueProperty));
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public Int32 MaxValue
        {
            get
            {
                return ((Int32)GetValue(MaxValueProperty));
            }
            set
            {
                SetValue(MaxValueProperty, value);
            }
        }

        public Color GlyphColour
        {
            get
            {
                return ((Color)GetValue(GlyphColourProperty));
            }
            set
            {
                SetValue(GlyphColourProperty, value);
                OnPropertyChanged("GlyphStateColour");
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
                OnPropertyChanged("GlyphStateColour");
            }
        }

        public Color GlyphStateColour
        {
            get
            {
                return (IsEnabled ? GlyphColour : DisabledGlyphColour);
            }
        }

        public Color ValueColour
        {
            get
            {
                return ((Color)GetValue(ValueColourProperty));
            }
            set
            {
                SetValue(ValueColourProperty, value);
                OnPropertyChanged("ValueStateColour");
            }
        }

        public Color DisabledValueColour
        {
            get
            {
                return ((Color)GetValue(DisabledValueColourProperty));
            }
            set
            {
                SetValue(DisabledValueColourProperty, value);
                OnPropertyChanged("ValueStateColour");
            }
        }

        public Color ValueStateColour
        {
            get
            {
                return (IsEnabled ? ValueColour : DisabledValueColour);
            }
        }

        public ICommand DownCommand
        {
            get
            {
                return (_downCommand);
            }
        }

        public ICommand UpCommand
        {
            get
            {
                return (_upCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public NumericUpDown ()
		{
            _downCommand = new Command(new Action<Object>(DownCommandAction));
            _upCommand = new Command(new Action<Object>(UpCommandAction));
            InitializeComponent();
        }

        #endregion

        #region bindable property change callbacks

        private static void ValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            NumericUpDown numericUpDown = (NumericUpDown)bindable;
            Int32 oldValueInt = (Int32)oldValue;
            Int32 newValueInt = (Int32)newValue;
            if(newValueInt >= numericUpDown.MinValue && newValueInt <= numericUpDown.MaxValue)
            {
                numericUpDown.OnPropertyChanged("ValueAsString");
            }
            else
            {
                if(newValueInt < numericUpDown.MinValue)
                {
                    numericUpDown.Value = numericUpDown.MinValue;
                }
                else if (newValueInt > numericUpDown.MaxValue)
                {
                    numericUpDown.Value = numericUpDown.MaxValue;
                }
            }
        }

        #endregion

        #region commands

        public void DownCommandAction(Object parameter)
        {
            Int32 value = Value - 1;
            if(value >= MinValue)
            {
                Value = value;
            }
        }

        public void UpCommandAction(Object parameter)
        {
            Int32 value = Value + 1;
            if (value <= MaxValue)
            {
                Value = value;
            }
        }

        #endregion

        #region base class events

        private void Root_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "IsEnabled":
                    {
                        bool isEnabled = IsEnabled;
                        OnPropertyChanged("GlyphStateColour");
                        OnPropertyChanged("ValueStateColour");
                        break;
                    }
            }
        }

        #endregion

    }

}
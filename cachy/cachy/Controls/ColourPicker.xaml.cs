using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ColourPicker : ContentView
	{

        #region public events

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        #endregion

        #region private objects

        public static BindableProperty ColoursProperty = BindableProperty.Create("Colours",
            typeof(List<ColourPickerItem>),
            typeof(ColourPicker),
            null,
            BindingMode.Default);

        #endregion

        #region private objects

        private ColourPickerItem _selectedItem;

        #endregion

        #region public properties

        public List<ColourPickerItem> Colours
        {
            get
            {
                return ((List<ColourPickerItem>)GetValue(ColoursProperty));
            }
            set
            {
                SetValue(ColoursProperty, value);
            }
        }

        public ColourPickerItem SelectedItem
        {
            get
            {
                return (_selectedItem);
            }
            set
            {
                if(_selectedItem != value)
                {
                    _selectedItem = value;
                    SelectedItemChanged?.Invoke(this, new SelectedItemChangedEventArgs(_selectedItem, Colours.IndexOf(_selectedItem)));
                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        #endregion

        #region constructor / destructor

        public ColourPicker ()
		{
			InitializeComponent ();
            if (Colours == null) Colours = GetNamedColours();
        }

        #endregion

        #region private methods

        private List<ColourPickerItem> GetNamedColours()
        {
            List<ColourPickerItem> _namedColours = new List<ColourPickerItem>();
            FieldInfo[] namedColours = typeof(Color).GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (FieldInfo curNamedColour in namedColours)
            {
                if (curNamedColour != null && !String.IsNullOrEmpty(curNamedColour.Name))
                {
                    Color curColour = (Color)curNamedColour.GetValue(null);
                    _namedColours.Add(new ColourPickerItem(curNamedColour.Name, 
                        curColour,
                        new Command<object>(new Action<object>(SelectColour))));
                }
            }
            return (_namedColours);
        }

        #endregion

        #region commands

        private void SelectColour(Object parameter)
        {
            SelectedItem = parameter as ColourPickerItem;
        }

        #endregion

    }

}
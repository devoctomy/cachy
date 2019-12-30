using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Tag : ContentView
	{

        #region public events

        public event EventHandler<EventArgs> StartEditing;
        public event EventHandler<EventArgs> StopEditing;
        public event EventHandler<EventArgs> ClickedDelete;
        public event EventHandler<TagValueChangingEventArgs> ValueChanging;
        public event EventHandler<EventArgs> ValueChanged;

        #endregion

        #region bindable properties

        public static BindableProperty TagBackColourProperty = BindableProperty.Create("TagBackColour",
            typeof(Color),
            typeof(TagEditor),
            Color.Blue,
            BindingMode.Default);

        public static BindableProperty TagTextColourProperty = BindableProperty.Create("TagTextColour",
            typeof(Color),
            typeof(TagEditor),
            Color.White,
            BindingMode.Default);

        public static BindableProperty TagBorderColourProperty = BindableProperty.Create("TagBorderColour",
            typeof(Color),
            typeof(TagEditor),
            Color.LightGray,
            BindingMode.Default);

        public static BindableProperty CornerRadiusProperty = BindableProperty.Create("CornerRadius",
            typeof(int),
            typeof(TagEditor),
            4,
            BindingMode.Default);

        public static BindableProperty HasShadowProperty = BindableProperty.Create("HasShadow",
            typeof(bool),
            typeof(TagEditor),
            true,
            BindingMode.Default);

        public static BindableProperty ShowDeleteProperty = BindableProperty.Create("ShowDelete",
            typeof(bool),
            typeof(TagEditor),
            false,
            BindingMode.Default,
            null,
            ShowDeletePropertyChanged);

        public static BindableProperty IsReadOnlyProperty = BindableProperty.Create("IsReadOnly",
            typeof(bool),
            typeof(TagEditor),
            true,
            BindingMode.Default);

        #endregion

        #region private objects

        private string _value = String.Empty;
        private bool _edit;
        private System.Timers.Timer _changeTimer;
        private ICommand _deleteCommand;

        #endregion

        #region public properties

        public string Value
        {
            get
            {
                return (_value);
            }
            set
            {
                bool changed = false;
                try
                {
                    if (_value != value)
                    {
                        _changeTimer.Stop();
                        changed = true;
                        _value = value.ToLower().Replace(" ", String.Empty);
                        OnPropertyChanged("Value");
                        ValueChanging?.Invoke(this, new TagValueChangingEventArgs(_value, value));
                    }
                }
                finally
                {
                    if(changed) _changeTimer.Start();
                }
            }
        }

        public bool ShowEdit
        {
            get
            {
                return (_edit);
            }
            set
            {
                if (_edit != value)
                {
                    _edit = value;
                    if (_edit && IsReadOnly) _edit = false;
                    OnPropertyChanged("ShowEdit");
                    OnPropertyChanged("ShowLabel");
                    if (_edit)
                    {
                        TagEntry.Text = TagEntry.Text;
                        StartEditing?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        StopEditing?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool ShowLabel
        {
            get
            {
                return (!ShowEdit);
            }
        }

        public Color TagBackColour
        {
            get
            {
                return ((Color)GetValue(TagBackColourProperty));
            }
            set
            {
                SetValue(TagBackColourProperty, value);
            }
        }

        public Color TagTextColour
        {
            get
            {
                return ((Color)GetValue(TagTextColourProperty));
            }
            set
            {
                SetValue(TagTextColourProperty, value);
            }
        }

        public Color TagBorderColour
        {
            get
            {
                return ((Color)GetValue(TagBorderColourProperty));
            }
            set
            {
                SetValue(TagBorderColourProperty, value);
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

        public bool HasShadow
        {
            get
            {
                return ((bool)GetValue(HasShadowProperty));
            }
            set
            {
                SetValue(HasShadowProperty, value);
            }
        }

        public bool ShowDelete
        {
            get
            {
                return ((bool)GetValue(ShowDeleteProperty));
            }
            set
            {
                SetValue(ShowDeleteProperty, value);
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((bool)GetValue(IsReadOnlyProperty));
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        public GridLength DeleteColumnWidth
        {
            get
            {
                return (new GridLength(ShowDelete ? 32 : 0, GridUnitType.Absolute));
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                if(_deleteCommand == null)
                {
                    _deleteCommand = new Command(new Action<object>(DeleteCommandAction));
                }
                return (_deleteCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public Tag (string value)
		{
            _changeTimer = new System.Timers.Timer(500);
            _changeTimer.Elapsed += _changeTimer_Elapsed;
            InitializeComponent();
            Value = value;
        }

        #endregion

        #region commands

        private void DeleteCommandAction(object parameter)
        {
            ClickedDelete?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region object events

        private void _changeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _changeTimer.Stop();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TagEditButton_Clicked(object sender, EventArgs e)
        {
            ShowEdit = !IsReadOnly;
        }

        private void TagEntry_Completed(object sender, EventArgs e)
        {
            ShowEdit = false;
        }

        private void TagEntry_Unfocused(object sender, FocusEventArgs e)
        {
            ShowEdit = false;
        }

        #endregion

        #region bindable property changed callbacks

        private static void ShowDeletePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Tag tag = bindable as Tag;
            if (tag != null)
            {
                tag.OnPropertyChanged("DeleteColumnWidth");
            }
        }

        #endregion

    }

}
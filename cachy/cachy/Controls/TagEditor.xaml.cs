using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TagEditor : ContentView
	{

        #region bindable properties

        public static BindableProperty OrientationProperty = BindableProperty.Create("Orientation",
            typeof(StackOrientation),
            typeof(TagEditor),
            StackOrientation.Vertical,
            BindingMode.Default);

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

        public static BindableProperty HorizontalSpacingProperty = BindableProperty.Create("HorizontalSpacing",
            typeof(double),
            typeof(TagEditor),
            4.0d,
            BindingMode.Default);

        public static BindableProperty VerticalSpacingProperty = BindableProperty.Create("VerticalSpacing",
            typeof(double),
            typeof(TagEditor),
            4.0d,
            BindingMode.Default);

        public static BindableProperty ItemHeightProperty = BindableProperty.Create("ItemHeight",
            typeof(double),
            typeof(TagEditor),
            36.0d,
            BindingMode.Default);

        public static BindableProperty TagsProperty = BindableProperty.Create("Tags",
            typeof(ObservableCollection<string>),
            typeof(TagEditor),
            new ObservableCollection<string>(),
            BindingMode.Default,
            null,
            TagsPropertyChanged);

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
            BindingMode.Default,
            null,
            IsReadOnlyPropertyChanged);

        #endregion

        #region private objects

        private Point _currentLocation;
        private System.Timers.Timer _resizeTimer;
        private object _resizeLock = new object();
        private Tag _selected;
        private bool _changingTag;

        #endregion

        #region public properties

        public StackOrientation Orientation
        {
            get
            {
                return ((StackOrientation)GetValue(OrientationProperty));
            }
            set
            {
                SetValue(OrientationProperty, value);
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

        public double HorizontalSpacing
        {
            get
            {
                return ((double)GetValue(HorizontalSpacingProperty));
            }
            set
            {
                SetValue(HorizontalSpacingProperty, value);
            }
        }

        public double VerticalSpacing
        {
            get
            {
                return ((double)GetValue(VerticalSpacingProperty));
            }
            set
            {
                SetValue(VerticalSpacingProperty, value);
            }
        }

        public double ItemHeight
        {
            get
            {
                return ((double)GetValue(ItemHeightProperty));
            }
            set
            {
                SetValue(ItemHeightProperty, value);
            }
        }

        public ObservableCollection<string> Tags
        {
            get
            {
                return ((ObservableCollection<string>)GetValue(TagsProperty));
            }
            set
            {
                SetValue(TagsProperty, value);
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
                SetValue(ShowDeleteProperty, true);
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
                SetValue(IsReadOnlyProperty, true);
            }
        }

        #endregion

        #region constructor / destructor        

        public TagEditor ()
		{
			InitializeComponent ();

            Tags.CollectionChanged += Tags_CollectionChanged;
            _resizeTimer = new System.Timers.Timer(100);
            _resizeTimer.Elapsed += _resizeTimer_Elapsed;
        }

        #endregion

        #region private methods

        private void LayoutChildren()
        {
            _resizeTimer.Stop();
            lock(_resizeLock)
            {
                if (TagLayout.Width > 0)
                {
                    Dictionary<Element, Rectangle> _bounds = new Dictionary<Element, Rectangle>();
                    _currentLocation = new Point(0, 0);
                    View[] children = TagLayout.Children.ToArray();
                    foreach (VisualElement element in children)
                    {
                        Rectangle curElementBounds = PositionElement(element, TagLayout.Width);
                        _bounds.Add(element, curElementBounds);
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        foreach (Element curElement in _bounds.Keys)
                        {
                            AbsoluteLayout.SetLayoutBounds(curElement, _bounds[curElement]);
                        }
                    });
                }
            }
        }

        private Rectangle PositionElement(
            VisualElement element,
            double width)
        {
            Rectangle bounds = Rectangle.Zero;
            switch (Orientation)
            {
                case StackOrientation.Vertical:
                    {
                        double farRightPos = _currentLocation.X + element.WidthRequest;
                        if (farRightPos > width)
                        {
                            _currentLocation = new Point(0, _currentLocation.Y + element.HeightRequest + VerticalSpacing);
                        }
                        bounds = (new Rectangle(_currentLocation.X, _currentLocation.Y, element.WidthRequest, element.HeightRequest));
                        _currentLocation = new Point(_currentLocation.X + element.WidthRequest + HorizontalSpacing, _currentLocation.Y);
                        break;
                    }
                case StackOrientation.Horizontal:
                    {
                        bounds = (new Rectangle(_currentLocation.X, _currentLocation.Y, element.WidthRequest, element.HeightRequest));
                        _currentLocation = new Point(_currentLocation.X + element.WidthRequest + HorizontalSpacing, _currentLocation.Y);
                        break;
                    }
            }
            return (bounds);
        }

        private void ClearTags()
        {
            _currentLocation = new Point(0, 0);
            while (TagLayout.Children.Count > 0)
            {
                Tag curChild = (Tag)TagLayout.Children[0];
                curChild.ClickedDelete += Tag_ClickedDelete;
                curChild.StartEditing += Tag_StartEditing;
                curChild.StopEditing += Tag_StopEditing;
                TagLayout.Children.RemoveAt(0);
            }
        }

        private void AddTags(List<string> tags)
        {
            foreach (string curTag in tags)
            {
                Tag tag = new Tag(curTag)
                {
                    WidthRequest = 128,
                    HeightRequest = 0.0d,
                    IsReadOnly = IsReadOnly,
                    ShowDelete = ShowDelete,
                    TagBackColour = TagBackColour,
                    TagTextColour = TagTextColour,
                    TagBorderColour = TagBorderColour
                };
                tag.ClickedDelete += Tag_ClickedDelete;
                tag.StartEditing += Tag_StartEditing;
                tag.StopEditing += Tag_StopEditing;
                tag.ValueChanging += Tag_ValueChanging;
                TagLayout.Children.Add(tag);
            }
        }

        #endregion

        #region public methods

        public void Refresh()
        {
            ClearTags();
            AddTags(Tags.ToList());
        }

        public void Recreate()
        {
            try
            {
                _changingTag = true;
                Tags.Clear();
                List<string> tags = new List<string>();
                foreach (Tag curTag in TagLayout.Children)
                {
                    tags.Add(curTag.Value);
                    Tags.Add(curTag.Value);
                }
                Refresh();
            }
            finally
            {
                _changingTag = false;
            }
        }

        #endregion

        #region object events

        private void TagLayout_SizeChanged(object sender, EventArgs e)
        {
            _resizeTimer.Stop();
            _resizeTimer.Start();
        }

        private void TagLayout_ChildAdded(object sender, ElementEventArgs e)
        {
            VisualElement element = e.Element as VisualElement;
            element.HeightRequest = ItemHeight;
            if(TagLayout.Width > 0)
            {
                Rectangle elementBounds = PositionElement(element, TagLayout.Width > 0 ? TagLayout.Width : 400);
                AbsoluteLayout.SetLayoutBounds(element, elementBounds);
            }
        }

        private void _resizeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            LayoutChildren();
        }

        private void Tag_ClickedDelete(object sender, EventArgs e)
        {
            _resizeTimer.Stop();
            Tag tag = (Tag)sender;
            tag.ClickedDelete -= Tag_ClickedDelete;
            TagLayout.Children.Remove(tag);
            Tags.Remove(tag.Value);
            _resizeTimer.Start();
        }

        private void Tag_StopEditing(object sender, EventArgs e)
        {
            //do nothing
        }

        private void Tag_StartEditing(object sender, EventArgs e)
        {
            if(_selected != null && _selected != sender)
            {
                _selected.ShowEdit = false;
                _selected = null;
            }
            _selected = (Tag)sender;
        }

        private void Tags_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_changingTag) return;

            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        List<string> newValues = new List<string>();
                        foreach(string curValue in e.NewItems)
                        {
                            newValues.Add(curValue);
                        }
                        AddTags(newValues);
                        break;
                    }
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    {
                        ClearTags();
                        break;
                    }
            }
        }

        private void Tag_ValueChanging(object sender, TagValueChangingEventArgs e)
        {
            try
            {
                _changingTag = true;
                Tags.Remove(e.OldValue);
                Tags.Add(e.NewValue);
            }
            finally
            {
                _changingTag = false;
            }
        }

        #endregion

        #region bindable property changed callbacks

        private static void TagsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TagEditor tagEditor = bindable as TagEditor;
            if (tagEditor != null)
            {
                ObservableCollection<string> tags = (ObservableCollection<string>)newValue;
                tags.CollectionChanged += tagEditor.Tags_CollectionChanged;
                tagEditor.ClearTags();
                tagEditor.AddTags(tags.ToList());
            }
        }

        private static void IsReadOnlyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TagEditor tagEditor = bindable as TagEditor;
            if (tagEditor != null)
            {
                foreach(Tag curTag in tagEditor.TagLayout.Children)
                {
                    curTag.IsReadOnly = (bool)newValue;
                }
            }
        }

        private static void ShowDeletePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TagEditor tagEditor = bindable as TagEditor;
            if (tagEditor != null)
            {
                foreach (Tag curTag in tagEditor.TagLayout.Children)
                {
                    curTag.ShowDelete = (bool)newValue;
                }
            }
        }

        #endregion

    }
}
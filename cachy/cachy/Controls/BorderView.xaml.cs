using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BorderView : ContentView
	{

        #region bindable properties

        public static BindableProperty BorderColourProperty = BindableProperty.Create("BorderColour",
            typeof(Color),
            typeof(BorderView),
            Color.Black,
            BindingMode.Default);

        public static BindableProperty BorderThicknessProperty = BindableProperty.Create("BorderThickness",
            typeof(String),
            typeof(BorderView),
            "0,0,0,0",
            BindingMode.Default,
            null,
            BorderThicknessChanged);

        public static BindableProperty InternalContentProperty = BindableProperty.Create("InternalContent",
            typeof(View),
            typeof(BorderView),
            null,
            BindingMode.Default);

        public static BindableProperty TagProperty = BindableProperty.Create("Tag",
            typeof(object),
            typeof(BorderView),
            null,
            BindingMode.Default);

        #endregion

        #region private objects

        private GridLength _leftBorder = 0;
        private GridLength _topBorder = 0;
        private GridLength _rightBorder = 0;
        private GridLength _bottomBorder = 0;

        #endregion

        #region public properties

        public Color BorderColour
        {
            get
            {
                return ((Color)GetValue(BorderColourProperty));
            }
            set
            {
                SetValue(BorderColourProperty, value);
            }
        }

        public String BorderThickness
        {
            get
            {
                return((String)GetValue(BorderThicknessProperty));
            }
            set
            {
                SetValue(BorderThicknessProperty, value);
            }
        }

        public View InternalContent
        {
            get
            {
                return ((View)GetValue(InternalContentProperty));
            }
            set
            {
                SetValue(InternalContentProperty, value);
            }
        }

        public object Tag
        {
            get
            {
                return ((object)GetValue(TagProperty));
            }
            set
            {
                SetValue(TagProperty, value);
            }
        }

        #endregion

        #region private properties

        public GridLength LeftBorder
        {
            get
            {
                return (_leftBorder);
            }
            set
            {
                if (!_leftBorder.Equals(value))
                {
                    _leftBorder = value;
                    OnPropertyChanged("LeftBorder");                   
                }
            }
        }

        public Boolean HasLeftBorder
        {
            get
            {
                return (LeftBorder.Value > 0);
            }
        }

        public GridLength TopBorder
        {
            get
            {
                return (_topBorder);
            }
            set
            {
                if (!_topBorder.Equals(value))
                {
                    _topBorder = value;
                    OnPropertyChanged("TopBorder");
                }
            }
        }

        public Boolean HasTopBorder
        {
            get
            {
                return (TopBorder.Value > 0);
            }
        }

        public GridLength RightBorder
        {
            get
            {
                return (_rightBorder);
            }
            set
            {
                if (!_rightBorder.Equals(value))
                {
                    _rightBorder = value;
                    OnPropertyChanged("RightBorder");
                }
            }
        }

        public Boolean HasRightBorder
        {
            get
            {
                return (RightBorder.Value > 0);
            }
        }

        public GridLength BottomBorder
        {
            get
            {
                return (_bottomBorder);
            }
            set
            {
                if (!_bottomBorder.Equals(value))
                {
                    _bottomBorder = value;
                    OnPropertyChanged("BottomBorder");
                }
            }
        }

        public Boolean HasBottomBorder
        {
            get
            {
                return (BottomBorder.Value > 0);
            }
        }

        #endregion

        #region constructor / destructor

        public BorderView ()
		{
			InitializeComponent ();


        }

        #endregion

        #region binding property change callbacks

        private static void BorderThicknessChanged(BindableObject bindable, object oldValue, object newValue)
        {
            BorderView borderView = (BorderView)bindable;
            String oldBorder = (String)oldValue;
            String newBorder = (String)newValue;

            String[] margins = newBorder.Split(',');
            if(margins.Length == 4)
            {
                borderView.LeftBorder = new GridLength(Int32.Parse(margins[0]), GridUnitType.Absolute);
                borderView.TopBorder = new GridLength(Int32.Parse(margins[1]), GridUnitType.Absolute);
                borderView.RightBorder = new GridLength(Int32.Parse(margins[2]), GridUnitType.Absolute);
                borderView.BottomBorder = new GridLength(Int32.Parse(margins[3]), GridUnitType.Absolute);

                Int32 LeftColumnRowSpan = 3;
                if (!borderView.HasTopBorder) LeftColumnRowSpan -= 1;
                if (!borderView.HasBottomBorder) LeftColumnRowSpan -= 1;
                Grid.SetRow(borderView.LeftColumn, borderView.HasTopBorder ? 0 : 1);
                Grid.SetRowSpan(borderView.LeftColumn, LeftColumnRowSpan);

                Int32 RightColumnRowSpan = 3;
                if (!borderView.HasTopBorder) RightColumnRowSpan -= 1;
                if (!borderView.HasBottomBorder) RightColumnRowSpan -= 1;
                Grid.SetRow(borderView.RightColumn, borderView.HasTopBorder ? 0 : 1);
                Grid.SetRowSpan(borderView.RightColumn, RightColumnRowSpan);

                Int32 TopRowColumnSpan = 3;
                if (!borderView.HasLeftBorder) TopRowColumnSpan -= 1;
                if (!borderView.HasRightBorder) TopRowColumnSpan -= 1;
                Grid.SetColumn(borderView.TopRow, borderView.HasLeftBorder ? 0 : 1);
                Grid.SetColumnSpan(borderView.TopRow, TopRowColumnSpan);

                Int32 BottomRowColumnSpan = 3;
                if (!borderView.HasLeftBorder) BottomRowColumnSpan -= 1;
                if (!borderView.HasRightBorder) BottomRowColumnSpan -= 1;
                Grid.SetColumn(borderView.BottomRow, borderView.HasLeftBorder ? 0 : 1);
                Grid.SetColumnSpan(borderView.BottomRow, BottomRowColumnSpan);
            }
            else if (!newBorder.Contains(","))
            {
                Int32 singleThickness = Int32.Parse(newBorder);
                GridLength singleLength = new GridLength(singleThickness, GridUnitType.Absolute);
                borderView.LeftBorder = singleLength;
                borderView.TopBorder = singleLength;
                borderView.RightBorder = singleLength;
                borderView.BottomBorder = singleLength;

                Grid.SetRow(borderView.LeftColumn, 0);
                Grid.SetRowSpan(borderView.LeftColumn, 3);
                Grid.SetRow(borderView.RightColumn, 2);
                Grid.SetRowSpan(borderView.RightColumn, 3);
                Grid.SetColumn(borderView.TopRow, 0);
                Grid.SetColumnSpan(borderView.TopRow, 3);
                Grid.SetColumn(borderView.BottomRow, 0);
                Grid.SetColumnSpan(borderView.BottomRow, 3);
            }
            borderView.UpdateChildrenLayout();
        }

        #endregion

    }

}
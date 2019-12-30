using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TabPage : ContentView
	{

        #region bindable properties

        public static BindableProperty TabTitleProperty = BindableProperty.Create("TabTitle",
            typeof(String),
            typeof(TabPage),
            String.Empty);

        public static BindableProperty TabPageContentProperty = BindableProperty.Create("TabPageContent",
            typeof(View),
            typeof(TabPage),
            null);

        #endregion

        #region public properties

        public String TabTitle
        {
            get
            {
                return ((String)GetValue(TabTitleProperty));
            }
            set
            {
                SetValue(TabTitleProperty, value);
            }
        }

        public View TabPageContent
        {
            get
            {
                return ((View)GetValue(TabPageContentProperty));
            }
            set
            {
                SetValue(TabPageContentProperty, value);
            }
        }

        #endregion

        #region constructor / destructor

        public TabPage ()
		{
			InitializeComponent ();
		}

        #endregion

    }

}
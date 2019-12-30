using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TabView : ContentView
	{

        #region bindable properties

        public static BindableProperty TabsProperty = BindableProperty.Create("Tabs",
            typeof(IEnumerable<TabPage>),
            typeof(TabView),
            null,
            BindingMode.Default,
            null,
            TabsChanged);

        public static BindableProperty SelectedTabProperty = BindableProperty.Create("SelectedTab",
            typeof(String),
            typeof(TabView),
            null,
            BindingMode.Default,
            null,
            SelectedTabChanged);

        public BindableProperty TabsBackgroundColourProperty = BindableProperty.Create("TabsBackgroundColour",
            typeof(Color),
            typeof(TabView),
            Application.Current.Resources["BackgroundColour"],
            BindingMode.Default);

        public BindableProperty ButtonBorderColourProperty = BindableProperty.Create("ButtonBorderColour",
            typeof(Color),
            typeof(TabView),
            Application.Current.Resources["Accent"],
            BindingMode.Default,
            null,
            ButtonBorderColourChanged);

        #endregion

        #region private objects

        private String _id = Guid.NewGuid().ToString();
        private Dictionary<TabButton, TabPage> _buttonPageLinks;
        private TabButton selectedTabButton;

        #endregion

        #region public properties

        public IEnumerable<TabPage> Tabs
        {
            get
            {
                return ((IEnumerable<TabPage>)GetValue(TabsProperty));
            }
            set
            {
                SetValue(TabsProperty, value);
            }
        }

        public String SelectedTab
        {
            get
            {
                return ((String)GetValue(SelectedTabProperty));
            }
            set
            {
                SetValue(SelectedTabProperty, value);
            }
        }

        public Color TabsBackgroundColour
        {
            get
            {
                return ((Color)GetValue(TabsBackgroundColourProperty));
            }
            set
            {
                SetValue(TabsBackgroundColourProperty, value);
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

        public TabView ()
		{
			InitializeComponent ();
            _buttonPageLinks = new Dictionary<TabButton, TabPage>();
        }

        #endregion

        #region private methods

        private void CreateTabButtons(IEnumerable<TabPage> tabPages)
        {
            TabButtons.Children.Clear();
            _buttonPageLinks.Clear();
            foreach (TabPage curPage in tabPages)
            {
                TabButton tabButton = new TabButton();
                tabButton.ButtonBorderColour = ButtonBorderColour;
                tabButton.Text = curPage.TabTitle;
                tabButton.ButtonGroup = _id;
                tabButton.Pressed += TabButton_Pressed;
                _buttonPageLinks.Add(tabButton, curPage);
                TabButtons.Children.Add(tabButton);
            }
        }

        #endregion

        #region binding property change callbacks

        private static void TabsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TabView tabView = (TabView)bindable;
            IEnumerable<TabPage> oldPages = (IEnumerable<TabPage>)oldValue;
            IEnumerable<TabPage> newPages = (IEnumerable<TabPage>)newValue;
            tabView.CreateTabButtons(newPages);
        }

        private static void SelectedTabChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TabView tabView = (TabView)bindable;
            String oldTab = (String)oldValue;
            String newTab = (String)newValue;
            foreach(TabButton curButton in tabView._buttonPageLinks.Keys)
            {
                if(curButton.Text == newTab)
                {
                    curButton.SimulatedClick();
                    break;
                }
            }
        }

        private static void ButtonBorderColourChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TabView tabView = (TabView)bindable;
            Color oldColour = (Color)oldValue;
            Color newColor = (Color)newValue;
            foreach(TabButton curButton in tabView._buttonPageLinks.Keys)
            {
                curButton.ButtonBorderColour = newColor;
            }
        }

        private void TabButton_Pressed(object sender, EventArgs e)
        {
            TabButton tabButton = (TabButton)sender;
            TabPage tabPage = _buttonPageLinks[tabButton];
            SelectedTabPage.Content = null;
            SelectedTabPage.Content = tabPage.TabPageContent;
            selectedTabButton = tabButton;
        }

        #endregion

    }

}
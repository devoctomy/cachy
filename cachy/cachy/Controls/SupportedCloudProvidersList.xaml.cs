using cachy.Config;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SupportedCloudProvidersList : ContentView
	{

        #region bindable properties

        public static BindableProperty SelectedProviderTypeProperty = BindableProperty.Create("SelectedProviderType",
            typeof(ProviderType),
            typeof(SupportedCloudProvidersList),
            null,
            BindingMode.Default,
            null,
            SelectedProviderTypeChanged);

        public static BindableProperty VerticalScrollBarVisibilityProperty = BindableProperty.Create("VerticalScrollBarVisibility",
            typeof(ScrollBarVisibility),
            typeof(SupportedCloudProvidersList),
            ScrollBarVisibility.Default,
            BindingMode.Default);

        #endregion

        #region public properties

        public ObservableCollection<ProviderType> SupportedCloudProviderTypes
        {
            get
            {
                return (SupportedProviderTypes.SupportedTypes);
            }
        }

        public ProviderType SelectedProviderType
        {
            get
            {
                return ((ProviderType)GetValue(SelectedProviderTypeProperty));
            }
            set
            {
                SetValue(SelectedProviderTypeProperty, value);
            }
        }

        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get
            {
                return ((ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty));
            }
            set
            {
                SetValue(VerticalScrollBarVisibilityProperty, value);
            }
        }

        #endregion

        #region constructor / destructor

        public SupportedCloudProvidersList ()
		{
			InitializeComponent ();
		}

        #endregion

        #region bindable property changed callbacks

        private static void SelectedProviderTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SupportedCloudProvidersList list = (SupportedCloudProvidersList)bindable;
            if (oldValue != null)
            {
                ProviderType oldProvider = (ProviderType)oldValue;
                oldProvider.IsSelected = false;
            }
            if (newValue != null)
            {
                ProviderType newProvider = (ProviderType)newValue;
                newProvider.IsSelected = true;
            }
        }

        #endregion

        #region public methods

        public void DeselectAll()
        {
            foreach(ProviderType curType in SupportedCloudProviderTypes)
            {
                curType.IsSelected = false;
            }
            SelectedProviderType = null;
        }

        #endregion

    }

}
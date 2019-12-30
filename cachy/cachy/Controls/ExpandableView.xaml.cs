using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExpandableView : ContentView
	{

        #region bindable properties

        public static readonly BindableProperty TitleProperty = BindableProperty.Create("Title", 
            typeof(String), 
            typeof(ExpandableView), 
            "View Title");

        public static readonly BindableProperty ExpandedContentProperty = BindableProperty.Create("ExpandedContent", 
            typeof(View), 
            typeof(ExpandableView), 
            null);

        public static readonly BindableProperty CollapsedContentProperty = BindableProperty.Create("CollapsedContent", 
            typeof(View), 
            typeof(ExpandableView), 
            null);

        public static readonly BindableProperty FooterContentProperty = BindableProperty.Create("FooterContent", 
            typeof(View), 
            typeof(ExpandableView), 
            null);

        public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create("IsExpanded", 
            typeof(Boolean), 
            typeof(ExpandableView), 
            true);

        public static readonly BindableProperty IsFooterVisibleProperty = BindableProperty.Create("IsFooterVisible", 
            typeof(Boolean), 
            typeof(ExpandableView), 
            true);

        //public static readonly BindableProperty BorderColourProperty = BindableProperty.Create("BorderColour",
        //    typeof(Color),
        //    typeof(ExpandableView),
        //    true);

        #endregion

        #region private objects

        private ICommand _contractCommand;
        private ICommand _expandCommand;

        #endregion

        #region public properties

        public String Title
        {
            get
            {
                return ((String)GetValue(TitleProperty));
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }    
        
        public View ExpandedContent
        {
            get
            {
                return ((View)GetValue(ExpandedContentProperty));
            }
            set
            {
                SetValue(ExpandedContentProperty, value);
            }
        }

        public View CollapsedContent
        {
            get
            {
                return ((View)GetValue(CollapsedContentProperty));
            }
            set
            {
                SetValue(CollapsedContentProperty, value);
            }
        }

        public View FooterContent
        {
            get
            {
                return ((View)GetValue(FooterContentProperty));
            }
            set
            {
                SetValue(FooterContentProperty, value);
            }
        }

        public Boolean IsExpanded
        {
            get
            {
                return ((Boolean)GetValue(IsExpandedProperty));
            }
            set
            {
                SetValue(IsExpandedProperty, value);
                OnPropertyChanged("IsCollapsed");
            }
        }

        public Boolean IsCollapsed
        {
            get
            {
                return (!IsExpanded);
            }
        }

        public Boolean IsFooterVisible
        {
            get
            {
                return ((Boolean)GetValue(IsFooterVisibleProperty));
            }
            set
            {
                SetValue(IsFooterVisibleProperty, value);
            }
        }

        public ICommand ContractCommand
        {
            get
            {
                return (_contractCommand);
            }
        }

        public ICommand ExpandCommand
        {
            get
            {
                return (_expandCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public ExpandableView ()
		{
            _contractCommand = new Command(new Action<Object>(ContractCommandAction));
            _expandCommand = new Command(new Action<Object>(ExpandCommandAction));

            InitializeComponent();
            OnPropertyChanged("IsExpanded");
            OnPropertyChanged("IsCollapsed");
            UpdateChildrenLayout();
        }

        #endregion

        #region commands

        public void ContractCommandAction(Object parameter)
        {
            IsExpanded = false;
        }

        public void ExpandCommandAction(Object parameter)
        {
            IsExpanded = true;
        }

        #endregion

    }
}
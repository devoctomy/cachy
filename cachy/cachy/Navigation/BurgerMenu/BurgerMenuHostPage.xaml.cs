using cachy.Controls;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Navigation.BurgerMenu
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BurgerMenuHostPage : ContentPage
	{

        #region bindable properties

        public static BindableProperty FooterHeightProperty = BindableProperty.Create(
            "FooterHeight",
            typeof(double),
            typeof(BurgerMenuHostPage),
            24d,
            BindingMode.Default);

        #endregion

        #region private objects

        private BurgerMenuViewItem _startItem;
        private BurgerMenuViewItem _selectedItem;
        private Stack<BurgerMenuPopupViewItem> _popups;
        private BurgerMenuPopupViewItem _selectedPopupItem;
        private Dictionary<string, BurgerMenuItem> _menuItemsByKey;
        private List<BurgerMenuItem> _commonItems;
        private double _menuNotExpandedWidth = 48;
        private double _menuExpandedWidth = 200;
        private bool _menuExpanded = false;
        private Color _menuBackgroundColor;
        private bool _showPopup = false;
        private View _logoView;
        private View _footerView;
        private ICommand _expandToggleCommand;
        private ICommand _menuItemCommand;

        #endregion

        #region public properties

        public View Logo
        {
            get
            {
                return (_logoView);
            }
            set
            {
                if(_logoView != value)
                {
                    _logoView = value;
                    OnPropertyChanged("Logo");
                }
            }
        }

        public View Footer
        {
            get
            {
                return (_footerView);
            }
            set
            {
                if(_footerView != value)
                {
                    _footerView = value;
                    OnPropertyChanged("Footer");
                }
            }
        }

        //!!!This isn't working when bound to for some reason
        public double FooterHeight
        {
            get
            {
                return ((double)GetValue(FooterHeightProperty));
            }
            set
            {
                SetValue(FooterHeightProperty, value);
                OnPropertyChanged("FooterRowHeight");
            }
        }

        public BurgerMenuViewItem StartItem
        {
            get
            {
                return (_startItem);
            }
        }

        public BurgerMenuViewItem SelectedItem
        {
            get
            {
                return (_selectedItem);
            }
        }

        public BurgerMenuPopupViewItem SelectedPopupItem
        {
            get
            {
                return (_selectedPopupItem);
            }
        }

        public IReadOnlyList<BurgerMenuPopupViewItem> Popups
        {
            get
            {
                return (_popups.ToList());
            }
        }

        public bool PopupsVisible
        {
            get
            {
                return (Popups.Count > 0);
            }
        }

        public List<BurgerMenuItem> VisibleSelectedItemChildItems
        {
            get
            {
                if(SelectedItem != null)
                {
                    List<BurgerMenuItem> visibleSelectedItemChildItems = SelectedItem.ChildItems.Where(bmi => bmi.IsVisible).ToList();
                    return (visibleSelectedItemChildItems);
                }
                else
                {
                    return (null);
                }
            }
        }

        public List<BurgerMenuItem> CommonItems
        {
            get
            {
                return (_commonItems);
            }
        }

        public double MenuNotExpandedWidth
        {
            get
            {
                return (_menuNotExpandedWidth);
            }
            set
            {
                if(_menuNotExpandedWidth != value)
                {
                    _menuNotExpandedWidth = value;
                    if (!IsMenuExpanded)
                    {
                        BurgerMenuFrame.WidthRequest = MenuNotExpandedWidth;
                    }
                }
                OnPropertyChanged("MenuNotExpandedWidth");
                OnPropertyChanged("PageFrameMargin");
                OnPropertyChanged("BurgerTopRowHeight");
            }
        }

        public GridLength FooterRowHeight
        {
            get
            {
                return (new GridLength(FooterHeight, GridUnitType.Absolute));
            }
        }

        public Thickness PageFrameMargin
        {
            get
            {
                return (new Thickness(MenuNotExpandedWidth, 0, 0, 0));
            }
        }

        public GridLength BurgerTopRowHeight
        {
            get
            {
                return (new GridLength(MenuNotExpandedWidth, GridUnitType.Absolute));
            }
        }

        public double MenuExpandedWidth
        {
            get
            {
                return (_menuExpandedWidth);
            }
            set
            {
                if(_menuExpandedWidth != value)
                {
                    _menuExpandedWidth = value;
                    if (IsMenuExpanded)
                    {
                        BurgerMenuFrame.WidthRequest = MenuExpandedWidth;
                    }
                    OnPropertyChanged("MenuExpandedWidth");
                }
            }
        }

        public bool IsMenuExpanded
        {
            get
            {
                return (_menuExpanded);
            }
            set
            {
                if(_menuExpanded != value)
                {
                    _menuExpanded = value;
                    if (!_menuExpanded)
                    {
                        UpdateMenuButtonModes();
                    }
                    AnimateBurgerMenu(value);
                    OnPropertyChanged("IsMenuExpanded");
                    OnPropertyChanged("IsMenuContracted");
                }
            }
        }

        public bool IsMenuContracted
        {
            get
            {
                return (!IsMenuExpanded);
            }
        }

        public ICommand ExpandToggleCommand
        {
            get
            {
                return (_expandToggleCommand);
            }
        }

        public ICommand MenuItemCommand
        {
            get
            {
                return (_menuItemCommand);
            }
        }

        public Color MenuBackgroundColor
        {
            get
            {
                return (_menuBackgroundColor);
            }
            set
            {
                if(_menuBackgroundColor != value)
                {
                    _menuBackgroundColor = value;
                    OnPropertyChanged("MenuBackgroundColor");
                }
            }
        }

        public bool ShowPopup
        {
            get
            {
                return (_showPopup);
            }
            set
            {
                if (_showPopup != value)
                {
                    _showPopup = value;
                    OnPropertyChanged("ShowPopup");
                }
            }
        }

        #endregion

        #region constructor / destructor

        public BurgerMenuHostPage ()
		{
            _popups = new Stack<BurgerMenuPopupViewItem>();
            _expandToggleCommand = new Command(new Action(ExpandToggleCommandAction));
            _menuItemCommand = new Command(new Action<object>(MenuItemCommandAction));
            _menuItemsByKey = new Dictionary<string, BurgerMenuItem>();
            _commonItems = new List<BurgerMenuItem>();
            InitializeComponent();
        }

        #endregion

        #region private methods

        private void AnimateBurgerMenu(bool expanded)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseMed | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Animate burger menu, expanded = '{0}'.", expanded);

            ViewExtensions.CancelAnimations(BurgerMenuFrame);
            Animation animate = new Animation(d => BurgerMenuFrame.WidthRequest = d,
                expanded ? MenuNotExpandedWidth : MenuExpandedWidth,
                expanded ? MenuExpandedWidth : MenuNotExpandedWidth,
                Easing.SpringOut);

            animate.Commit(BurgerMenuFrame,
                "BurgerMenuExpandAnimation",
                16,
                250,
                finished: AnimationComplete);

            LogoView.FadeTo(
                expanded ? 1.0f : 0.0f,
                250,
                Easing.SinOut);
        }

        private void AnimationComplete(double arg1, bool arg2)
        {
            //Let's show all button text now that the menu is open
            UpdateMenuButtonModes();
        }

        private void AttachMenuItem(BurgerMenuItem menuItem)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Attaching burger menu item '{0}'.", menuItem.Key);

            _menuItemsByKey.Add(menuItem.Key, menuItem);
            menuItem.AttachCommand(this, MenuItemCommand);
            foreach(BurgerMenuItem curChildItem in menuItem.ChildItems)
            {
                AttachMenuItem(curChildItem);
            }
        }

        private void UpdateMenuButtonModes()
        {
            //Hide all button text before closing the menu
            if(SelectedItem != null)
            {
                foreach (BurgerMenuItem curItem in SelectedItem.ChildItems)
                {
                    curItem.ButtonMode = _menuExpanded ? GlyphButton.ButtonMode.GlyphAndTextLeftRight : GlyphButton.ButtonMode.GlyphOnly;
                }
            }
            foreach (BurgerMenuItem curItem in CommonItems)
            {
                curItem.ButtonMode = _menuExpanded ? GlyphButton.ButtonMode.GlyphAndTextLeftRight : GlyphButton.ButtonMode.GlyphOnly;
            }
        }

        private void SetCommonItemEnables()
        {
            foreach(BurgerMenuItem curCommonItem in _commonItems)
            {
                curCommonItem.IsEnabled = (_selectedItem != curCommonItem);
            }
        }

        #endregion

        #region public methods

        public void DisplayPage(string key,
            out BurgerMenuViewItem item)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Displaying burger menu page '{0}'.", key);

            item = null;
            if (_menuItemsByKey.ContainsKey(key))
            {
                item = _menuItemsByKey[key] as BurgerMenuViewItem;
                if (item != null)
                {
                    _popups.Clear();
                    SelectItem(item);
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Cannot display page '{0}' as it is of the wrong type.", key));
                }
            }
            else
            {
                throw new KeyNotFoundException(String.Format("Burger menu item with the key '{0}' was not found.", key));
            }
        }

        public void DisplayPopup(string key,
            out BurgerMenuPopupViewItem item)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Displaying burger menu popup '{0}'.", key);

            item = null;
            if (_menuItemsByKey.ContainsKey(key))
            {
                item = _menuItemsByKey[key] as BurgerMenuPopupViewItem;
                if (item != null)
                {
                    SelectPopup(item);
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Cannot display popup '{0}' as it is of the wrong type.", key));
                }
            }
            else
            {
                throw new KeyNotFoundException(String.Format("Burger menu item with the key '{0}' was not found.", key));
            }
        }

        public void SelectItem(BurgerMenuViewItem item)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Selecting burger menu item '{0}'.", item.Key);

            _selectedItem = item;
            UpdateMenuButtonModes();
            OnPropertyChanged("SelectedItem");
            OnPropertyChanged("VisibleSelectedItemChildItems");
            OnPropertyChanged("CommonItems");
            SetCommonItemEnables();
            ShowPopup = false;
        }

        public void SelectPopup(BurgerMenuPopupViewItem item)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Selecting burger menu popup '{0}'.", item.Key);

            _selectedPopupItem = item;
            OnPropertyChanged("SelectedPopupItem");
            _popups.Push(item);
            ShowPopup = true;
        }

        public void ClosePopup()
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Closing buger menu popup.");

            _popups.Pop();
            _selectedPopupItem = _popups.Count > 0 ? _popups.Peek() : null;
            OnPropertyChanged("SelectedPopupItem");
            ShowPopup = _selectedPopupItem != null;
        }

        public void Setup(List<BurgerMenuViewItem> rootItems,
            string startItemKey,
            List<BurgerMenuViewItem> commonItems)
        {
            App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Setting up burger menu.");

            if (rootItems == null || rootItems.Count == 0) throw new ArgumentException("Argument must not be null or empty.", "rootItems");
            if(string.IsNullOrEmpty(startItemKey)) throw new ArgumentException("Argument must not be null.", "startItemKey");
            _menuItemsByKey.Clear();
            _startItem = rootItems.Where(bmi => bmi.Key == startItemKey).First();
            foreach (BurgerMenuViewItem curViewItem in rootItems)
            {
                AttachMenuItem(curViewItem);
            }
            _commonItems.Clear();
            if (commonItems != null && commonItems.Count > 0)
            {
                foreach(BurgerMenuViewItem curViewItem in commonItems)
                {
                    AttachMenuItem(curViewItem);
                    _commonItems.Add(curViewItem);
                }
            }
        }

        public void SetChildCommandItemEnabledState(
            string parentKey,
            string childKey,
            bool enabled)
        {
            if(_menuItemsByKey.ContainsKey(parentKey))
            {
                BurgerMenuItem parent = _menuItemsByKey[parentKey];
                IEnumerable<BurgerMenuItem> matches = parent.ChildItems.Where(bmi => bmi.Key == childKey);
                if(matches.Any())
                {
                    BurgerMenuItem first = matches.First();
                    if(first is BurgerMenuCommandItem)
                    {
                        first.IsEnabled = enabled;
                    }
                }
            }
        }

        #endregion

        #region commands

        private void ExpandToggleCommandAction()
        {
            IsMenuExpanded = !IsMenuExpanded;
        }

        private void MenuItemCommandAction(object parameter)
        {
            BurgerMenuItem item = parameter as BurgerMenuItem;
            if(item != null)
            {
                App.AppLogger.Logger.Log(devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | devoctomy.DFramework.Logging.Interfaces.LoggerMessageType.Information, "Burger menu item clicked '{0}'.", item.Key);

                //determine what type this type is before performing any actions
                if (item.GetType() == typeof(BurgerMenuViewItem))
                {
                    App.Controller.NavigateTo(item.Key);
                }
                else if (item.GetType() == typeof(BurgerMenuPopupViewItem))
                {
                    App.Controller.ShowPopup(item.Key);
                }
                else if(item.GetType() == typeof(BurgerMenuCommandItem))
                {
                    BurgerMenuCommandItem commandItem = item as BurgerMenuCommandItem;
                    BurgerMenuViewItem selectedViewItem = SelectedItem as BurgerMenuViewItem;
                    View viewInstance = selectedViewItem.PageViewInstance;
                    commandItem.Invoke(viewInstance.BindingContext, new object[] { null });
                }
                IsMenuExpanded = false;
            }
        }

        #endregion


    }

}
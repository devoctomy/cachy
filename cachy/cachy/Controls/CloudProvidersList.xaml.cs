using cachy.Config;
using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CloudProvidersList : ContentView
	{

        #region bindable properties

        public static BindableProperty ConfiguredCloudProvidersProperty = BindableProperty.Create("ConfiguredCloudProviders",
            typeof(ObservableCollection<CloudProvider>),
            typeof(CloudProvidersList),
            null,
            BindingMode.Default,
            null,
            CloudProvidersChanged);

        public static BindableProperty SelectedProviderProperty = BindableProperty.Create("SelectedProvider",
            typeof(CloudProvider),
            typeof(CloudProvidersList),
            null,
            BindingMode.Default,
            null,
            SelectedProviderChanged);

        public static BindableProperty AllowRemoveProviderProperty = BindableProperty.Create("AllowRemoveProvider",
            typeof(bool),
            typeof(CloudProvidersList),
            false,
            BindingMode.Default);

        public static BindableProperty SelectionModeProperty = BindableProperty.Create("SelectionMode",
            typeof(ListViewSelectionMode),
            typeof(CloudProvidersList),
            ListViewSelectionMode.None,
            BindingMode.Default);

        #endregion

        #region private objects

        private bool _isRefreshingProviders;

        private ICommand _removeCommand;

        #endregion

        #region public properties

        public ObservableCollection<CloudProvider> ConfiguredCloudProviders
        {
            get
            {
                return ((ObservableCollection<CloudProvider>)GetValue(ConfiguredCloudProvidersProperty));
            }
            set
            {
                SetValue(ConfiguredCloudProvidersProperty, value);
            }
        }

        public bool AllowRemoveProvider
        {
            get
            {
                return ((bool)GetValue(AllowRemoveProviderProperty));
            }
            set
            {
                SetValue(AllowRemoveProviderProperty, value);
            }
        }

        public CloudProvider SelectedProvider
        {
            get
            {
                return((CloudProvider)GetValue(SelectedProviderProperty));
            }
            set
            {
                SetValue(SelectedProviderProperty, value);
            }
        }

        public ListViewSelectionMode SelectionMode
        {
            get
            {
                return ((ListViewSelectionMode)GetValue(SelectionModeProperty));
            }
            set
            {
                SetValue(SelectionModeProperty, value);
            }
        }

        public bool IsRefreshingProviders
        {
            get
            {
                return (_isRefreshingProviders);
            }
            set
            {
                if(_isRefreshingProviders != value)
                {
                    _isRefreshingProviders = value;
                    OnPropertyChanged("IsRefreshingProviders");
                }
            }
        }

        public bool HasCloudProviders
        {
            get
            {
                bool hasProviders = (ConfiguredCloudProviders != null && ConfiguredCloudProviders.Count > 0);
                return (hasProviders);
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return (_removeCommand);
            }
            set
            {
                if(_removeCommand != value)
                {
                    _removeCommand = value;
                    OnPropertyChanged("RemoveCommand");
                }
            }
        }

        #endregion

        #region constructor / destructor

        public CloudProvidersList ()
		{
			InitializeComponent ();
            _removeCommand = new Command<object>(new Action<object>(RemoveCommandAction));
        }

        #endregion

        #region commands

        public async void RemoveCommandAction(object parameter)
        {
            CloudProvider provider = parameter as CloudProvider;
            if (provider != null && AllowRemoveProvider)
            {
                bool remove = true;
                if (provider.CheckIsInUse())
                {
                    remove = await App.Controller.MainPageInstance.DisplayAlert("Remove Provider",
                        "This provider is currently in use by one of your vaults, removing the provider will also remove the local copy of the vault. Are you sure you want to remove it?",
                        "Yes", "No");
                }
                if (remove)
                {
                    List<VaultIndex> matches = VaultIndexFile.Instance.Indexes.Where(vi => vi.Provider == provider.ID).ToList();
                    while(matches.Count > 0)
                    {
                        VaultIndexFile.Instance.RemoveFromVault(matches[0]);
                        matches.RemoveAt(0);
                    }
                    CloudProviders.Instance.RemoveProvider(provider.ID);
                    
                    //Reload the vault index file
                    VaultIndexFile.Invalidate();
                    VaultIndexFile instance = VaultIndexFile.Instance;
                }
            }
        }

        #endregion

        #region public methods

        public void DeSelect()
        {
            if(SelectedProvider != null)
            {
                SelectedProvider.IsSelected = false;
                SelectedProvider = null;
            }
            ConfiguredCloudProvidersList.SelectedItem = null;
        }

        #endregion

        #region base class overrides

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch(propertyName)
            {
                case "IsEnabled":
                    {
                        if(!IsEnabled)
                        {
                            ConfiguredCloudProvidersList.SelectedItem = null;
                        }
                        break;
                    }
            }
        }

        #endregion

        #region bindable property changed callbacks

        private static async void CloudProvidersChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CloudProvidersList list = (CloudProvidersList)bindable;
            try
            {
                list.IsRefreshingProviders = true;
                if (oldValue != null)
                {
                    ObservableCollection<CloudProvider> oldProviders = (ObservableCollection<CloudProvider>)oldValue;
                    oldProviders.CollectionChanged -= list.Providers_CollectionChanged;
                }
                ObservableCollection<CloudProvider> newProviders = (ObservableCollection<CloudProvider>)newValue;
                if (newProviders != null)
                {
                    foreach (CloudProvider curProvider in newProviders)
                    {
                        curProvider.IsSelected = false;
                        await curProvider.CheckCredentialAccessAsync();
                    }
                    newProviders.CollectionChanged += list.Providers_CollectionChanged;
                }
            }
            finally
            {
                list.IsRefreshingProviders = false;
                list.OnPropertyChanged("HasCloudProviders");
            }          
        }

        private static void SelectedProviderChanged(BindableObject bindable, object oldValue, object newValue)
        {
            CloudProvidersList list = (CloudProvidersList)bindable;
            if(oldValue != null)
            {
                CloudProvider oldProvider = (CloudProvider)oldValue;
                oldProvider.IsSelected = false;
            }
            if(newValue != null)
            {
                CloudProvider newProvider = (CloudProvider)newValue;
                newProvider.IsSelected = true;
            }
        }

        #endregion

        #region object events

        private void Providers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("HasCloudProviders");
        }

        #endregion

    }

}
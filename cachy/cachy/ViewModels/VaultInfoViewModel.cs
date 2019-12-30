using cachy.Config;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class VaultInfoViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private VaultInfoView _view;
        private VaultIndex _index;

        private ICommand _backCommand;

        #endregion

        #region public properties

        public VaultInfoView View
        {
            get
            {
                return (_view);
            }
        }

        public VaultIndex Index
        {
            get
            {
                return (_index);
            }
            set
            {
                if(_index != value)
                {
                    _index = value;
                    NotifyPropertyChanged("Index");
                    NotifyPropertyChanged("ProviderName");
                }
            }
        }

        public string ProviderName
        {
            get
            {
                if(Index != null)
                {
                    if (Index.IsCloudSynced)
                    {
                        string provider = Index.Provider;
                        IEnumerable<CloudProvider> matches = CloudProviders.Instance.Providers.Where(cp => cp.ID == provider);
                        if (matches.Any())
                        {
                            return (matches.First().ProviderKey);
                        }
                        else
                        {
                            return ("Provider not found!");
                        }
                    }
                    else
                    {
                        return ("Not synchronised with the cloud.");
                    }
                }
                else
                {
                    return (String.Empty);
                }
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return (_backCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public VaultInfoViewModel(VaultInfoView view)
        {
            _view = view;

            _backCommand = new Command(new Action<Object>(BackCommandAction));
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            switch(key)
            {
                case "VaultIndex":
                    {
                        Index = parameter as VaultIndex;
                        break;
                    }
            }
        }

        public void SetParameters(params KeyValuePair<String, Object>[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                foreach (KeyValuePair<String, Object> curParameter in parameters)
                {
                    SetParameter(curParameter.Key,
                        curParameter.Value);
                }
            }
        }

        public void OnClosePopup(View item, object parameter)
        {

        }

        public void OnNavigateTo(View view, object parameter)
        {

        }

        public void OnNavigateFrom(View view, object parameter)
        {

        }

        public void OnGoBack(View view, object parameter)
        {

        }

        #endregion

        #region commands

        public void BackCommandAction(Object parameter)
        {
            App.Controller.ClosePopup(null);
        }

        #endregion

    }

}

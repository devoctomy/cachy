using cachy.Config;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Build.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class ChangeLogViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private ChangeLogView _view;
        private ObservableCollection<ReleaseUIItem> _releases;
        private ICommand _backCommand;

        #endregion

        #region public properties

        public ChangeLogView View
        {
            get
            {
                return (_view);
            }
        }

        public ObservableCollection<ReleaseUIItem> Releases
        {
            get
            {
                return (_releases);
            }
            set
            {
                if(_releases != value)
                {
                    _releases = value;
                    NotifyPropertyChanged("Changes");
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

        public ChangeLogViewModel(ChangeLogView view)
        {
            _view = view;
            GetChanges();
            _backCommand = new Command(new Action<Object>(BackCommandAction));
        }

        #endregion

        #region private methods

        private void GetChanges()
        {
            Releases = new ObservableCollection<ReleaseUIItem>();
            foreach(Release curRelease in ChangeLog.Instance.Releases.Values)
            {
                Releases.Add(ReleaseUIItem.FromRelease(curRelease));
            }
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            //do nothing, we shouldn't have any paramaters for this view model
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

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

    public class AcknowledgementsViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private AcknowledgementsView _view;

        private ICommand _backCommand;
        private ICommand _acknowledgementSelectedCommand;

        #endregion

        #region public properties

        public AcknowledgementsView View
        {
            get
            {
                return (_view);
            }
        }
        
        public ObservableCollection<Acknowledgement> Acknowledgements
        {
            get
            {
                return (Config.Acknowledgements.Instance.Acks);
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return (_backCommand);
            }
        }

        public ICommand AcknowledgementSelectedCommand
        {
            get
            {
                return (_acknowledgementSelectedCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public AcknowledgementsViewModel(AcknowledgementsView view)
        {
            _view = view;
            GetAcknowledgements();
            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acknowledgementSelectedCommand= new Command(new Action<Object>(AcknowledgementSelectedCommandAction));
        }

        #endregion

        #region private methods

        private void GetAcknowledgements()
        {
            
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

        public void AcknowledgementSelectedCommandAction(Object parameter)
        {
            Acknowledgement acknowledgement = (Acknowledgement)parameter;
            try
            {
                Device.OpenUri(new Uri(acknowledgement.Website));
            }
            catch (Exception)
            { }
        }

        #endregion

    }

}

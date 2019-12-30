using cachy.Controls;
using cachy.Navigation.BurgerMenu;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data.Cloud;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class AmazonS3SetupViewViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private AmazonS3SetupView _view;
        private AmazonS3Config _s3Creds;
        private AmazonS3Region _selectedRegion = null;

        private ICommand _backCommand;
        private ICommand _acceptCommand;

        #endregion

        #region public properties

        public AmazonS3SetupView View
        {
            get
            {
                return (_view);
            }
        }

        public string AccessID
        {
            get
            {
                return (_s3Creds.AccessID);
            }
            set
            {
                if (_s3Creds.AccessID != value)
                {
                    _s3Creds.AccessID = value;
                    NotifyPropertyChanged("AccessID");
                }
            }
        }

        public string SecretKey
        {
            get
            {
                return (_s3Creds.SecretKey);
            }
            set
            {
                if (_s3Creds.SecretKey != value)
                {
                    _s3Creds.SecretKey = value;
                    NotifyPropertyChanged("SecretKey");
                }
            }
        }

        public AmazonS3Region SelectedRegion
        {
            get
            {
                return (_selectedRegion);
            }
            set
            {
                if (_selectedRegion != value)
                {
                    _selectedRegion = value;
                    _s3Creds.Region = _selectedRegion.Name;
                    NotifyPropertyChanged("SelectedRegion");
                }
            }
        }

        public string BucketName
        {
            get
            {
                return (_s3Creds.BucketName);
            }
            set
            {
                if (_s3Creds.BucketName != value)
                {
                    _s3Creds.BucketName = value;
                    NotifyPropertyChanged("BucketName");
                }
            }
        }

        public string Path
        {
            get
            {
                return (_s3Creds.Path);
            }
            set
            {
                if (_s3Creds.Path != value)
                {
                    _s3Creds.Path = value;
                    NotifyPropertyChanged("Path");
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

        public ICommand AcceptCommand
        {
            get
            {
                return (_acceptCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public AmazonS3SetupViewViewModel(AmazonS3SetupView view)
        {
            _view = view;

            _s3Creds = new AmazonS3Config();
            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acceptCommand = new Command(new Action<Object>(AcceptCommandAction));
        }

        #endregion

        #region base class overrides

        protected override bool OnValidate()
        {
            if (String.IsNullOrEmpty(AccessID)) return (false);
            if (String.IsNullOrEmpty(SecretKey)) return (false);
            if (String.IsNullOrEmpty(_s3Creds.Region)) return (false);
            if (String.IsNullOrEmpty(BucketName)) return (false);
            if (String.IsNullOrEmpty(Path)) return (false);

            return (true);
        }

        protected override void IsValidChanged(bool isValid)
        {
            EnableDisableAccept(isValid);
        }

        #endregion

        #region private methods

        private void EnableDisableAccept(bool enabled)
        {
            GlyphButton accept = View.FindByName<GlyphButton>("AcceptButton");
            if (accept != null)
            {
                accept.IsEnabled = enabled;
            }
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            switch(key)
            {
                case "S3Config":
                    {
                        AmazonS3Config config = (AmazonS3Config)parameter;
                        _s3Creds = config;
                        NotifyPropertyChanged("AccessID");
                        NotifyPropertyChanged("SecretKey");
                        NotifyPropertyChanged("Region");
                        NotifyPropertyChanged("BucketName");
                        NotifyPropertyChanged("Path");
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

        public void AcceptCommandAction(Object parameter)
        {
            App.Controller.ClosePopup(_s3Creds);
        }

        #endregion

    }

}

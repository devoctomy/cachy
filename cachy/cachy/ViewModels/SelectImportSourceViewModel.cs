using cachy.Config;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Data.Importers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class SelectImportSourceViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private SelectImportSourceView _view;
        private ImportSource _selectedImportSource;
        private CSVImporter _csvImporter;

        private ICommand _backCommand;
        private ICommand _acceptCommand;

        #endregion

        #region public properties

        public SelectImportSourceView View
        {
            get
            {
                return (_view);
            }
        }

        public ObservableCollection<ImportSource> ImportSources
        {
            get
            {
                return (SupportedImportSources.SupportedSources);
            }
        }

        public ImportSource SelectedImportSource
        {
            get
            {
                return (_selectedImportSource);
            }
            set
            {
                if(_selectedImportSource != value)
                {
                    if(_selectedImportSource != null)
                    {
                        _selectedImportSource.IsSelected = false;
                    }
                    _selectedImportSource = value;
                    _selectedImportSource.IsSelected = true;
                    NotifyPropertyChanged("SelectedImportSource");
                    NotifyPropertyChanged("SourceIsSelected");
                }
            }
        }

        public bool SourceIsSelected
        {
            get
            {
                return (SelectedImportSource != null);
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

        public SelectImportSourceViewModel(SelectImportSourceView view)
        {
            _view = view;

            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acceptCommand = new Command(new Action<Object>(AcceptCommandAction));
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            switch(key)
            {
                case "Format":
                    {
                        Common.CSVFormat format = (Common.CSVFormat)parameter;
                        IEnumerable<ImportSource> matches = SupportedImportSources.SupportedSources.Where(ins => ins.Format == format);
                        if(matches.Any())
                        {
                            SelectedImportSource = matches.First();
                        }
                        break;
                    }
                case "CSVImporter":
                    {
                        _csvImporter = parameter as CSVImporter;
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
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("ImportSource", SelectedImportSource);
            parameters.Add("CSVImporter", _csvImporter);
            App.Controller.ClosePopup(parameters);
        }

        #endregion

    }

}

using cachy.Data;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Data.Importers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class ImportMappingViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private ImportMappingView _view;
        private ObservableCollection<string> _importHeaders;
        private ObservableCollection<StandardFieldAttribute> _standardFields;
        private ObservableCollection<ImportFieldMapping> _mappings;
        private string _selectedHeader;
        private StandardFieldAttribute _selectedField;
        private CSVImporter _csvImporter;

        private ICommand _addMappingCommand;
        private ICommand _removeMappingCommand;
        private ICommand _backCommand;
        private ICommand _acceptCommand;

        #endregion

        #region public properties

        public ImportMappingView View
        {
            get
            {
                return (_view);
            }
        }

        public ObservableCollection<string> ImportHeaders
        {
            get
            {
                return (_importHeaders);
            }
            private set
            {
                if (_importHeaders != value)
                {
                    _importHeaders = value;
                    NotifyPropertyChanged("ImportHeaders");
                }
            }
        }

        public ObservableCollection<StandardFieldAttribute> StandardFields
        {
            get
            {
                return (_standardFields);
            }
            private set
            {
                if(_standardFields != value)
                {
                    _standardFields = value;
                    NotifyPropertyChanged("StandardFields");
                }
            }

        }

        public ObservableCollection<ImportFieldMapping> Mappings
        {
            get
            {
                return (_mappings);
            }
        }

        public bool MappingsConfigured
        {
            get
            {
                return (Mappings.Count > 0);
            }
        }

        public string SelectedHeader
        {
            get
            {
                return (_selectedHeader);
            }
            set
            {
                if(_selectedHeader != value)
                {
                    _selectedHeader = value;
                    NotifyPropertyChanged("SelectedHeader");
                    NotifyPropertyChanged("CanAddMapping");
                }
            }
        }

        public StandardFieldAttribute SelectedField
        {
            get
            {
                return (_selectedField);
            }
            set
            {
                if(_selectedField != value)
                {
                    _selectedField = value;
                    NotifyPropertyChanged("SelectedField");
                    NotifyPropertyChanged("CanAddMapping");
                }
            }
        }

        public bool CanAddMapping
        {
            get
            {
                return (!String.IsNullOrEmpty(SelectedHeader) && SelectedField != null);
            }
        }

        public ICommand AddMappingCommand
        {
            get
            {
                return (_addMappingCommand);
            }
        }

        public ICommand RemoveMappingCommand
        {
            get
            {
                return (_removeMappingCommand);
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

        public ImportMappingViewModel(ImportMappingView view)
        {
            _view = view;

            _mappings = new ObservableCollection<ImportFieldMapping>();
            _addMappingCommand = new Command(new Action<Object>(AddMappingCommandAction));
            _removeMappingCommand = new Command(new Action<Object>(RemoveMappingCommandAction));
            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acceptCommand = new Command(new Action<Object>(AcceptCommandAction));
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            switch(key)
            {
                case "CSVImporter":
                    {
                        _csvImporter = parameter as CSVImporter;
                        break;
                    }
                case "ImportHeaders":
                    {
                        List<string> importHeaders = parameter as List<string>;
                        ObservableCollection<string> headers = new ObservableCollection<string>();
                        foreach(string curHeader in importHeaders)
                        {
                            headers.Add(curHeader);
                        }
                        ImportHeaders = headers;
                        break;
                    }
                case "StandardFields":
                    {
                        List<StandardFieldAttribute> standardFields = parameter as List<StandardFieldAttribute>;
                        ObservableCollection<StandardFieldAttribute> fields = new ObservableCollection<StandardFieldAttribute>();
                        foreach (StandardFieldAttribute curField in standardFields)
                        {
                            fields.Add(curField);
                        }
                        StandardFields = fields;
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

        public async void AddMappingCommandAction(Object parameter)
        {
            string header = SelectedHeader;
            StandardFieldAttribute selectedField = SelectedField;

            IEnumerable<ImportFieldMapping> existing = Mappings.Where(ifm => ifm.Attribute == selectedField);

            if(existing.Any())
            {
                await App.Controller.MainPageInstance.DisplayAlert("Add Mapping",
                    String.Format("A mapping has already been configured for '{0}'.", selectedField.DisplayName),
                    "OK");
            }
            else
            {
                ImportFieldMapping mapping = new ImportFieldMapping(selectedField, header);
                Mappings.Add(mapping);
                SelectedHeader = null;
                SelectedField = null;
                NotifyPropertyChanged("MappingsConfigured");
            }
        }

        public void RemoveMappingCommandAction(object parameter)
        {
            ImportFieldMapping mapping = parameter as ImportFieldMapping;
            Mappings.Remove(mapping);
            NotifyPropertyChanged("MappingsConfigured");
        }

        public void BackCommandAction(Object parameter)
        {
            App.Controller.ClosePopup(null);
        }

        public void AcceptCommandAction(Object parameter)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("CSVImporter", _csvImporter);
            parameters.Add("Mappings", Mappings.ToList());
            App.Controller.ClosePopup(parameters);
        }

        #endregion

    }

}

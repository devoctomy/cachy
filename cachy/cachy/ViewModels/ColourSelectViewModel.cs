using cachy.Controls;
using cachy.Fonts;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class ColourSelectViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private ColourSelectView _view;
        private CachyFont.Glyph _glyph = CachyFont.Glyph.Apple;
        private ColourPickerItem _selectedColourPickerItem;
        private Credential _credential;
        private CreateCredentialViewModel.EditorMode _credentialEditorMode;

        private ICommand _colourSelectedCommand;
        private ICommand _backCommand;
        private ICommand _acceptCommand;

        #endregion

        #region public properties

        public ColourSelectView View
        {
            get
            {
                return (_view);
            }
        }

        public CachyFont.Glyph Glyph
        {
            get
            {
                return (_glyph);
            }
            set
            {
                if(_glyph != value)
                {
                    _glyph = value;
                    NotifyPropertyChanged("Glyph");
                }
            }
        }

        public ColourPickerItem SelectedColourPickerItem
        {
            get
            {
                return (_selectedColourPickerItem);
            }
            set
            {
                if(_selectedColourPickerItem != value)
                {
                    _selectedColourPickerItem = value;
                    NotifyPropertyChanged("SelectedColourPickerItem");
                }
            }
        }

        public Credential Credential
        {
            get
            {
                return (_credential);
            }
        }

        public ICommand ColourSelectedCommand
        {
            get
            {
                return (_colourSelectedCommand);
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

        public ColourSelectViewModel(ColourSelectView view)
        {
            _view = view;

            _colourSelectedCommand = new Command(new Action<Object>(ColourSelectedCommandAction));
            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acceptCommand = new Command(new Action<Object>(AcceptCommandAction));
        }

        #endregion

        #region ipagenavigationaware

        public void SetParameter(String key, Object parameter)
        {
            switch (key)
            {
                case "Credential":
                    {
                        _credential = (Credential)parameter;
                        break;
                    }
                case "Mode":
                    {
                        _credentialEditorMode = (CreateCredentialViewModel.EditorMode)parameter;
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

        public void ColourSelectedCommandAction(Object parameter)
        {
            SelectedColourPickerItem = parameter as ColourPickerItem;
        }

        public void BackCommandAction(Object parameter)
        {
            App.Controller.ClosePopup(null);
        }

        public void AcceptCommandAction(Object parameter)
        {
            App.Controller.ClosePopup(SelectedColourPickerItem.Name);
        }

        #endregion

    }

}

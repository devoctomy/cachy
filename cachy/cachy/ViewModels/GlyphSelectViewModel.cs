using cachy.Converters;
using cachy.Fonts;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static cachy.Fonts.CachyFont;

namespace cachy.ViewModels
{

    public class GlyphSelectViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private GlyphSelectView _view;
        private Credential _credential;
        private Glyph _selectedGlyph;
        private String _toggleGroup = Guid.NewGuid().ToString();
        private ObservableCollection<GlyphSelectViewItem> _items;
        private CreateCredentialViewModel.EditorMode _credentialEditorMode;

        private ICommand _backCommand;
        private ICommand _acceptCommand;
        private ICommand _glyphSelectCommand;

        #endregion

        #region public properties

        public GlyphSelectView View
        {
            get
            {
                return (_view);
            }
        }

        public Credential Credential
        {
            get
            {
                return (_credential);
            }
        }

        public String ToggleGroup
        {
            get
            {
                return (_toggleGroup);
            }
        }

        public Glyph SelectedGlyph
        {
            get
            {
                return (_selectedGlyph);
            }
            set
            {
                if(_selectedGlyph != value)
                {
                    _selectedGlyph = value;
                    NotifyPropertyChanged("SelectedGlyph");
                }
            }
        }

        public ReadOnlyObservableCollection<GlyphSelectViewItem> Items
        {
            get
            {
                return (_items == null ? null : new ReadOnlyObservableCollection<GlyphSelectViewItem>(_items));
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

        public ICommand GlyphSelectCommand
        {
            get
            {
                return (_glyphSelectCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public GlyphSelectViewModel(GlyphSelectView view)
        {
            _view = view;

            EnumerateGlyphs();
            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acceptCommand = new Command(new Action<Object>(AcceptCommandAction));
            _glyphSelectCommand = new Command(new Action<Object>(GlyphSelectCommandAction));
        }

        #endregion

        #region private methods

        private void EnumerateGlyphs()
        {
            _items = new ObservableCollection<GlyphSelectViewItem>();
            NotifyPropertyChanged("Items");
            String[] glyphs = Enum.GetNames(typeof(CachyFont.Glyph));
            foreach (String curGlyph in glyphs)
            {
                CachyFont.Glyph glyph = (CachyFont.Glyph)Enum.Parse(typeof(CachyFont.Glyph), curGlyph);
                _items.Add(new GlyphSelectViewItem(glyph, glyph == SelectedGlyph));
            }
        }

        private void SelectGlyph(Glyph glyph)
        {
            IEnumerable<GlyphSelectViewItem> matchingItems = _items.Where(item => item.Glyph == glyph);
            if (matchingItems.Any())
            {
                GlyphSelectViewItem matched = matchingItems.First();
                matched.Selected = true;
                SelectedGlyph = matched.Glyph;
            }
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
                        GlyphKeyConverter converter = new GlyphKeyConverter();
                        Glyph glyph = (Glyph)converter.Convert(Credential.GlyphKey, typeof(Glyph), null, null);
                        SelectGlyph(glyph);
                        break;
                    }
                case "SelectedGlyph":
                    {
                        SelectGlyph((Glyph)parameter);
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

        public void BackCommandAction(Object parameter)
        {
            App.Controller.ClosePopup(null);
        }

        public void AcceptCommandAction(Object parameter)
        {
            App.Controller.ClosePopup(SelectedGlyph.ToString());
        }

        public void GlyphSelectCommandAction(Object parameter)
        {
            GlyphSelectViewItem selectedGlyph = (GlyphSelectViewItem)parameter;
            SelectedGlyph = selectedGlyph.Glyph;
        }

        #endregion

    }

}

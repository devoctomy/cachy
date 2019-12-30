using cachy.Fonts;
using System;
using System.ComponentModel;

namespace cachy.Views
{

    public class GlyphSelectViewItem : INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private CachyFont.Glyph _glyph;
        private Boolean _selected;

        #endregion

        #region public properties

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

        public Boolean Selected
        {
            get
            {
                return (_selected);
            }
            set
            {
                if(_selected != value)
                {
                    _selected = value;
                    NotifyPropertyChanged("Selected");
                }
            }
        }

        #endregion

        #region constructor / destructor

        public GlyphSelectViewItem(CachyFont.Glyph glyph,
            Boolean selected)
        {
            _glyph = glyph;
            _selected = selected;
        }

        #endregion

        #region private methods

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

}

using cachy.Fonts;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class GlyphKeyConverter : IValueConverter
    {

        #region ivalueconverter

        public Object Convert(Object value,
            Type targetType,
            Object parameter,
            CultureInfo culture)
        {
            String glyphKey = (String)value;
            CachyFont.Glyph glyph = (CachyFont.Glyph)Enum.Parse(typeof(CachyFont.Glyph), glyphKey, true);
            String param = parameter as String;
            if(param == "Unicode")
            {
                return(CachyFont.GetString(glyph));
            }
            else
            {
                return (glyph);
            }
        }

        public Object ConvertBack(Object value,
            Type targetType,
            Object parameter,
            CultureInfo culture)
        {
            CachyFont.Glyph glyph = (CachyFont.Glyph)value;
            return (glyph.ToString());
        }

        #endregion

    }

}


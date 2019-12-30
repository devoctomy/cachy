using cachy.Fonts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class ColourConverter : IValueConverter
    {

        #region ivalueconverter

        public Object Convert(Object value,
            Type targetType,
            Object parameter,
            CultureInfo culture)
        {
            String colourName = (String)value;
            if(!String.IsNullOrEmpty(colourName))
            {
                return (ColourFromName(colourName));
            }
            else
            {
                return (Color.Black);
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

        #region private methods

        private Color ColourFromName(string name)
        {
            FieldInfo[] namedColours = typeof(Color).GetFields(BindingFlags.Static | BindingFlags.Public);
            IEnumerable<FieldInfo> matches = namedColours.Where(fi => fi.Name == name);
            if(matches.Any())
            {
                FieldInfo match = matches.First();
                Color curColour = (Color)match.GetValue(null);
                return (curColour);
            }
            else
            {
                return (Color.Black);
            }
        }

        #endregion

    }

}


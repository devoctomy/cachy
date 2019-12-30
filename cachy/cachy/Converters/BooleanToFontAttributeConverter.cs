using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class BooleanToFontAttributeConverter : IValueConverter
    {

        #region public properties

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FontAttributes attribute = (FontAttributes)Enum.Parse(typeof(FontAttributes), (string)parameter, true);
            return ((bool)value ? attribute : FontAttributes.None);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

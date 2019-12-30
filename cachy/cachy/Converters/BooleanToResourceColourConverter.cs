using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class BooleanToResourceColourConverter : IValueConverter
    {

        #region public methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return (false);

            string[] choices = ((string)parameter).Split(',');
            object resource = ((App)App.Current).Resources[choices[(bool)value ? 0 : 1]];
            return (resource);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

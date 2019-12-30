using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class ObjectToStringConverter : IValueConverter
    {

        #region ivalueconverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                return (value.ToString());
            }
            else
            {
                return (String.Empty);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

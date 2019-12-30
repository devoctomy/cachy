using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class StringComparisonConverter : IValueConverter
    {

        #region public methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value as string;
            string stringParameter = parameter as string;
            return (stringValue == stringParameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

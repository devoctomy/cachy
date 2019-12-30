using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class BooleanInverterConverter : IValueConverter
    {

        #region public methods

        public object Convert(object value, 
            Type targetType, 
            object parameter, 
            CultureInfo culture)
        {
            if (value == null || !(value is bool)) throw new ArgumentException("Value must be of type System.Boolean", "value");
            if (targetType != typeof(bool)) throw new ArgumentException("Target type must be of type System.Boolean", "targetType");

            bool boolValue = (bool)value;
            return (!boolValue);
        }

        public object ConvertBack(object value, 
            Type targetType,
            object parameter, 
            CultureInfo culture)
        {
            if (value == null || !(value is bool)) throw new ArgumentException("Value must be of type System.Boolean", "value");
            if (targetType != typeof(bool)) throw new ArgumentException("Target type must be of type System.Boolean", "targetType");

            bool boolValue = (bool)value;
            return (!boolValue);
        }

        #endregion

    }

}

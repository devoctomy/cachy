using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class BooleanToDoubleConverter : IValueConverter
    {

        #region ivalueconverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            string arguments = parameter as string;
            if(targetType == typeof(double) && !String.IsNullOrEmpty(arguments))
            {
                string[] args = arguments.Split(',');
                return (boolValue ? double.Parse(args[0]) : double.Parse(args[1]));
            }
            else
            {
                throw new ArgumentException("Invalid arguments have been passed for conversion.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

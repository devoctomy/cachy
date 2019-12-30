using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class BooleanToGridLengthConverter : IValueConverter
    {

        #region ivalueconverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            string arguments = parameter as string;
            if (targetType == typeof(GridLength) && !String.IsNullOrEmpty(arguments))
            {
                string[] args = arguments.Split(',');
                return (boolValue ? new GridLength(double.Parse(args[0]), GridUnitType.Absolute) : new GridLength(double.Parse(args[1]), GridUnitType.Absolute));
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

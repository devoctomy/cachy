using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class StringToGridLengthConverter : IValueConverter
    {

        #region ivalueconverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string[] widths = ((string)parameter).Split(',');
            string valueString = (string)value;
            if(String.IsNullOrEmpty(valueString))
            {
                if (widths[0] == "*")
                {
                    GridLength length = new GridLength(1, GridUnitType.Star);
                    return (length);
                }
                else
                {
                    GridLength length = new GridLength(Double.Parse(widths[0]), GridUnitType.Absolute);
                    return (length);
                }
            }
            else
            {
                if (widths[1] == "*")
                {
                    GridLength length = new GridLength(1, GridUnitType.Star);
                    return (length);
                }
                else
                {
                    GridLength length = new GridLength(Double.Parse(widths[1]), GridUnitType.Absolute);
                    return (length);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

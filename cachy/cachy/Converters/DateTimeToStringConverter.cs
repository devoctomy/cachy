using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class DateTimeToStringConverter : IValueConverter
    {

        #region public methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? valueDateTime = (DateTime?)value;
            if (valueDateTime.HasValue)
            {
                if (parameter == null)
                {
                    return (valueDateTime.Value.ToLocalTime().ToString());
                }
                else
                {
                    string format = (string)parameter;
                    return (valueDateTime.Value.ToLocalTime().ToString(format));
                }
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

using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class DateTimeToDaysPastStringConverter : IValueConverter
    {

        #region public methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime since = (DateTime)value;
            TimeSpan elapsed = DateTime.UtcNow - since;
            double daysPassed = Math.Round(elapsed.TotalDays, 1);
            return (String.Format((string)parameter, daysPassed));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

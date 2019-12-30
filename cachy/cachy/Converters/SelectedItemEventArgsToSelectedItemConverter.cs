using System;
using System.Globalization;
using Xamarin.Forms;
using static cachy.Fonts.CachyFont;

namespace cachy.Converters
{

    public class SelectedItemEventArgsToSelectedItemConverter : IValueConverter
    {

        #region ivalueconverter

        public Object Convert(Object value,
            Type targetType,
            Object parameter,
            CultureInfo culture)
        {
            SelectedItemChangedEventArgs eventArgs = value as SelectedItemChangedEventArgs;
            return (eventArgs.SelectedItem);
        }

        public Object ConvertBack(Object value,
            Type targetType,
            Object parameter,
            CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}


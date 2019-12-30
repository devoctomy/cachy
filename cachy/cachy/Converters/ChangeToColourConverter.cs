using cachy.Config;
using devoctomy.cachy.Build.Config;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class ChangeToColourConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Change.ItemChangeType changeType = (Change.ItemChangeType)value;
            switch (changeType)
            {
                case Change.ItemChangeType.Added:
                    {
                        return (Color.Green);
                    }
                case Change.ItemChangeType.Changed:
                    {
                        return (Color.Gold);
                    }
                case Change.ItemChangeType.Fixed:
                    {
                        return (Color.Purple);
                    }
                case Change.ItemChangeType.Removed:
                    {
                        return (Color.Blue);
                    }
                default:
                    {
                        return (Color.White);
                    }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}

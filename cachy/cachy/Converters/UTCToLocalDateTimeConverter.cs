using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class UTCToLocalDateTimeConverter : IValueConverter
    {

        #region ivalueconverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime valueDateTime = (DateTime)value;
            switch(valueDateTime.Kind)
            {
                case DateTimeKind.Utc:
                    {
                        return(valueDateTime.ToLocalTime());
                    }
                case DateTimeKind.Local:
                    {
                        return (valueDateTime);
                    }
                case DateTimeKind.Unspecified:
                    {
                        throw new Exception("Cannot convert from 'Unspecified' DateTime kind.");
                    }
                default:
                    {
                        throw new Exception(String.Format("Cannot convert from '{0}' DateTime kind.", valueDateTime.Kind));
                    }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime valueDateTime = (DateTime)value;
            switch (valueDateTime.Kind)
            {
                case DateTimeKind.Utc:
                    {
                        return (valueDateTime);
                    }
                case DateTimeKind.Local:
                    {
                        return (valueDateTime.ToUniversalTime());
                    }
                case DateTimeKind.Unspecified:
                    {
                        throw new Exception("Cannot convert from 'Unspecified' DateTime kind.");
                    }
                default:
                    {
                        throw new Exception(String.Format("Cannot convert from '{0}' DateTime kind.", valueDateTime.Kind));
                    }
            }
        }

        #endregion

    }

}

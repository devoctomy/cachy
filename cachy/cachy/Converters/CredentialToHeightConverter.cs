using devoctomy.cachy.Framework.Data;
using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class CredentialToHeightConverter : IValueConverter
    {

        #region public methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Credential credential = value as Credential;
            if(credential != null)
            {
                string[] param = ((string)parameter).Split(',');
                double[] heights = param.Select(s => double.Parse(s)).ToArray();
                double height = heights[0];
                if (credential.HasTags) height += heights[1];
                if (credential.IsSelectedWithClipboardFields) height += heights[2];
                return (height);
            }
            else
            {
                return (0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}

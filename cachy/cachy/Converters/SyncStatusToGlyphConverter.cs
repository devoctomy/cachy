using cachy.Fonts;
using devoctomy.cachy.Framework.Data.Cloud;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace cachy.Converters
{

    public class SyncStatusToGlyphConverter : IValueConverter
    {

        #region public methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CloudProviderSyncStatus.SyncStatus syncStats = (CloudProviderSyncStatus.SyncStatus)value;
            switch(syncStats)
            {
                case CloudProviderSyncStatus.SyncStatus.Unchecked:
                    {
                        return (CachyFont.CACHYFONT_GLYPH_CLOUD);
                    }
                case CloudProviderSyncStatus.SyncStatus.NoLocalCopyExists:
                    {
                        return (CachyFont.CACHYFONT_GLYPH_CLOUD_DOWNLOAD);
                    }
                case CloudProviderSyncStatus.SyncStatus.NoCloudCopyExists:
                    {
                        return (CachyFont.CACHYFONT_GLYPH_CLOUD_UPLOAD);
                    }
                case CloudProviderSyncStatus.SyncStatus.LocalCopyNewer:
                    {
                        return (CachyFont.CACHYFONT_GLYPH_CLOUD_UPLOAD);
                    }
                case CloudProviderSyncStatus.SyncStatus.CloudCopyNewer:
                    {
                        return (CachyFont.CACHYFONT_GLYPH_CLOUD_DOWNLOAD);
                    }
                case CloudProviderSyncStatus.SyncStatus.Conflict:
                    {
                        return (CachyFont.CACHYFONT_GLYPH_WARNING_MESSAGE);
                    }
                case CloudProviderSyncStatus.SyncStatus.AuthenticationError:
                    {
                        return (CachyFont.CACHYFONT_GLYPH_WARNING_MESSAGE);
                    }
                case CloudProviderSyncStatus.SyncStatus.UnknownError:
                    {
                        return (CachyFont.CACHYFONT_GLYPH_WARNING_MESSAGE);
                    }
                case CloudProviderSyncStatus.SyncStatus.UpToDate:
                    {
                        return (CachyFont.CACHYFONT_GLYPH_CLOUD_PRIVATE_02);
                    }
                default:
                    {
                        return (CachyFont.CACHYFONT_GLYPH_CLOUD);
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

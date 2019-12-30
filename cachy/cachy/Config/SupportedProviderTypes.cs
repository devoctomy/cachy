using cachy.Fonts;
using System.Collections.ObjectModel;

namespace cachy.Config
{

    public static class SupportedProviderTypes
    {

        #region private objects

        private static ObservableCollection<ProviderType> _supportedTypes;

        #endregion

        #region public properties

        public static ObservableCollection<ProviderType> SupportedTypes
        {
            get
            {
                if(_supportedTypes == null)
                {
                    Initialise();
                }
                return (_supportedTypes);
            }
        }

        #endregion

        #region private methods

        private static void Initialise()
        {
            _supportedTypes = new ObservableCollection<ProviderType>();
            _supportedTypes.Add(new ProviderType(ProviderType.AuthenticationType.OAuth, CachyFont.GetString(CachyFont.Glyph.Drop_Box), "Dropbox", "https://www.dropbox.com"));
            _supportedTypes.Add(new ProviderType(ProviderType.AuthenticationType.OAuth, CachyFont.GetString(CachyFont.Glyph.Onedrive), "OneDrive", "https://onedrive.live.com"));
            _supportedTypes.Add(new ProviderType(ProviderType.AuthenticationType.Amazon, CachyFont.GetString(CachyFont.Glyph.Amazon_01), "AmazonS3", "https://aws.amazon.com/s3/"));
        }

        #endregion

    }

}

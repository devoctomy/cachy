using Dropbox.Api.Users;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class AmazonS3StorageProviderUser : CloudStorageProviderUserBase
    {

        #region constructor / destructor

        public AmazonS3StorageProviderUser(string name,
            string email,
            string profileImageURL) : base(name, email, profileImageURL)
        {
        }

        #endregion

    }

}

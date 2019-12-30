using Dropbox.Api.Users;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class DropboxStorageProviderUser : CloudStorageProviderUserBase
    {

        #region constructor / destructor

        public DropboxStorageProviderUser(string name,
            string email,
            string profileImageURL) : base(name, email, profileImageURL)
        {
        }

        #endregion

        #region public methods

        public static DropboxStorageProviderUser FromFullAccount(FullAccount fullAccount)
        {
            return (CloudStorageProviderUserBase.Create<DropboxStorageProviderUser>(fullAccount.Name.DisplayName,
                fullAccount.Email,
                fullAccount.ProfilePhotoUrl));
        }

        #endregion

    }

}

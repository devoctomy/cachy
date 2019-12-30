using Newtonsoft.Json.Linq;
using System;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class OneDriveStorageProviderUser : CloudStorageProviderUserBase
    {

        #region constructor / destructor

        public OneDriveStorageProviderUser(string name,
            string email,
            string profileImageURL) : base(name, email, profileImageURL)
        {
        }

        #endregion

        #region public methods

        public static OneDriveStorageProviderUser FromJSON(JObject userJSON)
        {
            string displayName = userJSON["displayName"].Value<string>();
            string userPrincipalName = userJSON["userPrincipalName"].Value<string>();
            return (CloudStorageProviderUserBase.Create<OneDriveStorageProviderUser>(
                displayName,
                userPrincipalName,
                String.Empty));
        }

        #endregion

    }

}

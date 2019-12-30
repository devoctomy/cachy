using devoctomy.DFramework.Logging;
using devoctomy.DFramework.Logging.Interfaces;
using System;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class CloudStorageProviderUserBase
    {

        #region private objects

        private string _name = String.Empty;
        private string _email = String.Empty;
        private string _profileImageURL = String.Empty;

        #endregion

        #region public properties

        public string Name
        {
            get
            {
                return (_name);
            }
        }

        public string Email
        {
            get
            {
                return (_email);
            }
        }

        public string ProfileImageURL
        {
            get
            {
                return (_profileImageURL);
            }
        }

        #endregion

        #region constructor / destructor

        public CloudStorageProviderUserBase(string name,
            string email,
            string profileImageURL)
        {
            _name = name;
            _email = email;
            _profileImageURL = profileImageURL;
        }

        #endregion

        #region public methods

        public static T Create<T>(string name,
            string email,
            string profileImageURL) where T : CloudStorageProviderUserBase
        {
            DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "Creating CloudStorageProviderUserBase instance of type '{0}'.", typeof(T).Name);

            T instance = (T)Activator.CreateInstance(typeof(T), name, email, profileImageURL);
            return (instance);
        }

        #endregion

    }

}

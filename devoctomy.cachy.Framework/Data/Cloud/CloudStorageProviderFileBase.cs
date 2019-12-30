using devoctomy.DFramework.Logging;
using devoctomy.DFramework.Logging.Interfaces;
using System;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class CloudStorageProviderFileBase
    {

        #region private objects

        private string _name = String.Empty;
        private bool _isFolder;
        private string _path = String.Empty;
        private string _hash = String.Empty;
        private DateTime? _lastModified;

        #endregion

        #region public properties

        public string Name
        {
            get
            {
                return (_name);
            }
        }

        public bool IsFolder
        {
            get
            {
                return (_isFolder);
            }
        }

        public string Path
        {
            get
            {
                return (_path);
            }
        }

        public string Hash
        {
            get
            {
                return (_hash);
            }
        }

        public DateTime? LastModified
        {
            get
            {
                return (_lastModified);
            }
        }

        #endregion

        #region constructor / destructor

        public CloudStorageProviderFileBase(string name,
            bool isFolder,
            string path,
            string hash,
            DateTime? lastModified)
        {
            _name = name;
            _isFolder = isFolder;
            _path = path;
            _hash = hash;
            _lastModified = lastModified;
        }

        #endregion

        #region public methods

        public static T Create<T>(string name,
            bool isFolder,
            string path,
            string hash,
            DateTime? lastModified) where T : CloudStorageProviderFileBase
        {
            DLoggerManager.Instance.Logger.Log(LoggerMessageType.VerboseLow | LoggerMessageType.Information, "Creating CloudStorageProviderFileBase instance of type '{0}'.", typeof(T).Name);

            T instance = (T)Activator.CreateInstance(typeof(T), name, isFolder, path, hash, lastModified);
            return (instance);
        }

        #endregion

    }

}

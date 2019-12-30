using System;
using System.Collections.Generic;
using System.Text;

namespace devoctomy.cachy.Framework.Data.Cloud.Exceptions
{

    public class CloudStorageFileNotFoundException : CloudStorageProviderException
    {

        #region private objects

        private string _path = String.Empty;

        #endregion

        #region public properties

        public string Path
        {
            get
            {
                return (_path);
            }
        }

        #endregion

        #region constructor / destructor

        public CloudStorageFileNotFoundException(string path) :
            base(String.Format("The cloud storage file '{0}' was not found", path))
        {
            _path = path;
        }

        #endregion

    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace devoctomy.cachy.Framework.Data.Cloud.Exceptions
{

    public class CloudStorageProviderException : Exception
    {

        #region constructor / destructor

        public CloudStorageProviderException(string message) :
            base(message)
        { }

        public CloudStorageProviderException(
            string message,
            Exception innerException) :
            base(message, innerException)
        { }

        #endregion

    }

}

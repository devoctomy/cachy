using System;
using System.Collections.Generic;
using System.Text;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class CloudProviderResponse<T>
    {

        #region public enums

        public enum Response
        {
            None = 0,
            Success = 1,
            AuthenticationError = 2,
            NotFound = 3,
            UnknownError = 4
        }

        #endregion

        #region private objects

        private Response _response;
        private T _result;
        private Exception _exception;

        #endregion

        #region public properties

        public Response ResponseValue
        {
            get
            {
                return (_response);
            }
        }

        public T Result
        {
            get
            {
                return (_result);
            }
        }

        public Exception Exception
        {
            get
            {
                return (_exception);
            }
        }

        #endregion

        #region constuctor / destructor

        public CloudProviderResponse(T result)
        {
            _response = Response.Success;
            _result = result;
        }

        public CloudProviderResponse(
            Response response,
            T result)
        {
            _response = response;
            _result = result;
        }

        public CloudProviderResponse(
            Response response,
            Exception exception)
        {
            _response = response;
            _exception = exception;
        }

        #endregion

    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace devoctomy.cachy.Framework.Serialisers.JSON.Exceptions
{

    public class MultipleVersionSerialiserMatchesException : Exception
    {

        #region private objects

        private string _version = String.Empty;
        private Type _objectType;

        #endregion

        #region public properties

        public string Version
        {
            get
            {
                return (_version);
            }
        }

        public Type ObjectType
        {
            get
            {
                return (_objectType);
            }
        }

        #endregion

        #region constructor / destructor

        public MultipleVersionSerialiserMatchesException(
            string version,
            Type objectType) :
            base(String.Format("Multiple matches for version '{0}' of the type serialiser '{1}' found.", version, objectType.Name))
        {
            _version = version;
            _objectType = objectType;
        }

        #endregion

    }

}

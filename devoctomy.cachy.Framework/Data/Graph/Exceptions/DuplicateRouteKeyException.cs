using System;
using System.Collections.Generic;
using System.Text;

namespace devoctomy.cachy.Framework.Data.Graph.Exceptions
{

    public class DuplicateRouteKeyException : Exception
    {

        #region public properties

        public String Key { get; private set; }

        #endregion

        #region constructor / destructor

        public DuplicateRouteKeyException(String key) :
            base(String.Format("A route with the key '{0}' already exists in the graph.", key))
        {
            Key = key;
        }

        #endregion

    }

}

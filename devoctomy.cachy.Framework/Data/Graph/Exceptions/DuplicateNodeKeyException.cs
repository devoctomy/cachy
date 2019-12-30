using System;

namespace devoctomy.cachy.Framework.Data.Graph.Exceptions
{

    public class DuplicateNodeKeyException : Exception
    {

        #region public properties

        public String Key { get; private set; }

        #endregion

        #region constructor / destructor

        public DuplicateNodeKeyException(String key) :
            base(String.Format("A node with the key '{0}' already exists in the graph.", key))
        {
            Key = key;
        }

        #endregion

    }

}

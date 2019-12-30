using System;

namespace devoctomy.cachy.Framework.Data.Graph
{

    public interface IGraphIO
    {

        #region properties

        Type ValueType { get; }

        Object Value { get; }

        #endregion

        #region methods

        void SetValue(Object value);

        #endregion

    }

}

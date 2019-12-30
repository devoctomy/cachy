using System;
using System.Collections.Generic;

namespace devoctomy.cachy.Framework.Serialisers.Interfaces
{

    public interface ISerialiser
    {

        #region methods

        object Read(
            object data,
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters);

        object Write(
            object data,
            string masterPassphrase,
            params KeyValuePair<string, string>[] parameters);

        #endregion

    }

}

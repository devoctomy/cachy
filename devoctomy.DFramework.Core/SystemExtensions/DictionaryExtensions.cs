using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace devoctomy.DFramework.Core.SystemExtensions
{

    public static class DictionaryExtensions
    {

        #region public methods

        public static Int32 CompareTo(
            this IDictionary<String, String> dict1, IDictionary<String, String> dict2)
        {
            if (dict1 == dict2)
                return(0);

            if (dict1.Count != dict2.Count)
                return (dict1.Count.CompareTo(dict2.Count));

            foreach(String curKey in dict1.Keys)
            {
                String value2;
                if (dict2.TryGetValue(curKey, out value2))
                {
                    Int32 valueCompare = dict1[curKey].CompareTo(value2);
                    if (valueCompare != 0) return (valueCompare);
                }
            }

            return (0);
        }

        #endregion

    }

}

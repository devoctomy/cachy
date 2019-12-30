using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cachywebfunctions.Extensions
{

    public static class ByteExtensions
    {

        #region public methods

        public static string ToHexString(this byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        #endregion

    }

}

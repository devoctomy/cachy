using System;
using System.Collections.Generic;
using System.Text;

namespace devoctomy.cachy.Framework.Native.IO
{

    public static class Common
    {

        #region enums

        public enum HashStyle
        {
            None = 0,
            DropboxSHA256 = 1,
            OneDriveSHA1 = 2,
            AmazonS3MD5 = 3
        }

        #endregion

    }

}

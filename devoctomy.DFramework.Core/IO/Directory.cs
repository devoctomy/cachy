using System;

namespace devoctomy.DFramework.Core.IO
{

    public static class Directory
    {

        #region public delegates definitions

        public delegate Boolean ResolvePathDelegate(String token, 
            out String resolvedPath);

        #endregion

        #region public delegate references

        public static ResolvePathDelegate ResolvePath;

        #endregion

    }

}

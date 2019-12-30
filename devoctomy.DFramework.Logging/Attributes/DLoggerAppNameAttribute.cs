using System;

namespace devoctomy.DFramework.Logging.Attributes
{

    [AttributeUsage(AttributeTargets.Assembly)]
    public class DLoggerAppNameAttribute : Attribute
    {

        #region private objects

        private String _appName = String.Empty;

        #endregion

        #region public properties

        public String AppName
        {
            get
            {
                return (_appName);
            }
        }

        #endregion

        #region constructor / destructor

        public DLoggerAppNameAttribute(String appName)
        {
            _appName = appName;
        }

        #endregion

    }

}

using devoctomy.DFramework.Logging.Attributes;
using devoctomy.DFramework.Logging.Interfaces;
using System;
using System.Reflection;

namespace devoctomy.DFramework.Logging
{
    public class DLoggerManager
    {

        #region private objects

        private static DLoggerManager _defaultInstance;
        private DLoggerConfig _config;
        private IDLogger _defaultLogger;
        private static Boolean _forceAllToDummy;
        private static String _pathDelimiter = "/";

        #endregion

        #region public properties

        public static DLoggerManager Instance
        {
            get
            {
                if (_defaultInstance == null)
                {
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly != null)
                    {
                        DLoggerAppNameAttribute appName = entryAssembly.GetCustomAttribute<DLoggerAppNameAttribute>();
                        if (appName != null)
                        {
                            _defaultInstance = new DLoggerManager(appName.AppName);
                        }
                        else
                        {
                            throw new InvalidOperationException("Entry assembly must define DLoggerAppName attribute.");
                        }
                    }
                    else
                    {
                        _defaultInstance = new DLoggerManager("cachy"); //!!!
                    }
                }
                return (_defaultInstance);
            }
        }

        public DLoggerConfig Config
        {
            get
            {
                return (_config);
            }
        }

        public IDLogger Logger
        {
            get
            {
                return (_defaultLogger);
            }
        }

        public static Boolean ForceAllToDummy
        {
            get
            {
                return (_forceAllToDummy);
            }
            set
            {
                _forceAllToDummy = value;
            }
        }

        public static String PathDelimiter
        {
            get
            {
                return (_pathDelimiter);
            }
            set
            {
                _pathDelimiter = value;
            }
        }

        #endregion

        #region constructor / destructor

        private DLoggerManager(String appName) :
            this(appName, false)
        {     
        }

        private DLoggerManager(String appName, 
            Boolean forceDummy)
        {
            if(ForceAllToDummy || forceDummy)
            {
                _defaultLogger = new DummyDLogger();
            }
            else
            {
                if (DLoggerConfig.LoadDefault(appName, out _config))
                {
                    InitialiseDefaultLogger();
                    if (_config.DefaultEnabled)
                    {
                        _defaultLogger = new DLogger(_config);
                    }
                    else
                    {
                        _defaultLogger = new DummyDLogger();
                    }
                }
                else
                {
                    _defaultLogger = new DummyDLogger();
                }
            }
        }

        #endregion

        #region public methods

        public void InitialiseDefaultLogger()
        {
            if (_config.DefaultEnabled)
            {
                _defaultLogger = new DLogger(_config);
            }
            else
            {
                _defaultLogger = new DummyDLogger();
            }
        }

        public static DLoggerManager DefaultInstance(String appName)
        {
            if (_defaultInstance == null)
            {
                _defaultInstance = new DLoggerManager(appName);
            }
            return (_defaultInstance);
        }

        #endregion

    }

}
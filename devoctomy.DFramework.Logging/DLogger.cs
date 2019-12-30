using devoctomy.DFramework.Logging.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace devoctomy.DFramework.Logging
{

    public class DLogger : IDLogger
    {

        #region public events

        public event EventHandler<LogWritingEventArgs> Writing;

        #endregion

        #region private objects

        private Object _lock;
        private DLoggerConfig _config;
        private Boolean _isDefault;
        private String _fullPath = String.Empty;
        private String _name = String.Empty;
        private LoggerMessageType _allowedMessgeTypes = LoggerMessageType.None;
        private List<String> _allowedMessageTypesList;

        #endregion

        #region public properties

        public Boolean IsDefault
        {
            get
            {
                return (_isDefault);
            }
        }

        public String FullPath
        {
            get
            {
                return (_fullPath);
            }
        }

        public String Name
        {
            get
            {
                return (_name);
            }
        }

        public LoggerMessageType AllowedMessageTypes
        {
            get
            {
#if DEBUG
                //Only allow sensitive logs when a debug build
                return (_allowedMessgeTypes | LoggerMessageType.Sensitive);
#else
                return (_allowedMessgeTypes);
#endif
            }
        }

#endregion

        #region constructor / destructor

        public DLogger(DLoggerConfig config)
        {
            _lock = new Object();
            _config = config;
            _isDefault = true;
            _name = config.DefaultLoggerName;
            _allowedMessgeTypes = config.DefaultMessageTypes;

            String outputPath = DLoggerConfig.ParsePath(config.PathOutput);
            if (!outputPath.EndsWith("/")) outputPath += "/";
            _fullPath = String.Format(@"{0}{1}.log", outputPath, _name);

            LogStartupMessages();
            //Log(LoggerMessageType.Naked,
            //    new String('-', 50) + Environment.NewLine);
            //Log(LoggerMessageType.Information | LoggerMessageType.Success,
            //    "Logger initialised for '{0}', with allowed message types of '{1}'. Output file is located at '{2}'.",
            //    _name,
            //    _allowedMessgeTypes,
            //    _fullPath);
        }

        #endregion

        #region private methods

        private Boolean CheckMessageType(LoggerMessageType type)
        {
            lock (_lock)
            {
                if (_allowedMessageTypesList == null)
                {
                    String[] allowedMessageTypes = AllowedMessageTypes.ToString().Split(',');
                    _allowedMessageTypesList = new List<String>();
                    foreach (String curAllowedType in allowedMessageTypes)
                    {
                        _allowedMessageTypesList.Add(curAllowedType.Trim().ToLower());
                    }
                }
            }

            String[] messageTypes = type.ToString().Split(',');
            foreach (String curType in messageTypes)
            {
                if (!_allowedMessageTypesList.Contains(curType.Trim().ToLower()))
                {
                    return (false);
                }
            }
            return (true);
        }

        #endregion

        #region public methods

        public void LogStartupMessages()
        {
            Log(LoggerMessageType.Naked,
                new String('-', 50) + Environment.NewLine);
            Log(LoggerMessageType.Information | LoggerMessageType.Success,
                "Logger initialised for '{0}', with allowed message types of '{1}'. Output file is located at '{2}'.",
                _name,
                _allowedMessgeTypes,
                _fullPath);
        }

        public void LogEarlyShutdownMessages()
        {
            Log(LoggerMessageType.Information | LoggerMessageType.Success,
                "Logger '{0}', is shutting down early.",
                _name);
        }

        public Boolean Log(LoggerMessageType type,
            String format,
            params System.Object[] args)
        {
            if (!CheckMessageType(type)) return (false);

            LogWritingEventArgs writingArgs = new LogWritingEventArgs(FullPath,
                type,
                String.Format(format, args));

            Writing?.Invoke(this, writingArgs);

            FileInfo outputFile = new FileInfo(FullPath);
            if (!outputFile.Directory.Exists)
            {
                outputFile.Directory.Create();
            }
            LogMessageTemplate messageTemplate = new LogMessageTemplate(_config.MessageTemplateText,
                writingArgs);
            lock(_lock)
            {
                File.AppendAllText(FullPath, messageTemplate.ToString());
            }
            Console.WriteLine(messageTemplate.ToString());

            return (true);
        }

        #endregion

    }


}
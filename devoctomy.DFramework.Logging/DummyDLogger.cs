using devoctomy.DFramework.Logging.Interfaces;
using System;
using System.Collections.Generic;

namespace devoctomy.DFramework.Logging
{

    public class DummyDLogger : IDLogger
    {

        #region public events

        public event EventHandler<LogWritingEventArgs> Writing;

        #endregion

        #region public properties

        public Boolean IsDefault
        {
            get
            {
                return (false);
            }
        }

        public String FullPath
        {
            get
            {
                return (String.Empty);
            }
        }

        public String Name
        {
            get
            {
                return ("DummyLogger");
            }
        }

        public LoggerMessageType AllowedMessageTypes
        {
            get
            {
                return (LoggerMessageType.None);
            }
        }

        #endregion

        #region public methods

        public Boolean Log(LoggerMessageType type,
            String format,
            params System.Object[] args)
        {
            //This is a special flag which prevents sensitive logging information
            //for being output when a debugger is not attached.
            if(type.HasFlag(LoggerMessageType.Sensitive) && !System.Diagnostics.Debugger.IsAttached)
            {
                return (false);
            }
            else
            {
                type = type & ~LoggerMessageType.Sensitive;
            }

            //We need all message types to be in the allowed message
            //types before we can continue
            String[] allowedMessageTypes = AllowedMessageTypes.ToString().Split(',');
            List<String> allowedMessageTypesList = new List<String>();
            foreach (String curAllowedType in allowedMessageTypes)
            {
                allowedMessageTypesList.Add(curAllowedType.Trim().ToLower());
            }
            String[] messageTypes = type.ToString().Split(',');
            foreach (String curType in messageTypes)
            {
                if (!allowedMessageTypesList.Contains(curType.Trim().ToLower()))
                {
                    return (false);
                }
            }

            LogWritingEventArgs writingArgs = new LogWritingEventArgs(FullPath,
                type,
                String.Format(format, args));

            Writing?.Invoke(this, writingArgs);

            System.Diagnostics.Debug.WriteLine(String.Format(format, args));
            return (true);
        }

        public void LogEarlyShutdownMessages()
        {
            Log(LoggerMessageType.Information | LoggerMessageType.Success,
                "Logger '{0}', is shutting down early.",
                Name);
        }

        #endregion

    }

}

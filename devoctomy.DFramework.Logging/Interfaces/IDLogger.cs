using System;

namespace devoctomy.DFramework.Logging.Interfaces
{

    #region enums

    [Flags()]
    public enum LoggerMessageType
    {
        None = 0,
        Information = 1,
        Success = 2,
        Fail = 4,
        Warning = 8,
        Error = 16,
        Exception = 32,
        Timing = 64,
        VerboseLow = 128,
        VerboseMed = 256,
        VerboseHigh = 512,
        Sensitive = 1024,
        Naked = 2048
    }

    #endregion

    public interface IDLogger
    {

        #region events

        event EventHandler<LogWritingEventArgs> Writing;

        #endregion

        #region properties

        Boolean IsDefault { get; }

        String FullPath { get; }

        String Name { get; }

        LoggerMessageType AllowedMessageTypes { get; }

        #endregion

        #region public methods

        Boolean Log(LoggerMessageType type,
            String format,
            params System.Object[] args);

        void LogEarlyShutdownMessages();

        #endregion

    }

}

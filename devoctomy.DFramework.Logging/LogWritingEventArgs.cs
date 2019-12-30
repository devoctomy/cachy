using devoctomy.DFramework.Logging.Interfaces;
using System;

namespace devoctomy.DFramework.Logging
{

    public class LogWritingEventArgs : EventArgs
    {

        #region private objects

        private String _fullPath = String.Empty;
        private DateTime _dateTime = DateTime.Now;
        private LoggerMessageType _messageType = LoggerMessageType.None;
        private String _plainMessage = String.Empty;

        #endregion

        #region public properties

        public String FullPath
        {
            get
            {
                return (_fullPath);
            }
        }

        public DateTime DateTime
        {
            get
            {
                return (_dateTime);
            }
        }

        public LoggerMessageType MessageType
        {
            get
            {
                return (_messageType);
            }
        }

        public String PlainMessage
        {
            get
            {
                return (_plainMessage);
            }
        }

        #endregion

        #region constructor / destructor

        public LogWritingEventArgs(String fullPath,
            LoggerMessageType messageType,
            String plainMessage)
        {
            _fullPath = fullPath;
            _messageType = messageType;
            _plainMessage = plainMessage;
        }

        #endregion

    }

}

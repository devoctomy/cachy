using devoctomy.DFramework.Logging.Interfaces;
using System;
using System.Text;

namespace devoctomy.DFramework.Logging
{

    public class LogMessageTemplate
    {

        #region public enums

        public enum MessagePart
        {
            None = 0,
            Date = 1,
            Time = 2,
            MessageType = 3,
            PlainMessage = 4,
            NewLine = 5
        }

        #endregion

        #region private objects

        private String _messageTemplate = String.Empty;
        private LogWritingEventArgs _writingArgs;

        #endregion

        #region constructor / destructor

        public LogMessageTemplate(String messageTemplate,
            LogWritingEventArgs writingArgs)
        {
            _messageTemplate = messageTemplate;
            _writingArgs = writingArgs;
        }

        #endregion

        #region base class overrides

        public override string ToString()
        {
            StringBuilder templatedMessage = null;

            if (_writingArgs.MessageType != LoggerMessageType.Naked)
            {
                templatedMessage = new StringBuilder(_messageTemplate);
                String[] partNames = Enum.GetNames(typeof(MessagePart));
                foreach (String curPart in partNames)
                {
                    String partValue = String.Empty;
                    MessagePart curMessagePart = (MessagePart)Enum.Parse(typeof(MessagePart), curPart, true);
                    switch (curMessagePart)
                    {
                        case MessagePart.Date:
                            {
                                partValue = _writingArgs.DateTime.ToString("dd-MM-yyyy");
                                break;
                            }
                        case MessagePart.Time:
                            {
                                partValue = _writingArgs.DateTime.ToString("HH:mm:ss");
                                break;
                            }
                        case MessagePart.MessageType:
                            {
                                partValue = _writingArgs.MessageType.ToString();
                                break;
                            }
                        case MessagePart.PlainMessage:
                            {
                                partValue = _writingArgs.PlainMessage;
                                break;
                            }
                        case MessagePart.NewLine:
                            {
                                partValue = Environment.NewLine;
                                break;
                            }
                    }
                    String token = "{" + String.Format("{0}", curPart) + "}";
                    templatedMessage.Replace(token, partValue);
                }
            }
            else
            {
                templatedMessage = new StringBuilder(_writingArgs.PlainMessage);
            }

            return (templatedMessage.ToString());
        }

        #endregion

    }

}

using devoctomy.DFramework.Logging;
using System;

namespace devoctomy.cachy.Framework.Data.Graph
{

    public class GraphIO<T> : IGraphIO
    {

        #region private objects

        private String _name = String.Empty;
        private T _value;

        #endregion

        #region public properties

        public String Name
        {
            get
            {
                return (_name);
            }
        }

        public T Value
        {
            get
            {
                return (_value);
            }
        }

        Type IGraphIO.ValueType
        {
            get
            {
                return (typeof(T));
            }
        }

        Object IGraphIO.Value
        {
            get
            {
                return (_value);
            }
        }

        #endregion

        #region constructor / destructor

        public GraphIO(String name, T value)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating named instance of GraphIO '{0}'.", name);

            _name = name;
            _value = value;
        }

        public GraphIO(T value)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of GraphIO.");

            _value = value;
        }

        #endregion

        #region public methods

        public void SetValue(Object value)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Setting GraphIO Value to '{0}'.", value != null ?value.ToString() : "null");

            if (value == null)
            {
                _value = default(T);
            }
            else
            {
                if (value.GetType() == typeof(T))
                {
                    _value = (T)value;
                }
                else
                {
                    throw new ArgumentException(String.Format("Got type '{0}' but was expecting type '{1}'.", value.GetType().ToString(), typeof(T).ToString()));
                }
            }
        }

        #endregion

    }

}

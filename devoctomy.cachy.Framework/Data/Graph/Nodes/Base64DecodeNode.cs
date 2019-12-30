using devoctomy.DFramework.Logging;
using System;

namespace devoctomy.cachy.Framework.Data.Graph
{

    public class Base64DecodeNode : IGraphNode
    {

        #region events

        public event EventHandler<EventArgs> Processed;

        #endregion

        #region private objects

        private GraphBuilder _parent;
        private GraphIO<String> _encodedData;
        private GraphIO<Byte[]> _unencodedData;
        private Common.NodeState _state = Common.NodeState.None;
        private IGraphNode[] _next;

        #endregion

        #region public properties

        public GraphBuilder Parent
        {
            get
            {
                return (_parent);
            }
            set
            {
                _parent = value;
            }
        }

        public GraphIO<String> EncodedData
        {
            get
            {
                return (_encodedData);
            }
        }

        public GraphIO<Byte[]> UnencodedData
        {
            get
            {
                return (_unencodedData);
            }
        }

        public Common.NodeState State
        {
            get
            {
                return (_state);
            }
        }

        public IGraphNode[] Next
        {
            get
            {
                return (_next);
            }
        }

        #endregion

        #region constructor / destructor

        public Base64DecodeNode(GraphIO<String> encodedData)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of Base64DecodeNode.");
            _encodedData = encodedData;
            _unencodedData = new GraphIO<Byte[]>(null);
        }

        #endregion

        #region public methods

        public Boolean Initialise()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Initialising;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialising Base64DecodeNode.");

                //Clear outputs
                UnencodedData.SetValue(null);

                retVal = true;
            }
            finally
            {
                _state = retVal ? Common.NodeState.Ready : Common.NodeState.Error;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialised Base64DecodeNode.");
            }
            return (retVal);
        }

        public bool Process()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Processing;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Base64 decoding.");
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Input = {0}.", (String)EncodedData.Value);
                _unencodedData.SetValue(Convert.FromBase64String((String)EncodedData.Value));
                retVal = true;
            }
            finally
            {
                _state = Common.NodeState.Processed;
                if (retVal) Processed?.Invoke(this, EventArgs.Empty);
            }
            if (retVal)
            {
                if(_next != null && _next.Length > 0)
                {
                    return (_next[0].Process());
                }
                else
                {
                    return (true);
                }
            }
            else
            {
                return (false);
            }
        }

        public void Connect(params IGraphNode[] nextNodes)
        {
            _next = nextNodes;
        }

        public GraphIO<byte[]> GetBytesIO(string key)
        {
            throw new NotImplementedException();
        }

        public GraphIO<string> GetStringIO(string key)
        {
            throw new NotImplementedException();
        }


        #endregion

    }

}

using devoctomy.DFramework.Core.SystemExtensions;
using devoctomy.DFramework.Logging;
using System;

namespace devoctomy.cachy.Framework.Data.Graph.Nodes
{

    public class Base64EncodeNode : IGraphNode
    {

        #region events

        public event EventHandler<EventArgs> Processed;

        #endregion

        #region private objects

        private GraphBuilder _parent;
        private GraphIO<Byte[]> _unencodedData;
        private GraphIO<String> _encodedData;
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

        public GraphIO<Byte[]> UnencodedData
        {
            get
            {
                return (_unencodedData);
            }
        }

        public GraphIO<String> EncodedData
        {
            get
            {
                return (_encodedData);
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

        public Base64EncodeNode(GraphIO<Byte[]> unencodedData)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of Base64EncodeNode.");
            _unencodedData = unencodedData;
            _encodedData = new GraphIO<String>(String.Empty);
        }

        #endregion

        #region public methods

        public Boolean Initialise()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Initialising;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialising Base64EncodeNode.");

                //Clear outputs
                EncodedData.SetValue(String.Empty);

                retVal = true;
            }
            finally
            {
                _state = retVal ? Common.NodeState.Ready : Common.NodeState.Error;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information, "Initialised Base64EncodeNode.");
            }
            return (retVal);
        }

        public bool Process()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Processing;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Base64 encoding.");
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Input = {0}.", ((Byte[])UnencodedData.Value).ToPrettyString());
                _encodedData.SetValue(Convert.ToBase64String((Byte[])UnencodedData.Value));
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

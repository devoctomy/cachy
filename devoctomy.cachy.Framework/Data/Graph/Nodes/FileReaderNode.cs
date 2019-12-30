using devoctomy.DFramework.Logging;
using System;
using System.IO;

namespace devoctomy.cachy.Framework.Data.Graph.Nodes
{

    public class FileReaderNode : IGraphNode
    {

        #region events

        public event EventHandler<EventArgs> Processed;

        #endregion

        #region private objects

        private GraphBuilder _parent;
        private GraphIO<String> _fullPath;
        private GraphIO<Int32> _bufferSize;
        private GraphIO<String> _stringContents;
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

        public GraphIO<String> FullPath
        {
            get
            {
                return (_fullPath);
            }
        }

        public GraphIO<Int32> BufferSize
        {
            get
            {
                return (_bufferSize);
            }
        }

        public GraphIO<String> StringContents
        {
            get
            {
                return (_stringContents);
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

        public FileReaderNode(GraphBuilder parent,
            GraphIO<String> fullPath,
            GraphIO<Int32> bufferSize)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of FileReaderNode.");
            _parent = parent;
            _fullPath = fullPath;
            _bufferSize = bufferSize;
            _stringContents = new GraphIO<String>(String.Empty);
        }

        #endregion

        #region public methods

        public Boolean Initialise()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Initialising;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialising FileReaderNode.");

                //Clear outputs
                StringContents.SetValue(String.Empty);

                retVal = true;
            }
            finally
            {
                _state = retVal ? Common.NodeState.Ready : Common.NodeState.Error;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialised FileReaderNode.");
            }
            return (retVal);
        }

        public Boolean Process()
        {
            Boolean retVal = false;
            try
            {
                //!!!This may cause issues with UWP
                String stringContents = File.ReadAllText((String)_fullPath.Value);
                _stringContents.SetValue(stringContents);
                retVal = true;
            }
            finally
            {
                if(retVal) Processed?.Invoke(this, EventArgs.Empty);
            }
            if (retVal)
            {
                return (_next[0].Process());
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

using devoctomy.cachy.Framework.Cryptography.KeyDerivation;
using devoctomy.DFramework.Logging;
using System;

namespace devoctomy.cachy.Framework.Data.Graph.Nodes
{

    public class SCryptDeriveBytesNode : IGraphNode
    {

        #region events

        public event EventHandler<EventArgs> Processed;

        #endregion

        #region private objects

        private GraphBuilder _parent;
        private GraphIO<String> _password;
        private GraphIO<Int32> _keyLength;
        private GraphIO<Byte[]> _salt;
        private GraphIO<Int32> _iterations;
        private GraphIO<Int32> _blockSize;
        private GraphIO<Int32> _threadCount;
        private GraphIO<Byte[]> _derivedKey;
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

        public GraphIO<String> Password
        {
            get
            {
                return (_password);
            }
        }

        public GraphIO<Int32> KeyLength
        {
            get
            {
                return (_keyLength);
            }
        }

        public GraphIO<Byte[]> Salt
        {
            get
            {
                return (_salt);
            }
        }

        public GraphIO<Int32> Iterations
        {
            get
            {
                return (_iterations);
            }
        }

        public GraphIO<Int32> BlockSize
        {
            get
            {
                return (_blockSize);
            }
        }

        public GraphIO<Int32> ThreadCount
        {
            get
            {
                return (_threadCount);
            }
        }

        public GraphIO<Byte[]> DervivedKey
        {
            get
            {
                return (_derivedKey);
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

        public SCryptDeriveBytesNode(GraphIO<String> password,
            GraphIO<Int32> keyLength,
            GraphIO<Byte[]> salt,
            GraphIO<Int32> iterations,
            GraphIO<Int32> blockSize,
            GraphIO<Int32> threadCount)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of SCryptDeriveBytesNode.");
            _password = password;
            _keyLength = keyLength;
            _derivedKey = new GraphIO<Byte[]>(null);
            _salt = salt;
            _iterations = iterations;
            _blockSize = blockSize;
            _threadCount = threadCount;
        }

        #endregion

        #region public methods

        public Boolean Initialise()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Initialising;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialising Rfc2898DeriveBytes.");

                //Clear outputs
                DervivedKey.SetValue(null);

                retVal = true;
            }
            finally
            {
                _state = retVal ? Common.NodeState.Ready : Common.NodeState.Error;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialised Rfc2898DeriveBytes.");
            }
            return (retVal);
        }

        public bool Process()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Processing;
                SCrypt scrypt = new SCrypt(Iterations.Value, BlockSize.Value, ThreadCount.Value);
                byte[] saltOut;
                byte[] bytes = scrypt.DeriveBytes((String)_password.Value, _salt.Value, out saltOut);
                _derivedKey.SetValue(bytes);
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
            switch (key)
            {
                case "DerivedKey":
                    {
                        return (DervivedKey);
                    }
                default:
                    {
                        throw new NotSupportedException(String.Format("Cannot get IO key '{0}' from SCryptDeriveBytesNode.", key));
                    }
            }
        }

        public GraphIO<string> GetStringIO(string key)
        {
            switch (key)
            {
                case "Password":
                    {
                        return (Password);
                    }
                default:
                    {
                        throw new NotSupportedException(String.Format("Cannot get IO key '{0}' from SCryptDeriveBytesNode.", key));
                    }
            }
        }

        #endregion

    }

}

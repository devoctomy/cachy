using devoctomy.DFramework.Core.SystemExtensions;
using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Cryptography.Random;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using devoctomy.cachy.Framework.Cryptography.AES;

namespace devoctomy.cachy.Framework.Data.Graph.Nodes
{

    public class AESEncryptNode : IGraphNode
    {

        #region events

        public event EventHandler<EventArgs> Processed;

        #endregion

        #region private objects

        private GraphBuilder _parent;
        private GraphIO<String> _plainText;
        private GraphIO<Byte[]> _iv;
        private GraphIO<Byte[]> _key;
        private GraphIO<Byte[]> _encryptedData;
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

        public GraphIO<String> PlainText
        {
            get
            {
                return (_plainText);
            }
        }

        public GraphIO<Byte[]> IV
        {
            get
            {
                return (_iv);
            }
        }

        public GraphIO<Byte[]> Key
        {
            get
            {
                return (_key);
            }
        }

        public GraphIO<Byte[]> EncryptedData
        {
            get
            {
                return (_encryptedData);
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

        public AESEncryptNode(GraphIO<String> plainText,
            GraphIO<Byte[]> iv,
            GraphIO<Byte[]> key)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of AESEncryptNode.");

            _plainText = plainText;
            _iv = iv;
            _key = key;
            _encryptedData = new GraphIO<Byte[]>(null);
        }

        #endregion

        #region public methods

        public Boolean Initialise()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Initialising;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialising AESEncryptNode.");
                //Clear outputs
                EncryptedData.SetValue(null);

                retVal = true;
            }
            finally
            {
                _state = retVal ? Common.NodeState.Ready : Common.NodeState.Error;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialised AESEncryptNode.");
            }
            return (retVal);
        }

        public Boolean Process()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Processing;

                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Processing AESEncryptNode.");

                Boolean randomIV = false;
                if (IV.Value == null)
                {
                    Byte[] iv = SimpleRandomGenerator.QuickGetRandomBytes(16);
                    IV.SetValue(iv);
                    randomIV = true;
                }

                Byte[] inBlock = UnicodeEncoding.Unicode.GetBytes((String)PlainText.Value);
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating AES encryptor.");
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Key = {0}.", ((Byte[])Key.Value).ToPrettyString());
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "IV = {0}.", ((Byte[])IV.Value).ToPrettyString());
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Input = {0}.", inBlock.ToPrettyString());

                Byte[] outBlock = AESUtility.EncryptBytes(inBlock, (Byte[])Key.Value, (Byte[])IV.Value);

                if (randomIV)
                {
                    using (MemoryStream dataWithIV = new MemoryStream())
                    {
                        dataWithIV.Write(IV.Value, 0, IV.Value.Length);
                        dataWithIV.Write(outBlock, 0, outBlock.Length);
                        dataWithIV.Flush();
                        _encryptedData.SetValue(dataWithIV.ToArray());
                    }
                }
                else
                {
                    _encryptedData.SetValue(outBlock);
                }

                retVal = true;
            }
            finally
            {
                _state = Common.NodeState.Processed;
                if (retVal) Processed?.Invoke(this, EventArgs.Empty);
            }
            if(retVal)
            {
                if (_next != null && _next.Length > 0)
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

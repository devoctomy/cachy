using devoctomy.cachy.Framework.Cryptography.AES;
using devoctomy.DFramework.Core.SystemExtensions;
using devoctomy.DFramework.Logging;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace devoctomy.cachy.Framework.Data.Graph.Nodes
{

    public class AESDecryptNode : IGraphNode
    {

        #region events

        public event EventHandler<EventArgs> Processed;

        #endregion

        #region private objects

        private GraphBuilder _parent;
        private GraphIO<Byte[]> _encryptedData;
        private GraphIO<Byte[]> _iv;
        private GraphIO<Byte[]> _key;
        private GraphIO<String> _unencryptedData;
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

        public GraphIO<Byte[]> EncryptedData
        {
            get
            {
                return (_encryptedData);
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

        public GraphIO<String> UnencryptedData
        {
            get
            {
                return (_unencryptedData);
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

        public AESDecryptNode(GraphIO<Byte[]> encryptedData,
            GraphIO<Byte[]> iv,
            GraphIO<Byte[]> key)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of AESDecryptNode.");

            _encryptedData = encryptedData;
            _iv = iv;
            _key = key;
            _unencryptedData = new GraphIO<String>(String.Empty);
        }

        #endregion

        #region public methods

        public Boolean Initialise()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Initialising;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialising AESDecryptNode.");

                //Clear outputs
                UnencryptedData.SetValue(String.Empty);

                retVal = true;
            }
            finally
            {
                _state = retVal ? Common.NodeState.Ready : Common.NodeState.Error;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Initialised AESDecryptNode.");
            }
            return (retVal);
        }

        public Boolean Process()
        {
            Boolean retVal = false;
            try
            {
                _state = Common.NodeState.Processing;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Processing AESDecryptNode.");

                if (IV.Value == null)
                {
                    //Split IV from the data
                    using (MemoryStream dataWithIV = new MemoryStream(EncryptedData.Value))
                    {
                        Byte[] iv = new Byte[16];
                        dataWithIV.Read(iv, 0, iv.Length);
                        Byte[] dataBlock = new Byte[dataWithIV.Length - iv.Length];
                        dataWithIV.Read(dataBlock, 0, dataBlock.Length);
                        IV.SetValue(iv);
                        EncryptedData.SetValue(dataBlock);
                    }
                }

                Byte[] inBlock = (Byte[])EncryptedData.Value;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating AES decryptor.");
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Key = {0}.", ((Byte[])Key.Value).ToPrettyString());
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "IV = {0}.", ((Byte[])IV.Value).ToPrettyString());
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Input = {0}.", inBlock.ToPrettyString());

                Byte[] outBlock = AESUtility.DecryptBytes(inBlock, (Byte[])Key.Value, (Byte[])IV.Value);
                _unencryptedData.SetValue(UnicodeEncoding.Unicode.GetString(outBlock));
                retVal = true;
            }
            catch(Exception ex)
            {
                _state = Common.NodeState.Error;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.Fail | DFramework.Logging.Interfaces.LoggerMessageType.Exception | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Exception occurred in AESDecryptNode :: {0}", ex.ToString());
                retVal = false;
            }
            finally
            {
                _state = Common.NodeState.Processed;
                DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Processed AESDecryptNode.");
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

using devoctomy.DFramework.Logging;
using System;

namespace devoctomy.cachy.Framework.Data.Graph
{

    public class GraphRoute
    {

        #region private objects

        private String _key = String.Empty;
        private IGraphIO _input;
        private IGraphIO _output;
        private String _startNode;
        private String[] _nodes;

        #endregion

        #region public properties

        public String Key
        {
            get
            {
                return (_key);
            }
        }

        public IGraphIO Input
        {
            get
            {
                return (_input);
            }
        }

        public IGraphIO Output
        {
            get
            {
                return (_output);
            }
        }

        public String StartNode
        {
            get
            {
                return (_startNode);
            }
        }

        public String[] Nodes
        {
            get
            {
                return (_nodes);
            }
        }

        #endregion

        #region constructor / destructor

        public GraphRoute(String key,
            IGraphIO input,
            IGraphIO output,
            String startNode,
            params String[] nodes)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of GraphRoute.");

            _key = key;
            _input = input;
            _output = output;
            _startNode = startNode;
            _nodes = nodes;
        }

        #endregion

    }

}

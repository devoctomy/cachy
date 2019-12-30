using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Data.Graph.Exceptions;
using System;
using System.Collections.Generic;

namespace devoctomy.cachy.Framework.Data.Graph
{

    public class GraphBuilder
    {

        #region private objects

        private String _name = String.Empty;
        private Dictionary<String, IGraphNode> _nodes;
        private Dictionary<String, GraphRoute> _routes;

        #endregion

        #region constructor / destructor

        public GraphBuilder(String name)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating named instance of GraphBuilder '{0}'.", name);

            _name = name;
            _nodes = new Dictionary<String, IGraphNode>();
            _routes = new Dictionary<String, GraphRoute>();
        }

        public GraphBuilder()
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of GraphBuilder.");

            _nodes = new Dictionary<String, IGraphNode>();
            _routes = new Dictionary<String, GraphRoute>();
        }

        #endregion

        #region public properties

        public String Name
        {
            get
            {
                return (_name);
            }
        }

        #endregion

        #region public methods

        public T GetNode<T>(String key)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, String.IsNullOrEmpty(Name) ? String.Format("Getting node '{0}' from GraphBuilder.", key) : String.Format("Getting node '{0}' from named GraphBuilder '{1}'.", key, Name));
            if (_nodes.ContainsKey(key))
            {
                return ((T)_nodes[key]);
            }
            else
            {
                throw new KeyNotFoundException(String.Format("Node with the key '{0}' was not found.", key));
            }
        }

        public void AddNode(String key, IGraphNode node)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, String.IsNullOrEmpty(Name) ? String.Format("Adding node '{0}' to GraphBuilder.", key) : String.Format("Adding node '{0}' to named GraphBuilder '{1}'.", key, Name));
            if (!_nodes.ContainsKey(key))
            {
                node.Parent = this;
                _nodes.Add(key, node);
            }
            else
            {
                throw new DuplicateNodeKeyException(key);
            }
        }

        public Boolean Initialise()
        {
            foreach (IGraphNode curNode in _nodes.Values)
            {
                if (!curNode.Initialise())
                {
                    return (false);
                }
            }
            return (true);
        }

        public Boolean Link(params String[] nodes)
        {
            String links = String.Join(",", nodes);
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, String.IsNullOrEmpty(Name) ? String.Format("Linking GraphBuilder '{0}'.", links) : String.Format("Linking named GraphBuilder '{0}' with '{1}'.", Name, links));
            if (_nodes.Count > 1)
            {
                for (Int32 iNode = 0; iNode < (nodes.Length - 1); iNode++)
                {
                    IGraphNode curNode = _nodes[nodes[iNode]];
                    IGraphNode nextNode = _nodes[nodes[iNode + 1]];
                    curNode.Connect(nextNode);
                }
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public Boolean Process(Boolean initialise,
            String routeKey,
            Object inputValue,
            out Object outputValue)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, String.IsNullOrEmpty(Name) ? String.Format("Process route '{0}' on GraphBuilder.", routeKey) : String.Format("Process route '{0}' on named GraphBuilder '{0}'.", routeKey, Name));
            if (_routes.ContainsKey(routeKey))
            {
                if (initialise) Initialise();
                GraphRoute route = _routes[routeKey];
                Link(route.Nodes);
                outputValue = null;
                if (route.Input != null) route.Input.SetValue(inputValue);
                if (_nodes.ContainsKey(route.StartNode))
                {
                    if (_nodes[route.StartNode].Process())
                    {
                        outputValue = route.Output.Value;
                        return (true);
                    }
                    else
                    {
                        return (false);
                    }
                }
                else
                {
                    throw new KeyNotFoundException(String.Format("Node with the key '{0}' was not found.", route.StartNode));
                }
            }
            else
            {
                throw new KeyNotFoundException(String.Format("Route with the key '{0}' was not found.", routeKey));
            }
        }

        public void CreateRoute(String key,
            IGraphIO input,
            IGraphIO output,
            String startNode,
            params String[] nodes)
        {
            if (!_routes.ContainsKey(key))
            {
                GraphRoute route = new GraphRoute(key,
                    input,
                    output,
                    startNode,
                    nodes);
                _routes.Add(key, route);
            }
            else
            {
                throw new DuplicateRouteKeyException(key);
            }
        }

        public static Boolean ExecuteIsolatedNode(IGraphNode node,
            IGraphIO inputIO,
            IGraphIO outputIO,
            Object inputValue,
            out Object outputValue)
        {
            GraphBuilder graphBuilder = new GraphBuilder();
            graphBuilder.AddNode("isolatedNode", node);
            graphBuilder.CreateRoute("isolated",
                inputIO,
                outputIO,
                "isolatedNode",
                "isolatedNode");
            return (graphBuilder.Process(true, "isolated", inputValue, out outputValue));
        }

        #endregion

    }

}

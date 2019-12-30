using System;

namespace devoctomy.cachy.Framework.Data.Graph
{

    public interface IGraphNode
    {

        #region events

        event EventHandler<EventArgs> Processed;

        #endregion

        #region properties

        GraphBuilder Parent { get; set; }

        Common.NodeState State { get; }

        IGraphNode[] Next { get; }

        #endregion

        #region methods

        Boolean Initialise();

        Boolean Process();

        void Connect(params IGraphNode[] nextNodes);

        GraphIO<byte[]> GetBytesIO(string key);
        GraphIO<string> GetStringIO(string key);

        #endregion

    }

}

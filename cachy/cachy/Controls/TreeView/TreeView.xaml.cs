using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls.TreeView
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TreeView : ContentView
	{

        #region public events

        public event EventHandler<SelectedItemChangedEventArgs> SelectedNodeChanged;

        #endregion

        #region bindable properties

        public static BindableProperty NodesProperty = BindableProperty.Create(
            "Nodes",
            typeof(ObservableCollection<TreeViewNode>),
            typeof(TreeView),
            new ObservableCollection<TreeViewNode>(),
            BindingMode.Default,
            null,
            ItemsSourceChanged);

        #endregion

        #region private objects

        private TreeViewNode _prevSelectedNode;
        private TreeViewNode _selectedNode;

        #endregion

        #region public properties

        public ObservableCollection<TreeViewNode> Nodes
        {
            get
            {
                return ((ObservableCollection<TreeViewNode>)GetValue(NodesProperty));
            }
            set
            {
                SetValue(NodesProperty, value);
            }
        }

        public TreeViewNode SelectedNode
        {
            get
            {
                return (_selectedNode);
            }
            set
            {
                if(_selectedNode != value)
                {
                    _selectedNode = value;
                    StyleSelectedNode();
                    SelectedNodeChanged?.Invoke(this, new SelectedItemChangedEventArgs(_selectedNode, Nodes.IndexOf(_selectedNode)));
                }
            }
        }

        #endregion

        #region constructor / destructor

        public TreeView ()
		{
			InitializeComponent ();

            Nodes.CollectionChanged += Nodes_CollectionChanged;
        }

        #endregion

        #region bindable property changed callbacks

        private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TreeView treeView = bindable as TreeView;
            if (oldValue != null)
            {
                ObservableCollection<TreeViewNode> oldNodes = oldValue as ObservableCollection<TreeViewNode>;
                oldNodes.CollectionChanged -= treeView.Nodes_CollectionChanged;
            }
            if (newValue != null)
            {
                ObservableCollection<TreeViewNode> newNodes = newValue as ObservableCollection<TreeViewNode>;
                newNodes.CollectionChanged += treeView.Nodes_CollectionChanged;
            }
            treeView.BuildTree();
        }

        #endregion

        #region private methods

        private void StyleSelectedNode()
        {
            if(_prevSelectedNode != null)
            {
                //style _prevSelectedNode
            }
            if (_selectedNode != null)
            {
                //style _selectedNode
            }
            _prevSelectedNode = _selectedNode;
        }

        private void BuildTree()
        {
            LayoutRoot.Children.Clear();
            if(Nodes != null)
            {
                foreach (TreeViewNode curNode in Nodes)
                {
                    AttachTreeToNode(curNode);

                    TreeViewNodeView nodeView = new TreeViewNodeView(curNode);
                    nodeView.HeightRequest = 32d;
                    LayoutRoot.Children.Add(nodeView);
                }
            }
        }

        private void AttachTreeToNode(TreeViewNode node)
        {
            node.AttachTreeView(this);
            foreach (TreeViewNode curNode in node.Children)
            {
                AttachTreeToNode(curNode);
            }
        }

        #endregion

        #region object events

        private void Nodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            BuildTree();
        }

        #endregion

    }

}
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy.Controls.TreeView
{

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TreeViewNodeView : ContentView
	{

        #region private objects

        private TreeViewNode _node;

        private ICommand _clickCommand;

        #endregion

        #region public properties

        public TreeViewNode Node
        {
            get
            {
                return (_node);
            }
        }

        public ICommand ClickCommand
        {
            get
            {
                return (_clickCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public TreeViewNodeView (TreeViewNode node)
		{
            _clickCommand = new Command<object>(new Action<object>(ClickCommandAction));

            InitializeComponent();

            _node = node;
            _node.Resize += _node_Resize;
            BindingContext = Node;
            NodeLabelBox.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => {
                    OnLabelClicked();
                }),
                NumberOfTapsRequired = 1
            });
        }

        #endregion

        #region public methods

        public void BuildNode()
        {
            foreach (TreeViewNode curNode in Node.Children)
            {
                TreeViewNodeView nodeView = new TreeViewNodeView(curNode);
                nodeView.HeightRequest = 32d;
                LayoutChildrenStack.Children.Add(nodeView);
            }
            ResizeToFitContents();
        }

        public void ClearNode()
        {
            foreach(TreeViewNodeView curNodeView in LayoutChildrenStack.Children)
            {
                curNodeView.ClearNode();
            }
            LayoutChildrenStack.Children.Clear();
            ResizeToFitContents();
        }

        public double CalculateTotalRequiredHeight()
        {
            if(LayoutChildrenStack.IsVisible && LayoutChildrenStack.Children.Count > 0)
            {
                double spacing = LayoutChildrenStack.Children.Count > 1 ? (LayoutChildrenStack.Children.Count - 1) : 0d;
                double total = 33d;
                foreach (TreeViewNodeView curNodeView in LayoutChildrenStack.Children)
                {
                    total += curNodeView.CalculateTotalRequiredHeight();
                }
                return (total + spacing);
            }
            else
            {
                return (32d);
            }
        }

        public void ResizeToFitContents()
        {
            HeightRequest = CalculateTotalRequiredHeight();
            LayoutChildrenStack.HeightRequest = HeightRequest - 32d;
            foreach (TreeViewNodeView curNodeView in LayoutChildrenStack.Children)
            {
                curNodeView.ResizeToFitContents();
            }
        }

        #endregion

        #region private methods

        private void ClickCommandAction(object parameter)
        {
            if (Node.Children.Count > 0)
            {
                bool visible = !LayoutChildrenStack.IsVisible;
                if (visible && Node.Children.Count > 0)
                {
                    LayoutChildrenStack.IsVisible = true;
                    BuildNode();
                }
                else if (Node.Children.Count > 0)
                {
                    LayoutChildrenStack.IsVisible = false;
                    ClearNode();
                }

                TreeViewNode rootParent = Node.Parent;
                TreeViewNode curParent = Node.Parent;
                while (curParent != null)
                {
                    rootParent = curParent;
                    curParent = curParent.Parent;
                }
                if (rootParent != null)
                {
                    rootParent.RaiseResize();
                }
            }
            else
            {
                Node.Owner.SelectedNode = Node;
            }
        }

        #endregion

        #region object events

        private void OnLabelClicked()
        {
            if (Node.Children.Count > 0)
            {
                Node.IsSelected = true;
                //Node.Owner.SelectedNode = Node;
            }
        }

        private void _node_Resize(object sender, EventArgs e)
        {
            ResizeToFitContents();
        }

        #endregion

    }

}
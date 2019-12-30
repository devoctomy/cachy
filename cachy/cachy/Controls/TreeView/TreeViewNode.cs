using cachy.Fonts;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace cachy.Controls.TreeView
{

    public class TreeViewNode : INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs> Resize;

        #endregion

        #region private objects

        private TreeViewNode _parent;
        private string _name;
        private object _tag;
        private ObservableCollection<TreeViewNode> _children;
        private TreeView _treeView;
        private bool _isSelected;
        private bool _isEmptyFolder;

        #endregion

        #region public properties

        public TreeViewNode Parent
        {
            get
            {
                return (_parent);
            }
        }

        public string Name
        {
            get
            {
                return (_name);
            }
            set
            {
                if(_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public object Tag
        {
            get
            {
                return (_tag);
            }
            set
            {
                if(_tag != value)
                {
                    _tag = value;
                    NotifyPropertyChanged("Tag");
                }
            }
        }

        public CachyFont.Glyph Glyph
        {
            get
            {
                if(Children.Count > 0)
                {
                    return (CachyFont.Glyph.Folder_01);
                }
                else
                {
                    return (CachyFont.Glyph.Locker);
                }
            }
        }

        public ObservableCollection<TreeViewNode> Children
        {
            get
            {
                return (_children);
            }
        }

        public TreeView Owner
        {
            get
            {
                return (_treeView);
            }
        }

        public bool IsSelected
        {
            get
            {
                return (_isSelected);
            }
            set
            {
                if(_isSelected != value)
                {
                    _isSelected = value;
                    if (value)
                    {
                        Owner.SelectedNode = this;
                    }
                    else
                    {
                        Owner.SelectedNode = null;
                    }
                }
            }
        }

        public bool IsEmptyFolder
        {
            get
            {
                return (_isEmptyFolder);
            }
            set
            {
                if(_isEmptyFolder != value)
                {
                    _isEmptyFolder = value;
                    NotifyPropertyChanged("IsEmptyFolder");
                }
            }
        }

        #endregion

        #region constructor / destructor

        public TreeViewNode(
            TreeViewNode parent,
            string name,
            object tag)
        {
            _parent = parent;
            _name = name;
            _tag = tag;
            _children = new ObservableCollection<TreeViewNode>();
        }

        #endregion

        #region private methods

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region public methods

        public void RaiseResize()
        {
            Resize?.Invoke(null, EventArgs.Empty);
        }

        internal void AttachTreeView(TreeView treeView)
        {
            _treeView = treeView;
        }

        #endregion

    }

}

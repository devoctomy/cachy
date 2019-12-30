using cachy.Config;
using cachy.Controls.TreeView;
using cachy.ViewModels.Interfaces;
using cachy.Views;
using devoctomy.cachy.Framework.Data.Cloud;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace cachy.ViewModels
{

    public class CloudProviderFileSelectViewModel : ViewModelBase, IPageNavigationAware
    {

        #region private objects

        private CloudProviderFileSelectView _view;
        private ObservableCollection<TreeViewNode> _nodes;
        private string _providerID = String.Empty;
        private Dictionary<string, TreeViewNode> _nodesByPath = new Dictionary<string, TreeViewNode>();
        private CloudStorageProviderFileBase _selectedFile;

        private ICommand _selectedFileCommand;
        private ICommand _backCommand;
        private ICommand _acceptCommand;

        #endregion

        #region public properties

        public CloudProviderFileSelectView View
        {
            get
            {
                return (_view);
            }
        }

        public ObservableCollection<TreeViewNode> Nodes
        {
            get
            {
                return (_nodes);
            }
            set
            {
                if(_nodes != value)
                {
                    _nodes = value;
                    NotifyPropertyChanged("Nodes");
                    NotifyPropertyChanged("IsEmpty");
                }
            }
        }

        public CloudStorageProviderFileBase SelectedFile
        {
            get
            {
                return (_selectedFile);
            }
            private set
            {
                if(_selectedFile != value)
                {
                    _selectedFile = value;
                    NotifyPropertyChanged("SelectedFile");
                    NotifyPropertyChanged("FileIsSelected");
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (Nodes == null || Nodes.Count == 0);
            }
        }

        public bool FileIsSelected
        {
            get
            {
                return (SelectedFile != null);
            }
        }

        public ICommand SelectedFileCommand
        {
            get
            {
                return (_selectedFileCommand);
            }
        }

        public ICommand BackCommand
        {
            get
            {
                return (_backCommand);
            }
        }

        public ICommand AcceptCommand
        {
            get
            {
                return (_acceptCommand);
            }
        }

        #endregion

        #region constructor / destructor

        public CloudProviderFileSelectViewModel(CloudProviderFileSelectView view)
        {
            _view = view;

            _selectedFileCommand = new Command(new Action<Object>(SelectedFileCommandAction));
            _backCommand = new Command(new Action<Object>(BackCommandAction));
            _acceptCommand = new Command(new Action<Object>(AcceptCommandAction));
        }

        #endregion

        #region private methods

        private void CreateTreeNodes(List<CloudStorageProviderFileBase> files)
        {
            List<TreeViewNode> rootNodes = BuildTreeNodes(files);
            ObservableCollection<TreeViewNode> nodes = new ObservableCollection<TreeViewNode>();
            foreach(TreeViewNode curNode in rootNodes)
            {
                nodes.Add(curNode);
            }
            FindEmptyFolders();
            Nodes = nodes;
        }

        private void FindEmptyFolders()
        {
            foreach(string path in _nodesByPath.Keys)
            {
                TreeViewNode node = _nodesByPath[path];
                CloudStorageProviderFileBase file = (CloudStorageProviderFileBase)node.Tag;
                if(file.IsFolder && node.Children.Count == 0)
                {
                    node.IsEmptyFolder = true;
                }
            }
        }

        private List<TreeViewNode> BuildTreeNodes(List<CloudStorageProviderFileBase> files)
        {
            List<TreeViewNode> rootnodes = new List<TreeViewNode>();
            foreach (CloudStorageProviderFileBase curFile in files)
            {
                string[] nodeParts = curFile.Path.TrimStart('/').Split('/');
                for (int curPart = 0; curPart < nodeParts.Length; curPart++)
                {
                    bool existing;
                    TreeViewNode node = BuildNode(
                        curFile, 
                        nodeParts, 
                        curPart, 
                        out existing);
                    if (!existing && curPart == 0)
                    {
                        rootnodes.Add(node);
                    }
                }
            }
            return (rootnodes);
        }

        private TreeViewNode BuildNode(
            CloudStorageProviderFileBase file,
            string[] nodeParts,
            int curPart,
            out bool existing)
        {
            existing = false;
            TreeViewNode parent = null;
            TreeViewNode curNode = null;

            string curPartKey = String.Join("/", nodeParts, 0, curPart + 1);

            if (!_nodesByPath.ContainsKey(curPartKey))
            {
                //link to the parent
                if (curPart > 0)
                {
                    string curParentPartKey = String.Join("/", nodeParts, 0, curPart);
                    parent = _nodesByPath[curParentPartKey];
                }

                curNode = new TreeViewNode(parent, nodeParts[curPart], file);

                if (parent != null)
                {
                    parent.Children.Add(curNode);
                }

                _nodesByPath.Add(curPartKey, curNode);
            }
            else
            {
                curNode = _nodesByPath[curPartKey];
                existing = true;
            }

            return (curNode);
        }

        private async Task ListFilesFromProvider()
        {
            IEnumerable<CloudProvider> providers = CloudProviders.Instance.Providers.Where(cp => cp.ID == _providerID);
            if(providers.Any())
            {
                CloudStorageProviderBase storageProvider = null;
                CloudProvider provider = providers.First();
                switch(provider.AuthType)
                {
                    case ProviderType.AuthenticationType.OAuth:
                        {
                            string providerKey = (string)((App)App.Current).GetProviderValue(_providerID, "ProviderKey");
                            string accessToken = ((App)App.Current).GetCredential(_providerID);
                            Dictionary<string, string> createParams = new Dictionary<string, string>();
                            createParams.Add("AuthType", "OAuth");
                            createParams.Add("ProviderKey", providerKey);
                            createParams.Add("AccessToken", accessToken);
                            storageProvider = CloudStorageProviderBase.Create(
                                App.AppLogger.Logger,
                                createParams);
                            break;
                        }
                    case ProviderType.AuthenticationType.Amazon:
                        {
                            string providerKey = (string)((App)App.Current).GetProviderValue(_providerID, "ProviderKey");
                            string secret = ((App)App.Current).GetCredential(_providerID);
                            JObject s3ConfigJSON = JObject.Parse(secret);
                            AmazonS3Config s3Config = AmazonS3Config.FromJSON(s3ConfigJSON);
                            Dictionary<string, string> createParams = s3Config.ToDictionary();
                            createParams.Add("ProviderKey", providerKey);
                            storageProvider = CloudStorageProviderBase.Create(
                                App.AppLogger.Logger,
                                createParams);
                            break;
                        }
                    default:
                        {
                            throw new NotSupportedException(String.Format("Cloud provider authentication type '{0}' is not supported.", provider.AuthType));
                        }
                }

                CloudProviderResponse<List<CloudStorageProviderFileBase>> response = await storageProvider.ListFiles();
                if (response.ResponseValue == CloudProviderResponse<List<CloudStorageProviderFileBase>>.Response.Success)
                {
                    CreateTreeNodes(response.Result);
                }
            }

        }

        #endregion

        #region ipagenavigationaware

        public async void SetParameter(String key, Object parameter)
        {
            switch(key)
            {
                case "ProviderID":
                    {
                        _providerID = (string)parameter;
                        if(!String.IsNullOrEmpty(_providerID))
                        {
                            await ListFilesFromProvider();
                        }
                        break;
                    }
                case "Files":
                    {
                        List<CloudStorageProviderFileBase> _files = parameter as List<CloudStorageProviderFileBase>;
                        if(_files != null)
                        {
                            CreateTreeNodes(_files);
                        }
                        break;
                    }
            }
        }

        public void SetParameters(params KeyValuePair<String, Object>[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                foreach (KeyValuePair<String, Object> curParameter in parameters)
                {
                    SetParameter(curParameter.Key,
                        curParameter.Value);
                }
            }
        }

        public void OnClosePopup(View item, object parameter)
        {

        }

        public void OnNavigateTo(View view, object parameter)
        {

        }

        public void OnNavigateFrom(View view, object parameter)
        {

        }

        public void OnGoBack(View view, object parameter)
        {

        }

        #endregion

        #region commands

        public void SelectedFileCommandAction(object parameter)
        {
            TreeViewNode node = parameter as TreeViewNode;
            if(node != null)
            {
                SelectedFile = node.Tag as CloudStorageProviderFileBase;
            }
        }

        public void BackCommandAction(object parameter)
        {
            App.Controller.ClosePopup(null);
        }

        public void AcceptCommandAction(object parameter)
        {
            App.Controller.ClosePopup(SelectedFile);
        }

        #endregion

    }

}

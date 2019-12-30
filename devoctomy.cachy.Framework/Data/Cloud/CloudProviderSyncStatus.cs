using System.ComponentModel;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class CloudProviderSyncStatus : INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region public enums

        public enum SyncStatus
        {
            None = 0,
            Unchecked = 1,
            NoLocalCopyExists = 2,
            NoCloudCopyExists = 3,
            LocalCopyNewer = 4,
            CloudCopyNewer = 5,
            Conflict = 6,
            AuthenticationError = 7,
            UnknownError = 8,
            UpToDate = 9
        }

        #endregion

        #region private objects

        private VaultIndex _vaultIndex;
        private SyncStatus _syncStatus = SyncStatus.Unchecked;
        private string _message = string.Empty;

        #endregion

        #region public properties

        public VaultIndex VaultIndex
        {
            get
            {
                return (_vaultIndex);
            }
        }

        public SyncStatus Status
        {
            get
            {
                return (_syncStatus);
            }
        }

        public string Message
        {
            get
            {
                return (_message);
            }
        }

        #endregion

        #region constructor / destructor

        public CloudProviderSyncStatus(VaultIndex vaultIndex)
        {
            _vaultIndex = vaultIndex;
        }

        #endregion

        #region public methods

        public void SetStatus(SyncStatus status)
        {
            if (_syncStatus != status)
            {
                _syncStatus = status;
                NotifyPropertyChanged("Status");
            }
        }

        public void SetStatus(
            SyncStatus status,
            string message)
        {
            if (_syncStatus != status)
            {
                _syncStatus = status;
                NotifyPropertyChanged("Status");
            }
            if (_message != message)
            {
                _message = message;
                NotifyPropertyChanged("Message");
            }
        }
        
        #endregion

        #region private methods

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

}

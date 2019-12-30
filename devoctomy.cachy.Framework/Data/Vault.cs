using devoctomy.DFramework.Logging;
using devoctomy.cachy.Framework.Serialisers.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace devoctomy.cachy.Framework.Data
{

    public class Vault : IComparable<Vault>, INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private String _id = Guid.NewGuid().ToString();
        private String _name = String.Empty;
        private String _description = String.Empty;
        private DateTime _createdAt = DateTime.UtcNow;
        private DateTime _lastUpdatedAt = DateTime.UtcNow;
        private ObservableCollection<Credential> _credentials;
        private ObservableCollection<AuditLogEntry> _auditLogEntries;

        private Boolean _isDirty;
        private Boolean _isLoaded;

        //These are used for saving without prompting for the
        //passphrase again as we don't want that changing
        private ISerialiser _serialiser;
        private String _fullPath = String.Empty;
        private String _masterPassphrase = String.Empty;

        #endregion

        #region public properties

        public String ID
        {
            get
            {
                return (_id);
            }
        }

        public String Name
        {
            get
            {
                return (_name);
            }
            set
            {
                if(_name != value)
                {
                    String oldName = _name;
                    _name = value;
                    AddAuditEntry(AuditLogEntry.EntryType.ModifyName, new KeyValuePair<String, String>("PrevValue", oldName));
                    NotifyPropertyChanged("Name");
                }
            }
        }

        public String Description
        {
            get
            {
                return (_description);
            }
            set
            {
                if (_description != value)
                {
                    String oldDescription = _description;
                    _description = value;
                    AddAuditEntry(AuditLogEntry.EntryType.ModifyDescription, new KeyValuePair<String, String>("PrevValue", oldDescription));
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                return (_createdAt);
            }
        }

        public DateTime LastUpdatedAt
        {
            get
            {
                return (_lastUpdatedAt);
            }
        }

        public String FullPath
        {
            get
            {
                return (_fullPath);
            }
        }

        public ObservableCollection<Credential> Credentials
        {
            get
            {
                return (new ObservableCollection<Credential>(_credentials));
            }
        }

        public ObservableCollection<AuditLogEntry> AuditLogEntries
        {
            get
            {
                return (_auditLogEntries);
            }
        }

        public Boolean HasCredentials
        {
            get
            {
                return (_credentials.Count > 0);
            }
        }

        public Boolean IsEmpty
        {
            get
            {
                return (_credentials.Count == 0);
            }
        }

        public Boolean IsDirty
        {
            get
            {
                return (_isDirty);
            }
        }

        public Boolean IsLoaded
        {
            get
            {
                return (_isLoaded);
            }
        }

        #endregion

        #region constructor / destructor

        public Vault()
        {
            _credentials = new ObservableCollection<Credential>();
            _credentials.CollectionChanged += _credentials_CollectionChanged;
            _auditLogEntries = new ObservableCollection<AuditLogEntry>();
            SetLoaded();
            AddAuditEntry(AuditLogEntry.EntryType.CreatedVault,
                new KeyValuePair<String, String>("Version", AssemblyVersion().ToString()));
        }

        public Vault(String name,
            String description)
        {
            _name = name;
            _description = description;
            _credentials = new ObservableCollection<Credential>();
            _credentials.CollectionChanged += _credentials_CollectionChanged;
            _auditLogEntries = new ObservableCollection<AuditLogEntry>();
            SetLoaded();
            AddAuditEntry(AuditLogEntry.EntryType.CreatedVault,
                new KeyValuePair<String, String>("Version", AssemblyVersion().ToString()));
        }

        public Vault(String id,
            String name,
            String description,
            DateTime createdAt,
            DateTime lastUpdatedAt,
            Credential[] credentials,
            AuditLogEntry[] auditLogEntries)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of Vault.");

            _id = id;
            _name = name;
            _description = description;
            _createdAt = createdAt;
            _lastUpdatedAt = lastUpdatedAt;
            _credentials = new ObservableCollection<Credential>();
            _auditLogEntries = new ObservableCollection<AuditLogEntry>(auditLogEntries);
            _credentials.CollectionChanged += _credentials_CollectionChanged;
            if (credentials != null && credentials.Length > 0)
            {
                foreach(Credential curCredential in credentials)
                {
                    curCredential.AddToVault(this, false);
                }
            }            
        }

        #endregion

        #region private methods

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddAuditEntry(AuditLogEntry.EntryType type,
            params KeyValuePair<String, String>[] parameters)
        {
            if (IsLoaded)
            {
                Dictionary<String, String> paramsDictionary = new Dictionary<String, String>(parameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
                if (!paramsDictionary.ContainsKey("Name")) paramsDictionary.Add("Name", Name);
                if (!paramsDictionary.ContainsKey("Type")) paramsDictionary.Add("Type", "vault");
                _auditLogEntries.Insert(0, new AuditLogEntry(type, paramsDictionary.ToArray()));
            }
        }

        #endregion

        #region public methods

        public void ClearAuditEntries()
        {
            _auditLogEntries.Clear();
            MarkAsDirty();
        }

        public static Version AssemblyVersion()
        {
            Assembly assembly = typeof(Vault).Assembly;
            return (assembly.GetName().Version);
        }

        public void SetLoaded()
        {
            _isLoaded = true;
        }

        public void SetSaveParams(ISerialiser serialiser,
            String fullPath,
            String masterPassphrase,
            Boolean isDirty)
        {
            _serialiser = serialiser;
            _fullPath = fullPath;
            _masterPassphrase = masterPassphrase;
            _isDirty = isDirty;
        }

        public async Task<Common.SaveResult> Save(params KeyValuePair<string, string>[] parameters)
        {
            return (await SaveAs(
                _serialiser,
                _fullPath,
                _masterPassphrase,
                true,
                parameters));
        }

        public async Task<Common.SaveResult> SaveAs(
            ISerialiser serialiser,
            String fullPath,
            String masterPassphrase,
            Boolean overwrite,
            params KeyValuePair<string, string>[] parameters)
        {
            bool exists = await Native.Native.FileHandler.Exists(fullPath);
            if(!exists || (exists && overwrite))
            {
                AddAuditEntry(
                    AuditLogEntry.EntryType.Saved,
                    new KeyValuePair<String, String>("Version", AssemblyVersion().ToString()));
                Byte[] serialised = (Byte[])serialiser.Write(this, masterPassphrase, parameters);
                await Native.Native.FileHandler.WriteAsync(
                    fullPath, 
                    serialised);
                SetSaveParams(
                    serialiser,
                    fullPath, 
                    masterPassphrase, 
                    false);
                return (Common.SaveResult.Success);
            }
            return (Common.SaveResult.UnknownError);
        }

        public Credential CreateCredential()
        {
            Credential credential = new Credential(Guid.NewGuid().ToString(),
                this);
            return (credential);
        }

        public void AddCredential(Credential credential,
            Boolean audit)
        {
            if (credential.Vault == null) throw new InvalidOperationException("Credentials must be added to a vault using the Credential.AddToVault method.");
            if (credential.Vault != this) throw new InvalidOperationException("This credential was created for a different vault.");

            if (!CredentialExists(credential))
            {
                _credentials.Add(credential);
                if (audit)
                {
                    AddAuditEntry(AuditLogEntry.EntryType.AddCredential,
                        new KeyValuePair<string, string>("Name", credential.Name));
                }
            }
        }

        public void RemoveCredential(Credential credential)
        {
            if (CredentialExists(credential))
            {
                _credentials.Remove(credential);
                AddAuditEntry(AuditLogEntry.EntryType.RemovedCredential,
                    new KeyValuePair<string, string>("Name", credential.Name));
                NotifyPropertyChanged("IsEmpty");
            }
        }

        public Boolean CredentialExists(string name)
        {
            IEnumerable<Credential> found = Credentials.Where(cred => cred.Name == name);
            return (found.Any());
        }

        public Boolean CredentialExists(Credential credential)
        {
            //At most this should only ever find 1, as this check alone should prevent multiple credentials
            //with the same ID from being added to the vault.
            IEnumerable<Credential> found = Credentials.Where(cred => cred.ID == credential.ID);
            return (found.Any());
        }

        public void MarkAsDirty()
        {
            _isDirty = true;
        }

        #endregion

        #region icomparable

        public int CompareTo(Vault other)
        {
            Int32 idCompare = ID.CompareTo(other.ID);
            if (idCompare != 0) return (idCompare);

            Int32 nameCompare = Name.CompareTo(other.Name);
            if (nameCompare != 0) return (nameCompare);

            Int32 descriptionCompare = Description.CompareTo(other.Description);
            if (descriptionCompare != 0) return (descriptionCompare);

            Int32 createdAtCompare = CreatedAt.CompareTo(other.CreatedAt);
            if (createdAtCompare != 0) return (createdAtCompare);

            Int32 lastUpdatedAtCompare = LastUpdatedAt.CompareTo(other.LastUpdatedAt);
            if (lastUpdatedAtCompare != 0) return (lastUpdatedAtCompare);

            if (Credentials.Count == other.Credentials.Count)
            {
                Credential[] a = Credentials.OrderBy(i => i).ToArray();
                Credential[] b = other.Credentials.OrderBy(i => i).ToArray();
                for(Int32 iCred = 0; iCred < a.Length; iCred++)
                {
                    Credential credA = a[iCred];
                    Credential credB = b[iCred];
                    Int32 compValue = credA.CompareTo(credB);
                    if (compValue != 0) return (compValue);
                }
            }
            else
            {
                return (Credentials.Count.CompareTo(other.Credentials.Count));
            }

            if(AuditLogEntries.Count == other.AuditLogEntries.Count)
            {
                AuditLogEntry[] a = AuditLogEntries.OrderBy(i => i.DateTime).ToArray();
                AuditLogEntry[] b = other.AuditLogEntries.OrderBy(i => i.DateTime).ToArray();
                for (Int32 iEntry = 0; iEntry < a.Length; iEntry++)
                {
                    AuditLogEntry entryA = a[iEntry];
                    AuditLogEntry entryB = b[iEntry];
                    Int32 compValue = entryA.CompareTo(entryB);
                    if (compValue != 0) return (compValue);
                }
            }
            else
            {
                return (AuditLogEntries.Count.CompareTo(other.AuditLogEntries.Count));
            }

            return (0);
        }

        #endregion

        #region object events

        private void _credentials_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _isDirty = true;
            NotifyPropertyChanged("Credentials");
            NotifyPropertyChanged("HasCredentials");
            NotifyPropertyChanged("IsEmpty");
        }

        #endregion

    }

}

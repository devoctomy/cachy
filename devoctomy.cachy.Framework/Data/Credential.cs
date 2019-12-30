using devoctomy.DFramework.Core.SystemExtensions;
using devoctomy.DFramework.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace devoctomy.cachy.Framework.Data
{

    public class Credential : IComparable<Credential>, INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private string _id = Guid.NewGuid().ToString();
        private string _glyphKey = "Login_01";
        private string _glyphColour = "Black";
        private string _name = String.Empty;
        private string _description = String.Empty;
        private string _website = String.Empty;
        private DateTime _createdAt = DateTime.UtcNow;
        private DateTime _lastModifiedAt = DateTime.UtcNow;
        private DateTime _passwordLastModifiedAt = DateTime.UtcNow;
        private string _notes = String.Empty;
        private string _username = String.Empty;
        private string _password = String.Empty;
        private ObservableCollection<String> _tags;
        private ObservableCollection<AuditLogEntry> _auditLogEntries;
        private ObservableCollection<ClipboardField> _clipboardFields;
        private bool _isSelected;
        private bool _blockVaultDirty;

        private static List<StandardFieldAttribute> _standardFields;

        private Vault _vault;

        #endregion

        #region public properties

        public Credential Me
        {
            get
            {
                return (this);
            }
        }

        public static List<StandardFieldAttribute> StandardFields
        {
            get
            {
                _standardFields = new List<StandardFieldAttribute>();
                Type type = typeof(Credential);
                PropertyInfo[] properties = type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                foreach(PropertyInfo curProperty in properties)
                {
                    Object[] standardFieldsAttributes = curProperty.GetCustomAttributes(typeof(StandardFieldAttribute), true);
                    if (standardFieldsAttributes != null && standardFieldsAttributes.Length == 1)
                    {
                        foreach (Object curField in standardFieldsAttributes)
                        {
                            StandardFieldAttribute curStandardField = curField as StandardFieldAttribute;
                            _standardFields.Add(curStandardField);
                        }
                    }
                }
                return (_standardFields);
            }
        }

        public string ID
        {
            get
            {
                return (_id);
            }
        }

        public string GlyphKey
        {
            get
            {
                return (_glyphKey);
            }
            set
            {
                if(_glyphKey != value)
                {
                    _glyphKey = value;
                    AddAuditEntry(AuditLogEntry.EntryType.ModifyGlyphKey, false);
                    NotifyPropertyChanged(true, "GlyphKey");
                }
            }
        }

        public string GlyphColour
        {
            get
            {
                return (_glyphColour);
            }
            set
            {
                if(_glyphColour != value)
                {
                    _glyphColour = value;
                    AddAuditEntry(AuditLogEntry.EntryType.ModifyGlyphColour, false);
                    NotifyPropertyChanged(true, "GlyphColour");
                }
            }
        }

        [StandardField("Name", "Name", true)]
        public string Name
        {
            get
            {
                return (_name);
            }
            set
            {
                if (_name != value)
                {
                    String oldName = _name;
                    _name = value;
                    AddAuditEntry(AuditLogEntry.EntryType.ModifyName, false,
                        new KeyValuePair<String, String>("PrevValue", oldName));
                    NotifyPropertyChanged(true, "Name");
                }
            }
        }

        [StandardField("Description", "Description", false)]
        public string Description
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
                    AddAuditEntry(AuditLogEntry.EntryType.ModifyDescription, false,
                        new KeyValuePair<String, String>("PrevValue", oldDescription));
                    NotifyPropertyChanged(true, "Description");
                }
            }
        }

        [StandardField("Website", "Website", false)]
        public string Website
        {
            get
            {
                return (_website);
            }
            set
            {
                if(_website != value)
                {
                    _website = value;
                    NotifyPropertyChanged(true, "Website");
                }
            }
        }

        public bool HasWebsite
        {
            get
            {
                return (!String.IsNullOrEmpty(Website));
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                return (_createdAt);
            }
        }

        public DateTime LastModifiedAt
        {
            get
            {
                return (_lastModifiedAt);
            }
        }

        public DateTime PasswordLastModifiedAt
        {
            get
            {
                return (_passwordLastModifiedAt);
            }
        }

        [StandardField("Notes", "Notes", false)]
        public string Notes
        {
            get
            {
                return (_notes);
            }
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    AddAuditEntry(AuditLogEntry.EntryType.ModifyNotes, false);
                    NotifyPropertyChanged(true, "Notes");
                }
            }
        }

        [StandardField("Username", "Username", false)]
        public string Username
        {
            get
            {
                return (_username);
            }
            set
            {
                if(_username != value)
                {
                    _username = value;
                    AddAuditEntry(AuditLogEntry.EntryType.ModifyUsername, false);
                    NotifyPropertyChanged(true, "Username");
                }
            }
        }

        [StandardField("Password", "Password", false)]
        public string Password
        {
            get
            {
                return (_password);
            }
            set
            {
                if(_password != value)
                {
                    _password = value;
                    _passwordLastModifiedAt = DateTime.UtcNow;
                    AddAuditEntry(AuditLogEntry.EntryType.ModifyPassword, false);
                    NotifyPropertyChanged(true, "Password");
                }
            }
        }

        public ObservableCollection<string> Tags
        {
            get
            {
                return (_tags);
            }
        }

        public ObservableCollection<AuditLogEntry> AuditLogEntries
        {
            get
            {
                return (_auditLogEntries);
            }
        }

        public Vault Vault
        {
            get
            {
                return (_vault);
            }
        }

        public ObservableCollection<ClipboardField> ClipboardFields
        {
            get
            {
                return (_clipboardFields);
            }
        }

        public bool HasClipboardFields
        {
            get
            {
                return (_clipboardFields.Count > 0);
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
                    if(!_isSelected)
                    {
                        ClearClipboardFields();
                    }
                    NotifyPropertyChanged(false, "IsSelected");
                    NotifyPropertyChanged(false, "IsSelectedWithClipboardFields");
                    NotifyPropertyChanged(false, "Me");
                }
            }
        }

        public bool IsSelectedWithClipboardFields
        {
            get
            {
                return (IsSelected && HasClipboardFields);
            }
        }

        public bool BlockVaultDirty
        {
            get
            {
                return (_blockVaultDirty);
            }
            set
            {
                _blockVaultDirty = value;
            }
        }

        public bool HasTags
        {
            get
            {
                return (Tags.Count > 0);
            }
        }

        #endregion

        #region constructor / destructor

        public Credential(
            string id,
            Vault vault)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of Credential.");

            _id = id;
            _vault = vault;
            _tags = new ObservableCollection<String>();
            _tags.CollectionChanged += _tags_CollectionChanged;
            _auditLogEntries = new ObservableCollection<AuditLogEntry>();
            AddAuditEntry(AuditLogEntry.EntryType.CreatedCredential, true);
            _clipboardFields = new ObservableCollection<ClipboardField>();
        }

        public Credential(
            string id,
            string glyphKey,
            string glyphColour,
            string name,
            string description,
            string website,
            DateTime createdAt,
            DateTime lastModifiedAt,
            DateTime passwordLastModifiedAt,
            string username,
            string password,
            string[] tags,
            string notes,
            AuditLogEntry[] auditLogEntries)
        {
            DLoggerManager.Instance.Logger.Log(DFramework.Logging.Interfaces.LoggerMessageType.VerboseHigh | DFramework.Logging.Interfaces.LoggerMessageType.Information | DFramework.Logging.Interfaces.LoggerMessageType.Sensitive, "Creating instance of Credential.");

            _id = id;
            _glyphKey = glyphKey;
            _glyphColour = glyphColour;
            _name = name;
            _description = description;
            _website = website;
            _createdAt = createdAt;
            _lastModifiedAt = lastModifiedAt;
            _passwordLastModifiedAt = passwordLastModifiedAt;
            _username = username;
            _password = password;
            _tags = new ObservableCollection<string>();
            _notes = notes;
            _auditLogEntries = new ObservableCollection<AuditLogEntry>(auditLogEntries);
            _tags.CollectionChanged += _tags_CollectionChanged;
            if (tags != null && tags.Length > 0) tags.AddToObvservableCollection(_tags);
            _clipboardFields = new ObservableCollection<ClipboardField>();
        }

        #endregion

        #region private methods

        private void NotifyPropertyChanged(bool canMarkAsDirty, String propertyName)
        {
            if(Vault != null && canMarkAsDirty && !BlockVaultDirty) Vault.MarkAsDirty();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddAuditEntry(AuditLogEntry.EntryType type,
            Boolean force,
            params KeyValuePair<String, String>[] parameters)
        {
            if (force || Vault != null)
            {
                if (force || Vault.CredentialExists(this) && Vault.IsLoaded)
                {
                    Dictionary<String, String> paramsDictionary = new Dictionary<String, String>(parameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
                    if (!paramsDictionary.ContainsKey("Name")) paramsDictionary.Add("Name", Name);
                    if (!paramsDictionary.ContainsKey("Type")) paramsDictionary.Add("Type", "credential");
                    _auditLogEntries.Insert(0, new AuditLogEntry(type, paramsDictionary.ToArray()));
                }
            }
        }

        #endregion

        #region public methods

        public void ClearAuditEntries()
        {
            AuditLogEntries.Clear();
            Vault.MarkAsDirty();
        }

        public void SetClipboardFields(ICommand copyCommand,
            ICommand revealCommand)
        {
            _clipboardFields.Clear();
            if (!String.IsNullOrEmpty(Username)) _clipboardFields.Add(new ClipboardField("Username", Username, copyCommand, revealCommand));
            if (!String.IsNullOrEmpty(Password)) _clipboardFields.Add(new ClipboardField("Password", Password, copyCommand, revealCommand));
            if (!String.IsNullOrEmpty(Notes)) _clipboardFields.Add(new ClipboardField("Notes", Notes, copyCommand, revealCommand));
            NotifyPropertyChanged(false, "ClipboardFields");
            NotifyPropertyChanged(false, "HasClipboardFields");
            NotifyPropertyChanged(false, "IsSelectedWithClipboardFields");
        }

        public void ClearClipboardFields()
        {
            _clipboardFields.Clear();
            NotifyPropertyChanged(false, "ClipboardFields");
            NotifyPropertyChanged(false, "HasClipboardFields");
            NotifyPropertyChanged(false, "IsSelectedWithClipboardFields");
        }

        public Credential Clone(bool keepID)
        {
            Credential cloned = new Credential(keepID ? ID : Guid.NewGuid().ToString(),
                GlyphKey,
                GlyphColour,
                Name,
                Description,
                Website,
                CreatedAt,
                LastModifiedAt,
                PasswordLastModifiedAt,
                Username,
                Password,
                Tags.ToArray(),
                Notes,
                AuditLogEntries.ToArray());
            cloned._vault = Vault;
            return (cloned);
        }

        public void CopyTo(Credential target)
        {
            target.GlyphKey = GlyphKey;
            target.GlyphColour = GlyphColour;
            target.Name = Name;
            target.Description = Description;
            target.Website = Website;
            target.Username = Username;
            target.Password = Password;
            target.Notes = Notes;
            if(!target.CompareTags(Tags))
            {
                target.ReplaceTags(Tags);
            }
            target._createdAt = CreatedAt;
            target._lastModifiedAt = DateTime.UtcNow;
        }

        public bool CompareTags(ObservableCollection<string> tags)
        {
            if(Tags.Count == tags.Count)
            {
                string[] a = Tags.ToArray();
                string[] b = tags.ToArray();
                return (a.SequenceEqual(b));
            }
            else
            {
                return (false);
            }
        }

        public void ReplaceTags(ObservableCollection<string> tags)
        {
            Tags.Clear();
            foreach(string curTag in tags)
            {
                Tags.Add(curTag);
            }
            NotifyPropertyChanged(true, "Tags");
        }

        public void RemoveFromVault()
        {
            if (Vault == null) throw new InvalidOperationException("This credential does not belong to a vault.");
            Vault.RemoveCredential(this);
        }

        public void AddToVault(Boolean audit)
        {
            if (Vault == null) throw new InvalidOperationException("No vault specified to add the credential to.");
            BlockVaultDirty = false;
            Vault.AddCredential(this, audit);
        }

        public void AddToVault(Vault vault, Boolean audit)
        {
            _vault = vault;
            Vault.AddCredential(this, audit);
        }

        #endregion

        #region icomparable

        public int SimpleCompare(Credential other)
        {
            Int32 nameCompare = Name.CompareTo(other.Name);
            if (nameCompare != 0) return (nameCompare);

            Int32 descriptionCompare = Description.CompareTo(other.Description);
            if (descriptionCompare != 0) return (descriptionCompare);

            Int32 websiteCompare = Website.CompareTo(other.Website);
            if (websiteCompare != 0) return (websiteCompare);

            Int32 usernameCompare = Username.CompareTo(other.Username);
            if (usernameCompare != 0) return (usernameCompare);

            Int32 passwordCompare = Password.CompareTo(other.Password);
            if (passwordCompare != 0) return (passwordCompare);

            Int32 notesCompare = Notes.CompareTo(other.Notes);
            if (notesCompare != 0) return (notesCompare);

            return (0);
        }

        public int CompareTo(Credential other)
        {
            Int32 idCompare = ID.CompareTo(other.ID);
            if (idCompare != 0) return (idCompare);

            Int32 glyphKeyCompare = GlyphKey.CompareTo(other.GlyphKey);
            if (glyphKeyCompare != 0) return (glyphKeyCompare);

            Int32 glyphColourCompare = GlyphColour.CompareTo(other.GlyphColour);
            if (glyphColourCompare != 0) return (glyphColourCompare);

            Int32 nameCompare = Name.CompareTo(other.Name);
            if (nameCompare != 0) return (nameCompare);

            Int32 descriptionCompare = Description.CompareTo(other.Description);
            if (descriptionCompare != 0) return (descriptionCompare);

            Int32 websiteCompare = Website.CompareTo(other.Website);
            if (websiteCompare != 0) return (websiteCompare);

            Int32 createdAtCompare = CreatedAt.CompareTo(other.CreatedAt);
            if (createdAtCompare != 0) return (createdAtCompare);

            Int32 lastUpdatedAtCompare = LastModifiedAt.CompareTo(other.LastModifiedAt);
            if (lastUpdatedAtCompare != 0) return (lastUpdatedAtCompare);

            Int32 usernameCompare = Username.CompareTo(other.Username);
            if (usernameCompare != 0) return (usernameCompare);

            Int32 passwordCompare = Password.CompareTo(other.Password);
            if (passwordCompare != 0) return (passwordCompare);

            if (Tags.Count == other.Tags.Count)
            {
                if (!Tags.OrderBy(i => i).SequenceEqual(other.Tags.OrderBy(i => i)))
                {
                    return (-1);
                }
            }
            else
            {
                return (-1);
            }

            Int32 notesCompare = Notes.CompareTo(other.Notes);
            if (notesCompare != 0) return (notesCompare);

            return (0);
        }

        #endregion

        #region object events

        private void _tags_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AddAuditEntry(AuditLogEntry.EntryType.ModifyTags, false);
            NotifyPropertyChanged(true, "Tags");
            NotifyPropertyChanged(false, "HasTags");
            NotifyPropertyChanged(false, "Me");
        }

        #endregion

    }

}

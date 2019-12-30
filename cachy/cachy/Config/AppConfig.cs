using devoctomy.cachy.Framework.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace cachy.Config
{

    public class AppConfig : INotifyPropertyChanged
    {

        #region public enums

        public const string CACHY_KEYDERIVATIONFUNCTION_PBKDF2 = "PBKDF2";
        public const string CACHY_KEYDERIVATIONFUNCTION_SCRYPT = "SCRYPT";

        #endregion

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private constants

        public const string APPCONFIG_FILENAME = "cachy.config.json";

        #endregion

        #region private objects

        private static AppConfig _instance;
        private Config _config;
        private Boolean _isDirty;

        private bool _isFirstRun = true;
        private bool _autoOpenVault = false;
        private string _defaultVaultID = string.Empty;
        private bool _autoCloseVault = false;
        private TimeSpan _autoCloseTimeSpan = new TimeSpan(0, 5, 0);
        private bool _autoSave = true;
        private bool _autoSaveOnAcceptCredChanges = true;
        private bool _autoSaveOnDuplicatingCred = true;
        private bool _autoSaveOnDeletingCred = true;
        private string _defaultBrowseProtocol = String.Empty;
        private bool _showPasswordAge;
        private int _daysForOld = 180;
        private string _keyDerivationFunction = CACHY_KEYDERIVATIONFUNCTION_SCRYPT;
        private int _pbkdf2IterationCount = 10000;
        private int _scryptIterationCount = 16384;
        private int _scriptBlockSize = 8;
        private int _scryptThreadCount = 1;
        private bool _enableClipboardObfuscator = true;
        private int _clipboardObfuscatorDisableSecondsAfterCopy = 60;
        private string _memorablePasswordFormat = AppConstants.DEFAULT_MEMORABLEPASSWORDFORMAT;

        #endregion

        #region public properties

        public static AppConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppConfig();
                }
                return (_instance);
            }
        }

        public bool IsFirstRun
        {
            get
            {
                return (_isFirstRun);
            }
            set
            {
                if(_isFirstRun != value)
                {
                    _isFirstRun = value;
                    _isDirty = true;
                    NotifyPropertyChanged("IsFirstRun");
                }
            }
        }

        public bool AutoOpenVault
        {
            get
            {
                return (_autoOpenVault);
            }
            set
            {
                if (_autoOpenVault != value)
                {
                    _autoOpenVault = value;
                    _isDirty = true;
                    NotifyPropertyChanged("AutoOpenVault");
                }
            }
        }

        public string DefaultVaultID
        {
            get
            {
                return (_defaultVaultID);
            }
            set
            {
                if (_defaultVaultID != value)
                {
                    _defaultVaultID = value;
                    _isDirty = true;
                    NotifyPropertyChanged("DefaultVaultID");
                }
            }
        }

        public bool AutoCloseVault
        {
            get
            {
                return (_autoCloseVault);
            }
            set
            {
                if (_autoCloseVault != value)
                {
                    _autoCloseVault = value;
                    _isDirty = true;
                    NotifyPropertyChanged("AutoCloseVault");
                }
            }
        }

        public TimeSpan AutoCloseTimeSpan
        {
            get
            {
                return (_autoCloseTimeSpan);
            }
            set
            {
                if (_autoCloseTimeSpan != value)
                {
                    _autoCloseTimeSpan = value;
                    _isDirty = true;
                    NotifyPropertyChanged("AutoCloseTimeSpan");
                }
            }
        }

        public bool AutoSave
        {
            get
            {
                return (_autoSave);
            }
            set
            {
                if(_autoSave != value)
                {
                    _autoSave = value;
                    _isDirty = true;
                    NotifyPropertyChanged("AutoSave");
                }
            }
        }

        public bool AutoSaveOnAcceptCredChanges
        {
            get
            {
                return (_autoSaveOnAcceptCredChanges);
            }
            set
            {
                if(_autoSaveOnAcceptCredChanges != value)
                {
                    _autoSaveOnAcceptCredChanges = value;
                    _isDirty = true;
                    NotifyPropertyChanged("AutoSaveOnAcceptCredChanges");
                }
            }
        }

        public bool AutoSaveOnDuplicatingCred
        {
            get
            {
                return (_autoSaveOnDuplicatingCred);
            }
            set
            {
                if(_autoSaveOnDuplicatingCred != value)
                {
                    _autoSaveOnDuplicatingCred = value;
                    _isDirty = true;
                    NotifyPropertyChanged("AutoSaveOnDuplicatingCred");
                }
            }
        }

        public bool AutoSaveOnDeletingCred
        {
            get
            {
                return (_autoSaveOnDeletingCred);
            }
            set
            {
                if (_autoSaveOnDeletingCred != value)
                {
                    _autoSaveOnDeletingCred = value;
                    _isDirty = true;
                    NotifyPropertyChanged("AutoSaveOnDeletingCred");
                }
            }
        }

        public string DefaultBrowseProtocol
        {
            get
            {
                return (_defaultBrowseProtocol);
            }
            set
            {
                if(_defaultBrowseProtocol != value)
                {
                    _defaultBrowseProtocol = value;
                    _isDirty = true;
                    NotifyPropertyChanged("DefaultBrowseProtocol");
                }
            }
        }

        public bool ShowPasswordAge
        {
            get
            {
                return (_showPasswordAge);
            }
            set
            {
                _showPasswordAge = value;
            }
        }

        public int DaysForOld
        {
            get
            {
                return (_daysForOld);
            }
            set
            {
                _daysForOld = value;
            }
        }

        public string KeyDerivationFunction
        {
            get
            {
                return (_keyDerivationFunction);
            }
            set
            {
                if (_keyDerivationFunction != value)
                {
                    _keyDerivationFunction = value;
                    _isDirty = true;
                    NotifyPropertyChanged("KeyDerivationFunction");
                }
            }
        }

        public int PBKDF2IterationCount
        {
            get
            {
                return (_pbkdf2IterationCount);
            }
            set
            {
                if (_pbkdf2IterationCount != value)
                {
                    _pbkdf2IterationCount = value;
                    _isDirty = true;
                    NotifyPropertyChanged("PBKDF2IterationCount");
                }
            }
        }

        public int SCryptIterationCount
        {
            get
            {
                return (_scryptIterationCount);
            }
            set
            {
                if (_scryptIterationCount != value)
                {
                    _scryptIterationCount = value;
                    _isDirty = true;
                    NotifyPropertyChanged("SCryptIterationCount");
                }
            }
        }

        public int SCryptBlockSize
        {
            get
            {
                return (_scriptBlockSize);
            }
            set
            {
                if (_scriptBlockSize != value)
                {
                    _scriptBlockSize = value;
                    _isDirty = true;
                    NotifyPropertyChanged("SCryptBlockSize");
                }
            }
        }

        public int SCryptThreadCount
        {
            get
            {
                return (_scryptThreadCount);
            }
            set
            {
                if (_scryptThreadCount != value)
                {
                    _scryptThreadCount = value;
                    _isDirty = true;
                    NotifyPropertyChanged("SCryptThreadCount");
                }
            }
        }

        public Boolean EnableClipboardObfuscator
        {
            get
            {
                return (_enableClipboardObfuscator);
            }
            set
            {
                if (_enableClipboardObfuscator != value)
                {
                    _enableClipboardObfuscator = value;
                    _isDirty = true;
                    NotifyPropertyChanged("EnableClipboardObfuscator");
                }
            }
        }

        public int ClipboardObfuscatorDisableSecondsAfterCopy
        {
            get
            {
                return (_clipboardObfuscatorDisableSecondsAfterCopy);
            }
            set
            {
                if (_clipboardObfuscatorDisableSecondsAfterCopy != value)
                {
                    _clipboardObfuscatorDisableSecondsAfterCopy = value;
                    _isDirty = true;
                    NotifyPropertyChanged("ClipboardObfuscatorDisableSecondsAfterCopy");
                }
            }
        }

        public string MemorablePasswordFormat
        {
            get
            {
                return (_memorablePasswordFormat);
            }
            set
            {
                if(_memorablePasswordFormat != value)
                {
                    _memorablePasswordFormat = value;
                    _isDirty = true;
                    NotifyPropertyChanged("MemorablePasswordFormat");
                }
            }
        }

        public Boolean IsDirty
        {
            get
            {
                return (_isDirty);
            }
        }

        #endregion

        #region constructor / destructor

        public AppConfig()
        {
            _config = Config.Open(APPCONFIG_FILENAME);
            Reload();
        }

        #endregion

        #region public methods

        public static void Initialise()
        {
            if (!Config.Exists(APPCONFIG_FILENAME))
            {
                //we setup our default values here and then save the file
                Config config = Config.Open(APPCONFIG_FILENAME);
                config.GetValue<bool>("IsFirstRun", true);
                config.GetValue<bool>("AutoOpenVault", false);
                config.GetValue<string>("DefaultVaultID", String.Empty);
                config.GetValue<bool>("AutoCloseVault", false);
                config.GetValue<string>("AutoCloseTimeSpan", new TimeSpan(0, 5, 0).ToString());
                config.GetValue<bool>("AutoSave", true);
                config.GetValue<string>("DefaultBrowseProtocol", "https://");
                config.GetValue<bool>("ShowPasswordAge", false);
                config.GetValue<int>("DaysForOld", 180);
                config.GetValue<string>("KeyDerivationFunction", CACHY_KEYDERIVATIONFUNCTION_SCRYPT);
                config.GetValue<int>("PBKDF2IterationCount", 10000);
                config.GetValue<int>("SCryptIterationCount", 16384);
                config.GetValue<int>("SCryptBlockSize", 8);
                config.GetValue<int>("SCryptThreadCount", 1);
                config.GetValue<bool>("EnableClipboardObfuscator", true);
                config.GetValue<int>("ClipboardObfuscatorDisableSecondsAfterCopy", 60);
                config.GetValue<string>("MemorablePasswordFormat", AppConstants.DEFAULT_MEMORABLEPASSWORDFORMAT);

                config.Save();
            }
            AppConfig instance = Instance;
        }

        public void Reload()
        {
            _isFirstRun = _config.GetValue<bool>("IsFirstRun", true);
            _autoOpenVault = _config.GetValue<bool>("AutoOpenVault", false);
            _defaultVaultID = _config.GetValue<string>("DefaultVaultID", String.Empty);
            _autoCloseVault = _config.GetValue<bool>("AutoCloseVault", false);
            String autoCloseTimeSpanString = _config.GetValue<String>("AutoCloseTimeSpan", new TimeSpan(0, 5, 0).ToString());
            _autoSave = _config.GetValue<bool>("AutoSave", true);
            _autoCloseTimeSpan = TimeSpan.Parse(autoCloseTimeSpanString);
            _defaultBrowseProtocol = _config.GetValue<string>("DefaultBrowseProtocol", "https://");
            _showPasswordAge = _config.GetValue<bool>("ShowPasswordAge", false);
            _daysForOld = _config.GetValue<int>("DaysForOld", 180);
            _keyDerivationFunction = _config.GetValue<string>("KeyDerivationFunction", CACHY_KEYDERIVATIONFUNCTION_SCRYPT);
            _pbkdf2IterationCount = _config.GetValue<int>("PBKDF2IterationCount", 10000);
            _scryptIterationCount = _config.GetValue<int>("SCryptIterationCount", 16384);
            _scriptBlockSize = _config.GetValue<int>("SCryptBlockSize", 8);
            _scryptThreadCount = _config.GetValue<int>("SCryptThreadCount", 1);
            _enableClipboardObfuscator = _config.GetValue<bool>("EnableClipboardObfuscator", true);
            _clipboardObfuscatorDisableSecondsAfterCopy = _config.GetValue<int>("ClipboardObfuscatorDisableSecondsAfterCopy", 60);
            _memorablePasswordFormat = _config.GetValue<string>("MemorablePasswordFormat", AppConstants.DEFAULT_MEMORABLEPASSWORDFORMAT);

            Validate();
        }

        public void Save()
        {
            Validate();
            _config.SetValue<bool>("IsFirstRun", IsFirstRun);
            _config.SetValue<bool>("AutoOpenVault", AutoOpenVault);
            _config.SetValue<string>("DefaultVaultID", DefaultVaultID);
            _config.SetValue<bool>("AutoCloseVault", AutoCloseVault);
            _config.SetValue<string>("AutoCloseTimeSpan", AutoCloseTimeSpan.ToString());
            _config.SetValue<bool>("AutoSave", AutoSave);
            _config.SetValue<string>("DefaultBrowseProtocol", DefaultBrowseProtocol);
            _config.SetValue<bool>("ShowPasswordAge", ShowPasswordAge);
            _config.SetValue<int>("DaysForOld", DaysForOld);
            _config.SetValue<string>("KeyDerivationFunction", KeyDerivationFunction);
            _config.SetValue<int>("PBKDF2IterationCount", PBKDF2IterationCount);
            _config.SetValue<int>("SCryptIterationCount", SCryptIterationCount);
            _config.SetValue<int>("SCryptBlockSize", SCryptBlockSize);
            _config.SetValue<int>("SCryptThreadCount", SCryptThreadCount);
            _config.SetValue<bool>("EnableClipboardObfuscator", EnableClipboardObfuscator);
            _config.SetValue<int>("ClipboardObfuscatorDisableSecondsAfterCopy", ClipboardObfuscatorDisableSecondsAfterCopy);
            _config.SetValue<string>("MemorablePasswordFormat", MemorablePasswordFormat);
            _isDirty = false;
        }

        public static void DiscardChanges()
        {
            _instance = null;
        }

        #endregion

        #region private methods

        private void Validate()
        {
            if (AutoOpenVault)
            {
                if (!String.IsNullOrEmpty(DefaultVaultID))
                {
                    if (VaultIndexFile.Instance.Indexes.Count == 0)
                    {
                        AutoOpenVault = false;
                        DefaultVaultID = String.Empty;
                    }
                    else
                    {
                        IEnumerable<VaultIndex> defaultVaultMatches = VaultIndexFile.Instance.Indexes.Where(vi => vi.ID == DefaultVaultID);
                        if (!defaultVaultMatches.Any())
                        {
                            AutoOpenVault = false;
                            DefaultVaultID = String.Empty;
                        }
                    }
                }
                else
                {
                    AutoOpenVault = false;
                }
            }
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

}
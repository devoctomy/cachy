using cachy.Config;
using devoctomy.cachy.Framework.Data;
using devoctomy.cachy.Framework.Data.Cloud;
using devoctomy.cachy.Framework.Serialisers.AESEncrypted;
using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cachy.Pages
{

    public class FirstRunFinishEventArgs
    {

        #region private objects

        private FirstRun.FirstRunStage _stage = FirstRun.FirstRunStage.None;
        private Common.PasswordDerivationMode _passwordDerivationMode = Common.PasswordDerivationMode.None;
        private bool _clipboardObfuscatorEnabled;
        private bool _autoSaveEnabled;
        private Common.SyncMode _syncMode = Common.SyncMode.None;
        private CloudProvider _cloudProvider;
        private ProviderType _cloudStorageProviderType;
        private CloudStorageProviderUserBase _cloudStorageAccountUser;
        private string _name = String.Empty;
        private string _description = String.Empty;
        private string _masterPassphrase = String.Empty;

        #endregion

        #region public properties

        public FirstRun.FirstRunStage Stage
        {
            get
            {
                return (_stage);
            }
        }

        public Common.PasswordDerivationMode PasswordDerivationMode
        {
            get
            {
                return (_passwordDerivationMode);
            }
        }

        public bool ClipboardObfuscatorEnabled
        {
            get
            {
                return (_clipboardObfuscatorEnabled);
            }
        }

        public bool AutoSaveEnabled
        {
            get
            {
                return (_autoSaveEnabled);
            }
        }

        public Common.SyncMode SyncMode
        {
            get
            {
                return (_syncMode);
            }
        }

        public CloudProvider CloudProvider
        {
            get
            {
                return (_cloudProvider);
            }
        }

        public ProviderType CloudStorageProviderType
        {
            get
            {
                return (_cloudStorageProviderType);
            }
        }

        public CloudStorageProviderUserBase CloudStorageAccountUser
        {
            get
            {
                return (_cloudStorageAccountUser);
            }
        }

        public String Name
        {
            get
            {
                return (_name);
            }
        }

        public string Description
        {
            get
            {
                return (_description);
            }
        }

        public string MasterPassphrase
        {
            get
            {
                return (_masterPassphrase);
            }
        }

        #endregion

        #region constructor / destructor

        public FirstRunFinishEventArgs(
            FirstRun.FirstRunStage stage,
            Common.PasswordDerivationMode passwordDerivationMode,
            bool clipboardObfuscatorEnabled,
            bool autoSaveEnabled,
            Common.SyncMode syncMode,
            CloudProvider cloudProvider,
            ProviderType cloudStorageProviderType,
            CloudStorageProviderUserBase cloudStorageAccountUser,
            string name,
            string description,
            string masterPassphrase)
        {
            _stage = stage;
            _passwordDerivationMode = passwordDerivationMode;
            _clipboardObfuscatorEnabled = clipboardObfuscatorEnabled;
            _autoSaveEnabled = autoSaveEnabled;
            _syncMode = syncMode;
            _cloudProvider = cloudProvider;
            _cloudStorageProviderType = cloudStorageProviderType;
            _cloudStorageAccountUser = cloudStorageAccountUser;
            _name = name;
            _description = description;
            _masterPassphrase = masterPassphrase;
        }

        public FirstRunFinishEventArgs() :
            this(FirstRun.FirstRunStage.None, Common.PasswordDerivationMode.None, false, false, Common.SyncMode.None, null, null, null, String.Empty, String.Empty, String.Empty)
        {
        }

        public FirstRunFinishEventArgs(
            Common.PasswordDerivationMode passwordDerivationMode,
            bool clipboardObfuscatorEnabled,
            bool autoSaveEnabled) :
            this(FirstRun.FirstRunStage.DontCreateFirstVault, passwordDerivationMode, clipboardObfuscatorEnabled, autoSaveEnabled, Common.SyncMode.None, null, null, null, String.Empty, String.Empty, String.Empty)
        {
        }

        public FirstRunFinishEventArgs(
            Common.PasswordDerivationMode passwordDerivationMode,
            bool clipboardObfuscatorEnabled,
            bool autoSaveEnabled,
            Common.SyncMode syncMode,
            CloudProvider cloudProvider,
            ProviderType cloudStorageProviderType,
            CloudStorageProviderUserBase cloudStorageAccountUser,
            string name,
            string description,
            string masterPassphrase) :
            this(FirstRun.FirstRunStage.FullSetup, passwordDerivationMode, clipboardObfuscatorEnabled, autoSaveEnabled, syncMode, cloudProvider, cloudStorageProviderType, cloudStorageAccountUser, name, description, masterPassphrase)
        {
        }

        #endregion

        #region private methods

        private string CreateFileName(
            Vault vault,
            string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                return (String.Format("{0}.vault", vault.ID));
            }
            else
            {
                string name = fileName;
                name = name.Replace("{ID}", vault.ID);
                name = name.Replace("{Name}", vault.Name);
                name = name.Replace("{ddMMyyyy}", DateTime.Now.ToString("ddMMyyyy"));
                return (name);
            }
        }

        #endregion

        #region public methods

        public void ApplySettings()
        {
            AppConfig.Instance.IsFirstRun = false;

            if (Stage == FirstRun.FirstRunStage.None)
            {
                AppConfig.Instance.Save();
                return;
            }

            AppConfig.Instance.KeyDerivationFunction = PasswordDerivationMode == Common.PasswordDerivationMode.PBKDF2 ? AppConfig.CACHY_KEYDERIVATIONFUNCTION_PBKDF2 : AppConfig.CACHY_KEYDERIVATIONFUNCTION_SCRYPT;
            AppConfig.Instance.EnableClipboardObfuscator = ClipboardObfuscatorEnabled;
            AppConfig.Instance.AutoSave = AutoSaveEnabled;
            AppConfig.Instance.Save();
        }

        public async Task CreateVault()
        {
            if (Stage != FirstRun.FirstRunStage.FullSetup) return;

            Vault vault = new Vault(Name,
                Description);
            String appDataPath = String.Empty;
            Directory.ResolvePath("{AppData}", out appDataPath);
            if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
            string fileName = CreateFileName(vault, "{Name}-{ddMMyyyy}.vault");
            String vaultFullPath = String.Format("{0}{1}", appDataPath, String.Format(@"LocalVaults{0}{1}", DLoggerManager.PathDelimiter, fileName));

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("KeyDerivationFunction", AppConfig.Instance.KeyDerivationFunction);
            switch (AppConfig.Instance.KeyDerivationFunction)
            {
                case AppConfig.CACHY_KEYDERIVATIONFUNCTION_PBKDF2:
                    {
                        parameters.Add("IterationCount", AppConfig.Instance.PBKDF2IterationCount.ToString());
                        break;
                    }
                case AppConfig.CACHY_KEYDERIVATIONFUNCTION_SCRYPT:
                    {
                        parameters.Add("IterationCount", AppConfig.Instance.SCryptIterationCount.ToString());
                        parameters.Add("BlockSize", AppConfig.Instance.SCryptBlockSize.ToString());
                        parameters.Add("ThreadCount", AppConfig.Instance.SCryptThreadCount.ToString());
                        break;
                    }
            }

            Common.SaveResult result = await vault.SaveAs(
                new AESEncryptedVaultSerialiser(),
                vaultFullPath,
                MasterPassphrase,
                true,
                parameters.ToArray());

            if (result == Common.SaveResult.Success)
            {
                if (SyncMode == Common.SyncMode.CloudProvider)
                {
                    VaultIndexFile.Instance.AddVaultToLocalVaultStoreIndex(vault,
                        Common.SyncMode.CloudProvider,
                        CloudProvider.ID,
                        String.Format("/{0}/{1}", "Vaults", fileName),
                        false);
                }
                else
                {
                    VaultIndexFile.Instance.AddVaultToLocalVaultStoreIndex(vault,
                        Common.SyncMode.LocalOnly,
                        String.Empty,
                        String.Empty,
                        false);
                }
            }
            else
            {
                await App.Controller.MainPageInstance.DisplayAlert("Create Vault",
                    "Failed to create vault.",
                    "OK");
            }
        }

        #endregion

    }

}

using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace devoctomy.cachy.Framework.Data
{

    public class VaultIndexFile
    {

        #region private objects

        private static VaultIndexFile _indexFile;
        private String _fullPath = String.Empty;
        private ObservableCollection<VaultIndex> _vaults;

        #endregion

        #region public properties

        public static VaultIndexFile Instance
        {
            get
            {
                if(_indexFile == null)
                {
                    String appDataPath = String.Empty;
                    Directory.ResolvePath("{AppData}", out appDataPath);
                    if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
                    String indexFullPath = String.Format("{0}Config{1}{2}", appDataPath, DLoggerManager.PathDelimiter, "index.json");
                    _indexFile = FromFile(indexFullPath);
                }
                return (_indexFile);
            }
        }

        public static string LocalVaultPath
        {
            get
            {
                String appDataPath = String.Empty;
                devoctomy.DFramework.Core.IO.Directory.ResolvePath("{AppData}", out appDataPath);
                if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
                String localVaultStoreFullPath = String.Format("{0}{1}", appDataPath, String.Format(@"LocalVaults{0}", DLoggerManager.PathDelimiter));
                return (localVaultStoreFullPath);
            }
        }


        public string FullPath
        {
            get
            {
                return (_fullPath);
            }
        }

        public ObservableCollection<VaultIndex> Indexes
        {
            get
            {
                return (_vaults);
            }
        }

        #endregion

        #region constructor / destructor

        private VaultIndexFile()
        {
            _vaults = new ObservableCollection<VaultIndex>();
        }

        #endregion

        #region private methods

        private static VaultIndexFile FromFile(String fullPath)
        {
            VaultIndexFile vaultIndexFile = new VaultIndexFile();
            vaultIndexFile._fullPath = fullPath;
            if(System.IO.File.Exists(vaultIndexFile._fullPath))
            {
                String vaultIndexData = System.IO.File.ReadAllText(fullPath);
                JObject vaultIndexJSON = JObject.Parse(vaultIndexData);
                JArray vaults = vaultIndexJSON["Vaults"].Value<JArray>();
                foreach (JObject curVaultIndex in vaults)
                {
                    VaultIndex curIndex = VaultIndex.FromJSON(curVaultIndex);
                    vaultIndexFile._vaults.Add(curIndex);
                }
            }
            return (vaultIndexFile);
        }

        #endregion

        #region public methods

        public static void Invalidate()
        {
            _indexFile = null;
        }

        public VaultIndex CreateLocalVaultStoreIndex(String name,
            String description,
            Common.SyncMode syncMode,
            string provider,
            String fullPath,
            string cloudProviderPath)
        {
            VaultIndex vaultIndex = new VaultIndex(Guid.NewGuid().ToString(),
                name,
                description,
                syncMode,
                provider,
                fullPath,
                cloudProviderPath,
                true);
            _vaults.Add(vaultIndex);
            Save();
            return (vaultIndex);
        }

        public bool VaultIsIndexed(Vault vault)
        {
            IEnumerable<VaultIndex> matches = VaultIndexFile.Instance.Indexes.Where(vi => vi.ID == vault.ID);
            return (matches.Any());
        }

        public VaultIndex AddVaultToLocalVaultStoreIndex(Vault vault,
            Common.SyncMode syncMode,
            string provider,
            string cloudProviderPath,
            bool unopened)
        {
            VaultIndex vaultIndex = new VaultIndex(vault.ID,
                vault.Name,
                vault.Description,
                syncMode,
                provider,
                vault.FullPath,
                cloudProviderPath,
                unopened);
            _vaults.Add(vaultIndex);
            Save();
            return (vaultIndex);
        }

        public Boolean RemoveFromVault(VaultIndex vaultIndex)
        {
            if (_vaults.Contains(vaultIndex))
            {
                _vaults.Remove(vaultIndex);
                Save();
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public Boolean RemoveFromVault(Vault vault)
        {
            IEnumerable<VaultIndex> matchingVaults = _vaults.Where(vi => vi.ID == vault.ID);
            if(matchingVaults.Any())
            {
                VaultIndex vaultIndex = matchingVaults.First();
                _vaults.Remove(vaultIndex);
                Save();
                return (true);
            }
            else
            {
                return (false);
            }
        }

        public Boolean? DeleteFromVault(VaultIndex vaultIndex)
        {
            if (RemoveFromVault(vaultIndex))
            {
                try
                {
                    System.IO.File.Delete(vaultIndex.FullPath);
                    return (true);
                }
                catch
                {
                    return (false);
                }
            }
            else
            {
                return (null);
            }
        }

        public void Save()
        {
            JObject vaultIndexJSON = new JObject();
            JArray vaults = new JArray();
            foreach (VaultIndex curIndex in Indexes)
            {
                vaults.Add(curIndex.ToJSON());
            }
            vaultIndexJSON.Add("Vaults", vaults);
            System.IO.File.WriteAllText(_fullPath, vaultIndexJSON.ToString());
        }

        #endregion

    }

}

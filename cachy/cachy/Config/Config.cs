using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;
using Newtonsoft.Json.Linq;
using System;

namespace cachy.Config
{

    public class Config
    {

        #region private objects

        private string _fullPath = String.Empty;
        private JObject _json;

        #endregion

        #region public properties

        public string FullPath
        {
            get
            {
                return (_fullPath);
            }
        }

        #endregion

        #region constructor / destructor

        private Config(string fileName)
        {
            _fullPath = GetFullPath(fileName);
            Load();
        }

        #endregion

        #region private methods

        private static string GetFullPath(string fileName)
        {
            String appDataPath = String.Empty;
            Directory.ResolvePath("{AppData}", out appDataPath);
            if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
            string fullPath = String.Format("{0}Config{1}{2}", appDataPath, DLoggerManager.PathDelimiter, fileName);
            return (fullPath);
        }

        private void Load()
        {
            if(System.IO.File.Exists(FullPath))
            {
                String configData = System.IO.File.ReadAllText(FullPath);
                JObject json = JObject.Parse(configData);
                _json = json;
            }
        }

        private void DoSave()
        {
            String configData = _json.ToString(Newtonsoft.Json.Formatting.Indented);
            System.IO.FileInfo configFile = new System.IO.FileInfo(FullPath);
            configFile.Directory.Create();
            System.IO.File.WriteAllText(FullPath, configData);
        }

        #endregion

        #region public methods

        public void Save()
        {
            DoSave();
        }

        public static bool Exists(string fileName)
        {
            string fullPath = GetFullPath(fileName);
            return (System.IO.File.Exists(fullPath));
        }

        public static Config Open(string fileName)
        {
            Config config = new Config(fileName);
            return (config);
        }

        public T GetValue<T>(string key,
            T defaultValue)
        {
            if(_json != null)
            {
                if(_json.ContainsKey(key))
                {
                    return (_json.Value<T>(key));
                }
            }
            SetValue<T>(key, defaultValue);
            return (defaultValue);
        }

        public bool RemoveValue(string key)
        {
            if (_json != null)
            {
                if (_json.ContainsKey(key))
                {
                    _json.Remove(key);
                    return (true);
                }
            }
            return (false);
        }

        public void SetValue<T>(string key,
            T value)
        {
            if(_json == null)
            {
                _json = new JObject();
            }
            if(_json.ContainsKey(key))
            {
                _json.Remove(key);
            }
            _json.Add(key, new JValue(value));
            Save();
        }

        #endregion

    }

}

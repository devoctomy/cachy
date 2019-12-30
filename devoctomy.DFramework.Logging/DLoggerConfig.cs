using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using devoctomy.DFramework.Logging.Interfaces;
using System.Reflection;

namespace devoctomy.DFramework.Logging
{

    public class DLoggerConfig
    {

        #region private objects

        private string _pathOutput = String.Empty;
        private string _defaultLoggerName = String.Empty;
        private bool _defaultEnabled = false;
        private LoggerMessageType _defaultMessageTypes = LoggerMessageType.None;
        private bool _messageTemplateIsUTC;
        private string _messageTemplateText = String.Empty;
        private static Dictionary<String, String> _resolvedPathCache = new Dictionary<String, String>();
        private string _configPath;
        private JObject _configJObject;

        #endregion

        #region public properties

        public String PathOutput
        {
            get
            {
                return (_pathOutput);
            }
        }

        public String DefaultLoggerName
        {
            get
            {
                return (_defaultLoggerName);
            }
        }

        public bool DefaultEnabled
        {
            get
            {
                return (_defaultEnabled);
            }
            set
            {
                if(_defaultEnabled != value)
                {
                    _defaultEnabled = value;
                    SetDefaultEnabled();
                }
            }
        }

        public LoggerMessageType DefaultMessageTypes
        {
            get
            {
                return (_defaultMessageTypes);
            }
        }

        public Boolean MessageTemplateIsUTC
        {
            get
            {
                return (_messageTemplateIsUTC);
            }
        }

        public String MessageTemplateText
        {
            get
            {
                return (_messageTemplateText);
            }
        }

        #endregion

        #region constructor / destructor

        private DLoggerConfig(string configPath,
            string json)
        {
            _configPath = configPath;
            Parse(json);
        }

        #endregion

        #region private methods

        private void Parse(String json)
        {
            _configJObject = JObject.Parse(json);
            _pathOutput = _configJObject["Paths"]["Output"].Value<String>();
            _defaultLoggerName = _configJObject["Default"]["Name"].Value<String>();
            _defaultEnabled = _configJObject["Default"]["Enabled"].Value<bool>();
            String defaultMessageTypes = _configJObject["Default"]["AllowedMessageTypes"].Value<String>();
            _defaultMessageTypes = (LoggerMessageType)Enum.Parse(typeof(LoggerMessageType), defaultMessageTypes, true);
            _messageTemplateIsUTC = Boolean.Parse(_configJObject["MessageTemplate"]["IsUTC"].Value<String>());
            _messageTemplateText = _configJObject["MessageTemplate"]["Text"].Value<String>();
        }

        private void SetDefaultEnabled()
        {
            _configJObject["Default"].Value<JObject>().Remove("Enabled");
            _configJObject["Default"].Value<JObject>().Add("Enabled", new JValue(DefaultEnabled));
            string json = _configJObject.ToString(Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_configPath, json);

            if (!DefaultEnabled)
            {
                DLoggerManager.Instance.Logger.LogEarlyShutdownMessages();
            }
            DLoggerManager.Instance.InitialiseDefaultLogger();
        }

        #endregion

        #region public methods

        public static Boolean LoadDefault(String appName, out DLoggerConfig loadedConfig)
        {
            loadedConfig = null;
            String appDataPath = String.Empty;
            if(!Core.IO.Directory.ResolvePath("{AppData}", out appDataPath))
            {
                return (false);
            }
            if (!appDataPath.EndsWith(DLoggerManager.PathDelimiter)) appDataPath += DLoggerManager.PathDelimiter;
            String configPath = String.Format("{0}{1}", appDataPath, String.Format(@"Config{0}logger.config.json", DLoggerManager.PathDelimiter));
            new FileInfo(configPath).Directory.Create();
            if (!File.Exists(configPath))
            {
                Assembly thisAssembly = typeof(DLoggerConfig).Assembly;
                string embeddedConfigPath = string.Empty;
#if DEBUG
                embeddedConfigPath = "devoctomy.DFramework.Logging.Assets.debug.logger.config.json";
#else
                embeddedConfigPath = "devoctomy.DFramework.Logging.Assets.release.logger.config.json";
#endif
                using (Stream stream = thisAssembly.GetManifestResourceStream(embeddedConfigPath))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        String template = reader.ReadToEnd();
                        template = template.Replace("{AppName}", appName);
                        File.WriteAllText(configPath, template);
                    }
                }
            }
            if (File.Exists(configPath))
            {
                String loggerConfigJSON = File.ReadAllText(configPath);
                loadedConfig = new DLoggerConfig(configPath, loggerConfigJSON);
                return (true);
            }
            else
            {
                throw new FileNotFoundException(String.Format("The logger config '{0}' was not found.", configPath));
                //return (false);
            }
        }

        public static String ParsePath(String path)
        {
            if (Core.IO.Directory.ResolvePath == null) throw new InvalidOperationException("IO.Directory.ResolvePath event must be handled."); 
            String parsedPath = path;

            Regex regEx = new Regex(@"\{(.*?)\}");
            MatchCollection tokenMatches = regEx.Matches(parsedPath);
            foreach(Match curMatch in tokenMatches)
            {
                if(_resolvedPathCache.ContainsKey(curMatch.Value.ToLower()))
                {
                    parsedPath = _resolvedPathCache[curMatch.Value.ToLower()];
                }
                else
                {
                    String resolvedPath = String.Empty;
                    if(Core.IO.Directory.ResolvePath(curMatch.Value, out resolvedPath))
                    {
                        parsedPath = parsedPath.Replace(curMatch.Value, resolvedPath);
                        _resolvedPathCache.Add(curMatch.Value.ToLower(), parsedPath);
                    }
                }
            }

            return (parsedPath);
        }

#endregion

    }

}
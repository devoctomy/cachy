using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace devoctomy.cachy.Build.Config
{

    public class ChangeLog
    {

        #region private objects

        private static object _instanceLock = new object();
        private static ChangeLog _instance;

        private Dictionary<string, Release> _releases;
        private List<Change> _allChanges;

        #endregion

        #region public properties

        public static ChangeLog Instance
        {
            get
            {
                lock(_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = LoadDefault();
                    }
                }
                return (_instance);
            }
        }

        public IReadOnlyList<Change> AllChanges
        {
            get
            {
                return (_allChanges);
            }
        }

        public IReadOnlyDictionary<string, Release> Releases
        {
            get
            {
                return (_releases);
            }
        }

        #endregion

        #region constructor / destructor

        private ChangeLog(JObject json)
        {
            _releases = new Dictionary<string, Release>();
            _allChanges = new List<Change>();
            ParseChanges(json);
        }

        #endregion

        #region private methods

        private static ChangeLog LoadDefault()
        {
            JObject json;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("devoctomy.cachy.Build.Assets.Log.changes.json"))
            {
                using (JsonReader reader = new JsonTextReader(new StreamReader(stream)))
                {
                    reader.DateParseHandling = DateParseHandling.None;
                    json = JObject.Load(reader);
                }
            }

            ChangeLog changeLog = new ChangeLog(json);
            return (changeLog);
        }

        private void ParseChanges(JObject json)
        {
            JObject releases = json["Releases"].Value<JObject>();
            foreach(JProperty curRelease in releases.Children())
            {
                string version = curRelease.Name;
                Version releaseVersion = Version.Parse(version);
                JObject releaseJSON = releases[version].Value<JObject> ();
                Release release = Release.FromJSON(
                    this, 
                    releaseVersion, 
                    releaseJSON);
                _releases.Add(version, release);
                _allChanges.AddRange(release.Changes);
            }
            //Order all changes
            _allChanges.OrderByDescending(ch => ch.Date);
        }

        #endregion

        #region public methods

        public static string CreateLatestReleaseSummary()
        {
            StringBuilder stringBuilder = new StringBuilder();
            Release release = null;
            bool hasUnreleased = Instance.Releases.Values.Any(r => !r.IsReleased);
            if(hasUnreleased)
            {
                release = Instance.Releases.Values.First(r => !r.IsReleased);
                stringBuilder.AppendLine(String.Format("{0} : {1}", "Unreleased", release.Version.ToString()));
            }
            else
            {
                IEnumerable<Release> releases = Instance.Releases.Values.OrderByDescending(r => r.Date);
                release = releases.First();
                stringBuilder.AppendLine(String.Format("{0} : {1}", release.Date.Value.ToString("dd/MM/yyyy"), release.Version.ToString()));
            }
            stringBuilder.AppendLine(new string('-', 24));
            foreach (Change curChange in release.Changes)
            {
                stringBuilder.AppendLine(String.Format("{0}\t: {1}", curChange.ChangeType.ToString(), curChange.Summary));
                stringBuilder.AppendLine(String.Format("\t- {0}", curChange.Description));
            }
            return (stringBuilder.ToString());
        }

        #endregion

    }

}

using Newtonsoft.Json.Linq;
using System;

namespace devoctomy.cachy.Build.Config
{

    public class Change
    {

        #region public enums

        public enum ItemChangeType
        {
            None = 0,
            Added = 1,
            Changed = 2,
            Fixed = 3,
            Removed = 4
        }

        [Flags]
        public enum ItemChangePlatform
        {
            None = 0,
            UWP = 1,
            Android = 2,
            iOS = 4
        }

        #endregion

        #region private objects

        private ChangeLog _log;
        private ItemChangeType _changeType;
        private DateTime _date;
        private Version _releaseVersion;
        private string _summary = String.Empty;
        private string _description = String.Empty;
        private ItemChangePlatform _platform;

        #endregion

        #region public properties

        public ItemChangeType ChangeType
        {
            get
            {
                return (_changeType);
            }
        }

        public string ReleaseVersionLabel
        {
            get
            {
                Release release = _log.Releases[ReleaseVersion.ToString()];
                return (release.ReleaseVersionLabel);
            }
        }

        public Version ReleaseVersion
        {
            get
            {
                return (_releaseVersion);
            }
        }

        public DateTime Date
        {
            get
            {
                return (_date);
            }
        }

        public string Summary
        {
            get
            {
                return (_summary);
            }
        }

        public string Description
        {
            get
            {
                return (_description);
            }
        }

        public ItemChangePlatform Platform
        {
            get
            {
                return (_platform);
            }
        }

        #endregion

        #region constructor / destructor

        private Change(
            ChangeLog log,
            Version releaseVersion,
            ItemChangeType type,
            DateTime date,
            string summary,
            string description,
            ItemChangePlatform platform)
        {
            _log = log;
            _releaseVersion = releaseVersion;
            _changeType = type;
            _date = date;
            _summary = summary;
            _description = description;
            _platform = platform;
        }

        #endregion

        #region public methods

        public static Change FromJSON(
            ChangeLog log,
            Version version,
            JObject json)
        {
            ItemChangeType type = (ItemChangeType)Enum.Parse(typeof(ItemChangeType), json["Type"].Value<String>(), true);
            DateTime date = DateTime.ParseExact(json["Date"].Value<String>(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            string summary = json["Summary"].Value<String>();
            string description = json["Description"].Value<String>();
            ItemChangePlatform platform = (ItemChangePlatform)Enum.Parse(typeof(ItemChangePlatform), json["Platform"].Value<String>(), true);
            return (new Change(
                log,
                version,
                type,
                date,
                summary,
                description,
                platform));
        }

        #endregion

    }

}

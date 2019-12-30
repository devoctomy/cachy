using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace devoctomy.cachy.Build.Config
{

    public class Release
    {

        #region private objects

        private ChangeLog _log;
        private Version _version;
        private DateTime? _date;
        private List<Change> _changes;

        #endregion

        #region public properties

        public DateTime? Date
        {
            get
            {
                return (_date);
            }
        }

        public bool IsReleased
        {
            get
            {
                return (Date.HasValue);
            }
        }

        public Version Version
        {
            get
            {
                return (_version);
            }
        }

        public string ReleaseVersionLabel
        {
            get
            {
                return (IsReleased ?
                    String.Format("{0} : {1}", Date.Value.ToString("dd/MM/yyyy"), Version.ToString()) :
                    String.Format("{0} : {1}", "Not Yet Released", Version.ToString()));
            }
        }

        public IReadOnlyList<Change> Changes
        {
            get
            {
                return (_changes);
            }
        }

        #endregion

        #region constructor / destructor

        public Release(
            ChangeLog log,
            Version version,
            DateTime? date)
        {
            _log = log;
            _version = version;
            _date = date;
            _changes = new List<Change>();
        }

        #endregion

        #region public methods

        public static Release FromJSON(
            ChangeLog log,
            Version version,
            JObject json)
        {
            String date = json["Date"].Value<String>();
            DateTime? releaseDate = String.IsNullOrEmpty(date) ? (DateTime?)null : DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            Release release = new Release(
                log,
                version,
                releaseDate);
            JArray changes = json["Changes"].Value<JArray>();
            foreach (JObject curChange in changes)
            {
                Change change = Change.FromJSON(
                    log,
                    release.Version,
                    curChange);
                release._changes.Add(change);
            }
            release._changes.OrderByDescending(ch => ch.Date);
            return (release);
        }

        #endregion

    }

}

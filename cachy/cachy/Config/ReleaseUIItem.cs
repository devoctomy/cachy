using devoctomy.cachy.Build.Config;
using System.Collections.Generic;

namespace cachy.Config
{

    public class ReleaseUIItem : List<Change>
    {

        #region public properties

        public string Heading { get; set; }

        public List<Change> Changes => this;

        #endregion

        #region public methods


        public static ReleaseUIItem FromRelease(Release release)
        {
            ReleaseUIItem item = new ReleaseUIItem();
            item.AddRange(release.Changes);
            item.Heading = release.ReleaseVersionLabel;
            return (item);
        }

        #endregion

    }

}

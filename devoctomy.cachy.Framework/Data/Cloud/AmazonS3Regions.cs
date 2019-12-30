using Amazon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public sealed class AmazonS3Regions
    {

        #region private objects

        private static object _lock = new object();
        private static List<AmazonS3Region> _allRegions;

        #endregion

        #region public properties

        public static List<AmazonS3Region> AllRegions
        {
            get
            {
                lock(_lock)
                {
                    if (_allRegions == null)
                    {
                        _allRegions = new List<AmazonS3Region>();
                        IEnumerable<RegionEndpoint> regionEndpoints = RegionEndpoint.EnumerableAllRegions;
                        foreach(RegionEndpoint endpoint in regionEndpoints)
                        {
                            AmazonS3Region region = new AmazonS3Region(endpoint.DisplayName, endpoint.SystemName, endpoint.GetEndpointForService("S3").Hostname);
                            _allRegions.Add(region);
                        }
                    }
                }
                return (_allRegions);
            }
        }

        #endregion

        #region public methods

        public static RegionEndpoint GetRegionByDisplayName(string displayName)
        {
            IEnumerable<RegionEndpoint> regionEndpoints = RegionEndpoint.EnumerableAllRegions.Where(re => re.DisplayName == displayName);
            if(regionEndpoints.Any())
            {
                return (regionEndpoints.First());
            }
            else
            {
                throw new NotSupportedException(String.Format("Amazon endpoint region with the display name '{0}' could not be found.", displayName));
            }
        }

        #endregion

    }

}

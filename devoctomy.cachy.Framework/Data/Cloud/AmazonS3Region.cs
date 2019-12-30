using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class AmazonS3Region
    { 

        #region private objects

        private string _name = String.Empty;
        private string _region = String.Empty;
        private string _endpoint = String.Empty;

        #endregion

        #region public properties

        public string Name
        {
            get
            {
                return (_name);
            }
        }

        public  string Region
        {
            get
            {
                return (_region);
            }
        }

        public string Endpoint
        {
            get
            {
                return (_endpoint);
            }
        }

        #endregion

        #region constructor / destructor

        public AmazonS3Region(
            string name,
            string region,
            string endpoint)
        {
            _name = name;
            _region = region;
            _endpoint = endpoint;
        }

        #endregion

        #region base class overrides

        public override string ToString()
        {
            return (Name);
        }

        #endregion

    }

}

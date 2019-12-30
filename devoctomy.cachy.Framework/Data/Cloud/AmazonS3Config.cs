using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class AmazonS3Config : INotifyPropertyChanged
    {

        #region public events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region private objects

        private string _accessID = String.Empty;
        private string _secretKey = String.Empty;
        private string _region = String.Empty;
        private string _bucketName = String.Empty;
        private string _path = String.Empty;

        #endregion

        #region public properties

        public string AccessID
        {
            get
            {
                return (_accessID);
            }
            set
            {
                if (_accessID != value)
                {
                    _accessID = value;
                    NotifyPropertyChanged("AccessID");
                }
            }
        }

        public string SecretKey
        {
            get
            {
                return (_secretKey);
            }
            set
            {
                if (_secretKey != value)
                {
                    _secretKey = value;
                    NotifyPropertyChanged("SecretKey");
                }
            }
        }

        public string Region
        {
            get
            {
                return (_region);
            }
            set
            {
                if(_region != value)
                {
                    _region = value;
                    NotifyPropertyChanged("Region");
                }
            }
        }

        public string BucketName
        {
            get
            {
                return (_bucketName);
            }
            set
            {
                if (_bucketName != value)
                {
                    _bucketName = value;
                    NotifyPropertyChanged("BucketName");
                }
            }
        }

        public string Path
        {
            get
            {
                return (_path);
            }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    NotifyPropertyChanged("Path");
                }
            }
        }

        #endregion

        #region constructor / destructor


        public AmazonS3Config()
        {
        }

        public AmazonS3Config(
            string accessID,
            string secretKey,
            string region,
            string bucketName,
            string path)
        {
            _accessID = accessID;
            _secretKey = secretKey;
            _region = region;
            _bucketName = bucketName;
            _path = path;
        }

        #endregion

        #region private methods

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region public methods

        public JObject ToJSON()
        {
            JObject json = new JObject();
            json.Add("AccessID", new JValue(AccessID));
            json.Add("SecretKey", new JValue(SecretKey));
            json.Add("Region", new JValue(Region));
            json.Add("BucketName", new JValue(BucketName));
            json.Add("Path", new JValue(Path));
            return (json);
        }

        public static AmazonS3Config FromJSON(JObject json)
        {
            string accessID = json["AccessID"].Value<string>();
            string secretKey = json["SecretKey"].Value<string>();
            string region = json["Region"].Value<string>();
            string bucketName = json["BucketName"].Value<string>();
            string path = json["Path"].Value<string>();
            return (new AmazonS3Config(
                accessID,
                secretKey,
                region,
                bucketName,
                path));
        }

        public Dictionary<string, string> ToDictionary()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("AccessID", AccessID);
            parameters.Add("SecretKey", SecretKey);
            parameters.Add("Region", Region);
            parameters.Add("BucketName", BucketName);
            parameters.Add("Path", Path);
            return (parameters);
        }

        #endregion

        #region base class overrides

        public override string ToString()
        {
            return(ToJSON().ToString(Newtonsoft.Json.Formatting.None));
        }

        #endregion

    }

}

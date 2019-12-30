using Newtonsoft.Json.Linq;
using System;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class OneDriveStorageProviderFile : CloudStorageProviderFileBase
    {

        #region constructor / destructor

        public OneDriveStorageProviderFile(string name,
            bool isFolder,
            string path,
            string hash,
            DateTime lastModified) : base(name, isFolder, path, hash, lastModified)
        {
        }

        #endregion

        #region public methods

        public static OneDriveStorageProviderFile FromJSON(
            JObject fileJSON,
            string path)
        {
            string name = fileJSON["name"].Value<string>();
            bool isFolder = fileJSON.ContainsKey("folder");
            string sha1Hash = isFolder ? String.Empty : fileJSON["file"]["hashes"]["sha1Hash"].Value<string>();
            DateTime lastModified = fileJSON["fileSystemInfo"]["lastModifiedDateTime"].Value<DateTime>();

            switch(lastModified.Kind)
            {
                case DateTimeKind.Unspecified:
                    {
                        //incorrect date time kind
                        throw new InvalidOperationException();
                    }
                case DateTimeKind.Local:
                    {
                        lastModified = lastModified.ToUniversalTime();
                        break;
                    }
            }

            return (CloudStorageProviderFileBase.Create<OneDriveStorageProviderFile>(
                name,
                isFolder,
                path,
                !isFolder ? sha1Hash : null,
                !isFolder ? lastModified : new Nullable<DateTime>()));
        }

        #endregion

    }

}

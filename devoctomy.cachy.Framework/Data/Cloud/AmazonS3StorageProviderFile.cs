using Amazon.S3.Model;
using Dropbox.Api.Files;
using System;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class AmazonS3StorageProviderFile : CloudStorageProviderFileBase
    {

        #region constructor / destructor

        public AmazonS3StorageProviderFile(string name,
            bool isFolder,
            string path,
            string hash,
            DateTime lastModified) : base(name, isFolder, path, hash, lastModified)
        {
        }

        #endregion

        #region public methods

        //public static AmazonS3StorageProviderFile FromMetadata(MetadataCollection metaData)
        //{
        //    return (CloudStorageProviderFileBase.Create<AmazonS3StorageProviderFile>(
        //        "",
        //        false,
        //        "",
        //        null,
        //        new Nullable<DateTime>()));
        //}

        #endregion

    }

}

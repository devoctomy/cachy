using Dropbox.Api.Files;
using System;

namespace devoctomy.cachy.Framework.Data.Cloud
{

    public class DropboxStorageProviderFile : CloudStorageProviderFileBase
    {

        #region constructor / destructor

        public DropboxStorageProviderFile(string name,
            bool isFolder,
            string path,
            string hash,
            DateTime lastModified) : base(name, isFolder, path, hash, lastModified)
        {
        }

        #endregion

        #region public methods

        public static DropboxStorageProviderFile FromMetadata(Metadata metaData)
        {
            return (CloudStorageProviderFileBase.Create<DropboxStorageProviderFile>(
                metaData.Name,
                metaData.IsFolder,
                metaData.PathLower,
                metaData.IsFile ? metaData.AsFile.ContentHash : null,
                metaData.IsFile ? metaData.AsFile.ClientModified : new Nullable<DateTime>()));
        }

        #endregion

    }

}

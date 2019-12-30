using devoctomy.cachy.Framework.Native.IO.Interfaces;
using devoctomy.DFramework.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using devoctomy.DFramework.Core.SystemExtensions;
using static devoctomy.cachy.Framework.Native.IO.Common;

namespace cachy.UWP.IO
{

    public class NativeFileHandler : INativeFileHandler
    {

        #region public methods

        public async Task<bool> Exists(string fullPath)
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(fullPath);
                return (true);
            }
            catch(FileNotFoundException)
            {
                return (false);
            }
        }

        public async Task<byte[]> ReadAsync(string fullPath)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(fullPath);
            using (IRandomAccessStream input = await file.OpenReadAsync())
            {
                byte[] bytes = new byte[input.Size];
                await input.ReadAsync(bytes.AsBuffer(), (uint)input.Size, InputStreamOptions.None);
                return (bytes);
            }
        }

        public async Task WriteAsync(string fullPath, byte[] data)
        {
            string folderPath = fullPath.Substring(0, fullPath.LastIndexOf(DLoggerManager.PathDelimiter));
            StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            string filePath = fullPath.Substring(folderPath.Length + 1);
            StorageFile file = await folder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
            using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
            {
                await transaction.Stream.WriteAsync(data.AsBuffer());
                await transaction.CommitAsync();
            }
        }

        public async Task<string> HashFileAsync(
            HashStyle hashStyle,
            string fullPath)
        {
            switch(hashStyle)
            {
                case HashStyle.DropboxSHA256:
                    {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(fullPath);
                        using (IRandomAccessStream input = await file.OpenReadAsync())
                        {
                            List<byte[]> hashes = new List<byte[]>();

                            using (SHA256 crypto = SHA256.Create())
                            {
                                bool moreData = true;
                                while (moreData)
                                {
                                    byte[] bytes = new byte[4 * 1024 * 1024];
                                    IBuffer read = await input.ReadAsync(bytes.AsBuffer(), (uint)input.Size, InputStreamOptions.None);

                                    byte[] blockHash = crypto.ComputeHash(read.ToArray());
                                    hashes.Add(blockHash);

                                    moreData = !(read.Length < bytes.Length);
                                }

                                byte[] joined = hashes.SelectMany(byteArr => byteArr).ToArray();
                                byte[] totalHash = crypto.ComputeHash(joined);
                                string hex = BitConverter.ToString(totalHash).ToLower().Replace("-", String.Empty);
                                return (hex);
                            }
                        }
                    }
                case HashStyle.OneDriveSHA1:
                    {
                        string shaHash = String.Empty;
                        StorageFile file = await StorageFile.GetFileFromPathAsync(fullPath);
                        using (SHA1Managed shaForStream = new SHA1Managed())
                        using (Stream sourceFileStream = await file.OpenStreamForReadAsync())
                        using (Stream sourceStream = new CryptoStream(sourceFileStream, shaForStream, CryptoStreamMode.Read))
                        {
                            //!!!Could this be optimised?
                            while (sourceStream.ReadByte() != -1) ;
                            shaHash = shaForStream.Hash.ToHexString().ToUpper();
                        }
                        return (shaHash);
                    }
                case HashStyle.AmazonS3MD5:
                    {
                        string md5Hash = String.Empty;
                        StorageFile file = await StorageFile.GetFileFromPathAsync(fullPath);
                        using (MD5 md5ForStream = MD5.Create())
                        using (Stream sourceFileStream = await file.OpenStreamForReadAsync())
                        using (Stream sourceStream = new CryptoStream(sourceFileStream, md5ForStream, CryptoStreamMode.Read))
                        {
                            //!!!Could this be optimised?
                            while (sourceStream.ReadByte() != -1) ;
                            md5Hash = md5ForStream.Hash.ToHexString().ToUpper();
                        }
                        return (md5Hash);
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public async Task<KeyValuePair<string, Stream>?> PickFileForSave(
            string suggestedFileName,
            params KeyValuePair<string, string[]>[] extensions)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.SuggestedFileName = suggestedFileName;

            foreach(KeyValuePair<string, string[]> curPair in extensions)
            {
                savePicker.FileTypeChoices.Add(curPair.Key, curPair.Value.ToList());
            }

            StorageFile file = await savePicker.PickSaveFileAsync();
            return (file == null ? (KeyValuePair<string, Stream>?)null : new KeyValuePair<string, Stream>(file.Path, await file.OpenStreamForWriteAsync()));
        }

        #endregion

    }

}

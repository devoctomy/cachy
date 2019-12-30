using devoctomy.cachy.Framework.Native.IO.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using devoctomy.DFramework.Core.SystemExtensions;
using static devoctomy.cachy.Framework.Native.IO.Common;
using System.Security;

namespace cachy.Droid.IO
{

    public class NativeFileHandler : INativeFileHandler
    {

        #region public methods

        public Task<bool> Exists(string fullPath)
        {
            return Task<bool>.Run(() =>
            {
                return (File.Exists(fullPath));
            });
        }

        public Task<byte[]> ReadAsync(string fullPath)
        {
            return Task<byte[]>.Run(() =>
            {
                byte[] data = File.ReadAllBytes(fullPath);
                return (data);
            });
        }

        public Task WriteAsync(string fullPath, byte[] data)
        {
            return Task.Run(() =>
            {
                FileInfo file = new FileInfo(fullPath);
                if (!file.Directory.Exists) file.Directory.Create();
                File.WriteAllBytes(fullPath, data);
                return (true);
            });
        }

        public async Task<string> HashFileAsync(
            HashStyle hashStyle,
            string fullPath)
        {
            switch(hashStyle)
            {
                case HashStyle.DropboxSHA256:
                    {
                        using (Stream input = File.OpenRead(fullPath))
                        {
                            List<byte[]> hashes = new List<byte[]>();

                            using (SHA256 crypto = SHA256.Create())
                            {
                                bool moreData = true;
                                while (moreData)
                                {
                                    byte[] bytes = new byte[4 * 1024 * 1024];
                                    int read = await input.ReadAsync(bytes, 0, bytes.Length);

                                    byte[] blockHash = crypto.ComputeHash(bytes, 0, read);
                                    hashes.Add(blockHash);

                                    moreData = !(read < bytes.Length);
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
                        using (SHA1Managed shaForStream = new SHA1Managed())
                        using (Stream sourceFileStream = File.Open(fullPath, FileMode.Open))
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
                        using (MD5 md5ForStream = MD5.Create())
                        using (Stream sourceFileStream = File.Open(fullPath, FileMode.Open))
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
                        throw new NotImplementedException(String.Format("Hash style '{0}' is not supported by the native file handler.", hashStyle.ToString()));
                    }
            }
        }

        public Task<KeyValuePair<string, Stream>?> PickFileForSave(
            string suggestedFileName,
            params KeyValuePair<string, string[]>[] extensions)
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
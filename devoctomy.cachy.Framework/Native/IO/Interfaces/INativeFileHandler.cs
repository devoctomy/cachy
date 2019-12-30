using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static devoctomy.cachy.Framework.Native.IO.Common;

namespace devoctomy.cachy.Framework.Native.IO.Interfaces
{

    public interface INativeFileHandler
    {

        #region public methods

        Task<bool> Exists(string fullPath);

        Task<byte[]> ReadAsync(string fullPath);

        Task WriteAsync(
            string fullPath,
            byte[] data);

        Task<string> HashFileAsync(
            HashStyle hashStyle,
            string fullPath);

        Task<KeyValuePair<string, Stream>?> PickFileForSave(
            string suggestedFileName,
            params KeyValuePair<string, string[]>[] extensions);

        #endregion

    }

}

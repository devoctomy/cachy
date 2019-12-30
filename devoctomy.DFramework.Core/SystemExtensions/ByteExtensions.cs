using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace devoctomy.DFramework.Core.SystemExtensions
{

    public static class ByteExtensions
    {

        #region public methods

        public static string ToHexString(this byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        public static String ToPrettyString(this byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(Byte curByte in bytes)
            {
                stringBuilder.Append(String.Format("[{0}]", curByte));
                stringBuilder.Append(",");
            }
            stringBuilder.Length -= 1;
            return (stringBuilder.ToString());
        }

        public static byte[] AppendBytes(
            this byte[] bytes,
            params byte[][] appendBytes)
        {
            int totalLength = bytes.Length;
            foreach(byte[] append in appendBytes)
            {
                totalLength += append.Length;
            }
            byte[] total = new byte[totalLength];
            Buffer.BlockCopy(bytes, 0, total, 0, bytes.Length);
            int curIndex = bytes.Length;
            foreach (byte[] append in appendBytes)
            {
                Buffer.BlockCopy(append, 0, total, curIndex, append.Length);
                curIndex += append.Length;
            }
            return (total);
        }

        public static void RemoveFromEnd(
            this byte[] bytes,
            int length,
            out byte[] start,
            out byte[] end)
        {
            start = new byte[bytes.Length - length];
            end = new byte[length];
            Buffer.BlockCopy(bytes, 0, start, 0, start.Length);
            Buffer.BlockCopy(bytes, start.Length, end, 0, end.Length);
        }

        #endregion

    }

}

using System;
using System.Collections.ObjectModel;
using System.Text;

namespace cachywebfunctions.SystemExtensions
{

    public static class StringExtensions
    {

        #region public methods

        public static void AddToObvservableCollection(this String[] values,
            ObservableCollection<String> collection)
        {
            foreach(String curValue in values)
            {
                collection.Add(curValue);
            }
        }

        public static string ToHexString(this string str)
        {
            var sb = new StringBuilder();
            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }

        public static string StringFromHexString(this string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return Encoding.Unicode.GetString(bytes);
        }

        public static byte[] BytesFromHexString(this string hexString)
        {
            var bytes = new byte[hexString.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return bytes;
        }

        #endregion

    }

}

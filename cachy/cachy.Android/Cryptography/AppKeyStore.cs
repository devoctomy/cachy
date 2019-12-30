using System;
using System.Collections.Generic;
using System.IO;
using Android.Content;
using Android.Runtime;
using Java.Security;
using Java.Util;

namespace cachy.Droid.Cryptography
{
    public sealed class AppKeyStore : IDisposable
    {

        #region private constants

        private const string _fileName = "App.Accounts";
        private static char[] _password = null;

        #endregion

        #region private objects

        private static AppKeyStore _instance;
        private bool _disposed;
        private Context _context;
        private KeyStore _keyStore;
        private KeyStore.PasswordProtection _passProtection;
        private readonly object _fileLock = new object();
        private IntPtr id_load_Ljava_io_InputStream_arrayC;

        #endregion

        #region public properties

        public static AppKeyStore Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AppKeyStore();
                    _instance.CreateStore();
                }
                return (_instance);
            }
        }

        #endregion

        #region public methods

        public IEnumerable<string> FindAccountsForService(string serviceId)
        {
            List<string> r = new List<string>();
            string postfix = "-" + serviceId;
            IEnumeration aliases = _keyStore.Aliases();
            while (aliases.HasMoreElements)
            {
                string alias = aliases.NextElement().ToString();
                if (alias.EndsWith(postfix))
                {
                    KeyStore.SecretKeyEntry e = _keyStore.GetEntry(
                        alias, 
                        _passProtection) as KeyStore.SecretKeyEntry;
                    if (e != null)
                    {
                        byte[] bytes = e.SecretKey.GetEncoded();
                        string password = System.Text.Encoding.UTF8.GetString(bytes);
                        r.Add(password);
                    }
                }
            }
            return r;
        }

        public void Delete(string serviceId)
        {
            string alias = MakeAlias(serviceId);
            _keyStore.DeleteEntry(alias);
            Save();
        }

        public void Add(
            string secret, 
            string serviceId)
        {
            string alias = MakeAlias(serviceId);
            SecretAccount secretKey = new SecretAccount(secret);
            KeyStore.SecretKeyEntry entry = new KeyStore.SecretKeyEntry(secretKey);
            _keyStore.SetEntry(alias, entry, _passProtection);
            Save();
        }

        #endregion

        #region private methods

        private void Save()
        {
            lock (_fileLock)
            {
                using (Stream outputStream = _context.OpenFileOutput(_fileName, FileCreationMode.Private))
                {
                    _keyStore.Store(outputStream, _password);
                }
            }
        }

        private void CreateStore()
        {
            _context = Android.App.Application.Context;
            _keyStore = KeyStore.GetInstance(KeyStore.DefaultType);
            _passProtection = new KeyStore.PasswordProtection(_password);
            try
            {
                lock (_fileLock)
                {
                    using (var s = _context.OpenFileInput(_fileName))
                    {
                        _keyStore.Load(s, _password);
                    }
                }
            }
            catch (Java.IO.FileNotFoundException)
            {
                LoadEmptyKeyStore(_password);
            }
        }

        private static string MakeAlias(string serviceId)
        {
            return("-" + serviceId);
        }

        private void LoadEmptyKeyStore(char[] password)
        {
            if (id_load_Ljava_io_InputStream_arrayC == IntPtr.Zero)
            {
                id_load_Ljava_io_InputStream_arrayC = JNIEnv.GetMethodID(_keyStore.Class.Handle, "load", "(Ljava/io/InputStream;[C)V");
            }
            IntPtr intPtr = IntPtr.Zero;
            IntPtr intPtr2 = JNIEnv.NewArray(password);
            JNIEnv.CallVoidMethod(_keyStore.Handle, id_load_Ljava_io_InputStream_arrayC, new JValue[]
                {
                new JValue (intPtr),
                new JValue (intPtr2)
                });
            JNIEnv.DeleteLocalRef(intPtr);
            if (password != null)
            {
                JNIEnv.CopyArray(intPtr2, password);
                JNIEnv.DeleteLocalRef(intPtr2);
            }
        }

        #endregion

        #region idisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if(disposing)
            {
                if(_passProtection != null)
                {
                    _passProtection.Dispose();
                    _passProtection = null;
                }
                if(_keyStore != null)
                {
                    _keyStore.Dispose();
                    _keyStore = null;
                }
            }

            _disposed = true;
        }

        #endregion

    }

}
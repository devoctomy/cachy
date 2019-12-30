using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using cachy.Droid.Cryptography;
using cachy.Droid.Fixes;
using cachy.Droid.IO;
using cachy.Droid.Web;
using devoctomy.DFramework.Core.IO;
using devoctomy.DFramework.Logging;

namespace cachy.Droid
{

    [Activity(Label = "cachy", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        #region private constants

        private const Int32 ANDROID_PERMISSION_REQUESTCODE_WRITEEXTERNALSTORAGE = 1001;

        #endregion

        #region private objects

        private App _app;
        private Dictionary<Int32, Boolean> _permissionRequests;

        #endregion

        #region private methods

        private static Boolean ResolvePath(String token,
            out String resolvedPath)
        {
            switch (token)
            {
                case "{AppData}":
                    {
                        String appData = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath; ;
                        if (!appData.EndsWith("/")) appData += "/";
                        appData += "devoctomy/cachy";
                        resolvedPath = appData;
                        return (true);
                    }
                default:
                    {
                        throw new NotImplementedException(String.Format("Path token '{0}' has not been handled by the logging host.", token));
                    }
            }
        }

        private Boolean CheckPermissions()
        {
            if (!CheckStoragePermissions()) return (false);

            return (true);
        }

        private void RequestPermissions()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this,
                Manifest.Permission.WriteExternalStorage))
            {
                View view = FindViewById(Android.Resource.Id.Content);
                Snackbar.Make(view, "External storage access is required to store important cachy configuration data, and local vaults.", Snackbar.LengthIndefinite)
                    .SetAction("OK", v => ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage }, ANDROID_PERMISSION_REQUESTCODE_WRITEEXTERNALSTORAGE))
                    .Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this,
                        new string[] { Manifest.Permission.WriteExternalStorage },
                        ANDROID_PERMISSION_REQUESTCODE_WRITEEXTERNALSTORAGE);
            }
        }

        private void AndroidSetSoftInputMode(bool adjustResize)
        {
            Window.SetSoftInputMode(adjustResize ? Android.Views.SoftInput.AdjustResize : Android.Views.SoftInput.AdjustNothing);
            AndroidBug5497WorkaroundForXamarinAndroid.assistActivity(this);
        }

        private Boolean CheckStoragePermissions()
        {
            if(_permissionRequests.ContainsKey(ANDROID_PERMISSION_REQUESTCODE_WRITEEXTERNALSTORAGE))
            {
                return (_permissionRequests[ANDROID_PERMISSION_REQUESTCODE_WRITEEXTERNALSTORAGE]);
            }
            else
            {
                return (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage)
                        == Permission.Granted);
            }
        }

        #endregion

        #region base class overrides

        public override void OnRequestPermissionsResult(
            int requestCode, 
            string[] permissions, 
            [GeneratedEnum] Permission[] grantResults)
        {
            _permissionRequests[requestCode] = grantResults[0] == Permission.Granted;
            _app.PermissionsChanged();
        }

        protected override void OnCreate(Bundle bundle)
        {
            Directory.ResolvePath = ResolvePath;
            DLoggerManager.PathDelimiter = "/";

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            _permissionRequests = new Dictionary<Int32, Boolean>();

            devoctomy.cachy.Framework.Native.Native.FileHandler = new NativeFileHandler();
            devoctomy.cachy.Framework.Native.Native.PasswordVault = new NativeAndroidKeyStorePasswordVault();
            devoctomy.cachy.Framework.Native.Native.WebUtility = new NativeWebUtility();

            _app = new App(new Func<Boolean>(CheckPermissions),
                new Action(RequestPermissions),
                new Action<Boolean>(AndroidSetSoftInputMode));
            LoadApplication(_app);

            //!!!this should be put somewhere to disable screenshots
            //this.Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            if(hasFocus)
            {
                _app.AndroidGotFocus();
            }
            else
            {
                _app.AndroidLostFocus();
            }
        }

        #endregion

    }
}


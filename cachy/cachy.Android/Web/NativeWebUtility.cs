using Android.Webkit;
using devoctomy.cachy.Framework.Native.Web;
using System.Security;

namespace cachy.Droid.Web
{

    public class NativeWebUtility : INativeWebUtility
    {

        #region public methods

        public void ClearInAppBrowserCache()
        {
            var cookieManager = CookieManager.Instance;
            cookieManager.RemoveAllCookie();
        }

        //IOS
          //      NSHttpCookieStorage CookieStorage = NSHttpCookieStorage.SharedStorage;
          //  foreach (var cookie in CookieStorage.Cookies)
          //      CookieStorage.DeleteCookie(cookie);
          //}

        #endregion

    }

}
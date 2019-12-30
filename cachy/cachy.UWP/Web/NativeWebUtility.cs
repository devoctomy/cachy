using devoctomy.cachy.Framework.Native.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cachy.UWP.Web
{

    public class NativeWebUtility : INativeWebUtility
    {

        #region public methods

        public async void ClearInAppBrowserCache()
        {
            await Windows.UI.Xaml.Controls.WebView.ClearTemporaryWebDataAsync();
        }

        #endregion

    }

}

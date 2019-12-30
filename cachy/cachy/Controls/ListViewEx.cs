using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace cachy.Controls
{

    public class ListViewEx : ListView
    {

        #region bindable properties

        public static BindableProperty TagExProperty = BindableProperty.Create("TagEx",
            typeof(object),
            typeof(ListViewEx),
            null,
            BindingMode.Default);

        public static BindableProperty CachingStrategyProperty = BindableProperty.Create("CachingStrategyProperty",
            typeof(ListViewCachingStrategy),
            typeof(ListViewEx),
            ListViewCachingStrategy.RetainElement,
            BindingMode.Default);

        #endregion

        #region public properties

        public object TagEx
        {
            get
            {
                return (GetValue(TagExProperty));
            }
            set
            {
                SetValue(TagExProperty, value);
            }
        }

        #endregion

    }

}

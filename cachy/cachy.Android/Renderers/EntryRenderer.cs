using Android.Content;
using Android.Views.InputMethods;
using SoftInput.Droid.Render;
using System.Security;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entry), typeof(EntryRender))]
namespace SoftInput.Droid.Render
{

    public class EntryRender : EntryRenderer
    {

        public EntryRender(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.ImeOptions = (ImeAction)ImeFlags.NoExtractUi;
            }
        }
    }

}
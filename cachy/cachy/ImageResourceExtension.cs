using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cachy
{

    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {

        #region public properties

        public String Source { get; set; }

        #endregion

        #region public methods

        public Object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return(null);
            }
            ImageSource imageSource = ImageSource.FromResource(Source, typeof(ImageResourceExtension).Assembly);
            return (imageSource);
        }

        #endregion

    }

}

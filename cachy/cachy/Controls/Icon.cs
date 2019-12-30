using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.IO;
using Xamarin.Forms;

namespace cachy.Controls
{

    public class Icon : Frame
    {

        #region dependency properties

        public static readonly BindableProperty IconFilePathProperty = BindableProperty.Create(
            nameof(ResourceId),
            typeof(string),
            typeof(Icon),
            default(string),
            propertyChanged: RedrawCanvas);

        #endregion

        #region private objects

        private readonly SKCanvasView _canvasView;

        #endregion

        #region public properties

        public string ResourceId
        {
            get => (string)GetValue(IconFilePathProperty);
            set => SetValue(IconFilePathProperty, value);
        }

        #endregion

        #region constructror / destructor

        public Icon()
        {
            Padding = new Thickness(0);
            HasShadow = false;
            BackgroundColor = Color.Transparent;
            _canvasView = new SKCanvasView();
            _canvasView.IgnorePixelScaling = true;
            _canvasView.WidthRequest = WidthRequest;
            _canvasView.HeightRequest = HeightRequest;
            _canvasView.PaintSurface += CanvasViewOnPaintSurface;
            Content = _canvasView;
            SizeChanged += Icon_SizeChanged;
        }

        private void Icon_SizeChanged(object sender, EventArgs e)
        {
            _canvasView.InvalidateSurface();
        }

        #endregion

        #region dependency property events

        private static void RedrawCanvas(BindableObject bindable, object oldvalue, object newvalue)
        {
            Icon svgIcon = bindable as Icon;
            svgIcon?._canvasView.InvalidateSurface();
        }

        #endregion

        #region object events

        private void CanvasViewOnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;
            canvas.Clear();

            if (string.IsNullOrEmpty(ResourceId) && (Width > 0 && Height > 0))
                return;

            using (Stream stream = GetType().Assembly.GetManifestResourceStream(ResourceId))
            {
                if(stream != null)
                {
                    SKImageInfo info = args.Info;
                    float boundsWidth = (float)Width;
                    float boundsHeight = (float)Height;
                    float xRatio = (float)boundsWidth / info.Width;
                    float yRatio = (float)boundsHeight / info.Height;
                    float ratio = Math.Min(xRatio, yRatio);
                    float scaledX = info.Width * ratio;
                    float scaledY = info.Height * ratio;

                    SkiaSharp.Extended.Svg.SKSvg svg = new SkiaSharp.Extended.Svg.SKSvg(new SKSize((float)scaledX, (float)scaledY));
                    svg.Load(stream);
                    canvas.DrawPicture(svg.Picture);
                }
            }
        }

        #endregion

    }

}

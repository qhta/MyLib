using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qhta.Drawing.ArrayGraphics;
using Qhta.Drawing.ColorUtils;
using Qhta.WindowsMedia.ColorUtils;
using DrawingColor = System.Drawing.Color;

namespace Qhta.WPF.Controls
{
  public class BitmapRaster : FrameworkElement
  {
    #region Source property
    /// <summary>
    /// Bitmapa jako źródło i miejsce przechowywania danych. Może być typu <c>WriteableBitmap</c>
    /// </summary>
    public BitmapSource Source
    {
      get => (BitmapSource)GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    public static DependencyProperty SourceProperty = DependencyProperty.Register
      ("Source", typeof(BitmapSource), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Scale property
    /// <summary>
    /// Skala wyświetlania - rozmiar kwadratu na piksel
    /// </summary>
    public int Scale
    {
      get => (int)GetValue(ScaleProperty);
      set => SetValue(ScaleProperty, value);
    }

    public static DependencyProperty ScaleProperty = DependencyProperty.Register
      ("Scale", typeof(int), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure));
    #endregion

    #region ShowRaster property
    /// <summary>
    /// Czy pokazywać linie siatki. Linie pokazywane są w kolorze negatywowym względem koloru piksela.
    /// </summary>
    public bool ShowRaster
    {
      get => (bool)GetValue(ShowRasterProperty);
      set => SetValue(ShowRasterProperty, value);
    }

    public static DependencyProperty ShowRasterProperty = DependencyProperty.Register
      ("ShowRaster", typeof(bool), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region RasterThickness property
    /// <summary>
    /// Grubość linii rastra
    /// </summary>
    public double RasterThickness
    {
      get => (double)GetValue(RasterThicknessProperty);
      set => SetValue(RasterThicknessProperty, value);
    }

    public static DependencyProperty RasterThicknessProperty = DependencyProperty.Register
      ("RasterThickness", typeof(double), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Bounds measure
    /// <summary>
    /// Rozmiar kontrolki obliczany w oparciu o rozmiar obrazu i skalę
    /// </summary>
    public Rect Bounds
    {
      get
      {
        return new Rect(0, 0, imageSize.Width, imageSize.Height);
      }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      if (Source!=null)
        return imageSize = new Size(Source.PixelWidth*Scale, Source.PixelHeight*Scale);
      return imageSize;
    }

    /// <summary>
    /// Przychowywany rozmiar obrazu * skala
    /// </summary>
    Size imageSize = new Size(0,0);
    #endregion

    #region Get/SetPixelArray methods
    public static PixelArray GetPixelArray(BitmapSource source)
    {
      PixelArray pixels;
      int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);
      byte[] data = new byte[stride * source.PixelHeight];
      source.CopyPixels(data, stride, 0);
      pixels = new PixelArray(source.PixelWidth, source.PixelHeight);
      for (int y = 0; y < source.PixelHeight; y++)
        for (int x = 0; x < source.PixelWidth; x++)
        {
          int pixelAddress = y*stride+x*source.Format.BitsPerPixel/8;
          byte B = data[pixelAddress++];
          byte G = data[pixelAddress++];
          byte R = data[pixelAddress++];
          byte A = 0xFF;
          if (source.Format.BitsPerPixel == 32)
            A = data[pixelAddress++];
          pixels[x, y] = DrawingColor.FromArgb(A, R, G, B);
        }
      return pixels;
    }

    public static void SetPixelArray(WriteableBitmap target, PixelArray pixels)
    {
      int stride = target.PixelWidth * (target.Format.BitsPerPixel / 8);
      byte[] data = new byte[stride * target.PixelHeight];
      for (int y = 0; y < target.PixelHeight; y++)
        for (int x = 0; x < target.PixelWidth; x++)
        {
          int pixelAddress = y*stride+x*target.Format.BitsPerPixel/8;
          var pixel = pixels[x, y];
          data[pixelAddress++]=pixel.B;
          data[pixelAddress++]=pixel.G;
          data[pixelAddress++]=pixel.R;
          data[pixelAddress++]=pixel.A;
        }
      Int32Rect rect = new Int32Rect(0, 0, (int)target.PixelWidth, (int)target.PixelHeight);
      target.WritePixels(rect, data, stride, 0);
    }
    #endregion

    #region Render methods
    /// <summary>
    /// Metoda wyświetlenia
    /// </summary>
    /// <param name="drawingContext"></param>
    protected override void OnRender(DrawingContext drawingContext)
    {
      if (Source == null)
        return;
      RenderBitmap(drawingContext);
    }

    protected virtual void RenderBitmap(DrawingContext drawingContext)
    {
      //Debug.WriteLine($"BitmapRaster.RenderBitmap");
      var pixelWidth = Source.PixelWidth;
      var pixelHeight = Source.PixelHeight;
      int scale = Scale;
      double rasterThickness = RasterThickness;
      //Debug.WriteLine($"BitmapRaster rasterThickness={rasterThickness}");
      var pixels = GetPixelArray(Source);
      bool showRaster = ShowRaster && Scale>1;
      for (int y = 0; y < pixelHeight; y++)
        for (int x = 0; x < pixelWidth; x++)
        {
          var pixel = pixels[x, y];
          var brush = new SolidColorBrush(pixel.ToMediaColor());
          var pixelRect = new Rect(x*scale, y*scale, scale, scale);
          Pen pen = null; //new Pen(new SolidColorBrush(pixel.ToMediaColor()), 0.1);
          drawingContext.DrawRectangle(brush, pen, pixelRect);
          if (showRaster && rasterThickness>0)
          {
            var lineColor = pixels[x, y].Inverse();
            pen = new Pen(new SolidColorBrush(lineColor.ToMediaColor()), rasterThickness);
            drawingContext.DrawRectangle(null, pen, pixelRect);
          }
        }
    }
    #endregion

  }
}

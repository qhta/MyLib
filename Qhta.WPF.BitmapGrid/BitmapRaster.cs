using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qhta.WindowsMedia.ColorUtils;

namespace Qhta.WPF.Controls
{
  public class BitmapRaster: UIElement
  {
    public static DependencyProperty SourceProperty = DependencyProperty.Register
      ("Source", typeof(BitmapSource), typeof(BitmapRaster), 
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Edytowalna bitmapa. Utożsamiana z kontekstem danych.
    /// </summary>
    public BitmapSource Source
    {
      get => (BitmapSource)GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    public static DependencyProperty ScaleProperty = DependencyProperty.Register
      ("Scale", typeof(int), typeof(BitmapRaster), 
      new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsMeasure));

    /// <summary>
    /// Edytowalna bitmapa. Utożsamiana z kontekstem danych.
    /// </summary>
    public int Scale
    {
      get => (int)GetValue(ScaleProperty);
      set => SetValue(ScaleProperty, value);
    }

    public static DependencyProperty ShowRasterProperty = DependencyProperty.Register
      ("ShowRaster", typeof(bool), typeof(BitmapRaster),
      new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Edytowalna bitmapa. Utożsamiana z kontekstem danych.
    /// </summary>
    public bool ShowRaster
    {
      get => (bool)GetValue(ShowRasterProperty);
      set => SetValue(ShowRasterProperty, value);
    }

    public Rect Bounds
    {
      get
      {
        return new Rect(0, 0, imageSize.Width, imageSize.Height);
      }
    }

    protected override Size MeasureCore(Size availableSize)
    {
      if (Source!=null)
        return imageSize = new Size(Source.PixelWidth*Scale, Source.PixelHeight*Scale);
      return base.MeasureCore(availableSize);
    }

    Size imageSize = new Size(16,16);

    protected override void OnRender(DrawingContext drawingContext)
    {
      if (Source == null)
        return;
      //var pixelWidth = Source.PixelWidth;
      //var pixelHeight = Source.PixelHeight;
      //int scale = Scale;

      //var pixels = GetPixelArray(this.Source);
      //var bitmap = new WriteableBitmap(pixelWidth*scale, pixelHeight*scale, Source.DpiX, Source.DpiY, PixelFormats.Bgra32, Source.Palette);
      //SetPixelArray(bitmap, pixels, scale);
      //var rect = new Rect(imageSize);
      //drawingContext.DrawImage(bitmap, rect);
      RenderBitmap(drawingContext);
      //if (ShowRaster && Scale>1)
      //  RenderRaster(drawingContext);
    }

    Color[,] GetPixelArray(BitmapSource source)
    {
      Color[,] pixels;
      int stride = source.PixelWidth * (source.Format.BitsPerPixel / 8);
      byte[] data = new byte[stride * source.PixelHeight];
      source.CopyPixels(data, stride, 0);
      pixels = new Color[source.PixelWidth, source.PixelHeight];
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
          pixels[x, y] = Color.FromArgb(A, R, G, B);
        }
      return pixels;
    }

    void SetPixelArray(WriteableBitmap target, Color[,] pixels)
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

    void SetPixelArray(WriteableBitmap target, Color[,] pixels, int scale)
    {
      int pixelWidth = (pixels.GetUpperBound(0)+1);
      int pixelHeight = (pixels.GetUpperBound(1)+1);
      int newPixelWidth = pixelWidth*scale;
      int newPixelHeight = pixelHeight*scale;
      var scaledPixels = new Color[newPixelWidth, newPixelHeight];
      for (int y = 0; y < pixelHeight; y++)
        for (int x = 0; x < pixelWidth; x++)
          for (int i = 0; i<scale; i++)
            for (int j = 0; j<scale; j++)
              scaledPixels[x*scale+i, y*scale+j]=pixels[x, y];
      SetPixelArray(target, scaledPixels);
    }

    protected virtual void RenderBitmap(DrawingContext drawingContext)
    {
      var pixelWidth = Source.PixelWidth;
      var pixelHeight = Source.PixelHeight;
      int scale = Scale;
      var pixels = GetPixelArray(Source);
      bool showRaster = ShowRaster && Scale>1;
      for (int y = 0; y < pixelHeight; y++)
        for (int x = 0; x < pixelWidth; x++)
        {
          var brush = new SolidColorBrush(pixels[x, y]);
          drawingContext.DrawRectangle(brush, null, new Rect(x*scale, y*scale, scale, scale));
          if (showRaster)
          {
            var lineColor = Colors.Transparent;
            lineColor = pixels[x, y].Inverse();
            var pen = new Pen(new SolidColorBrush(lineColor), 1);
            drawingContext.DrawRectangle(null, pen, new Rect(x*scale, y*scale, scale, scale));
          }
        }
    }

    //protected virtual void RenderRaster(DrawingContext drawingContext)
    //{
    //  var pixelWidth = Source.PixelWidth;
    //  var pixelHeight = Source.PixelHeight;
    //  int scale = Scale;
    //  var pen = new Pen(Brushes.Gray, 1);
    //  for (int y = 0; y < pixelHeight+1; y++)
    //    drawingContext.DrawLine(pen, new Point(0, y*scale), new Point(pixelWidth*scale, y*scale));
    //  for (int x = 0; x < pixelWidth+1; x++)
    //    drawingContext.DrawLine(pen, new Point(x*scale, 0), new Point(x*scale, pixelHeight*scale));
    //}
  }
}

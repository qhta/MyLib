using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Qhta.WPF.Utils
{
  public static partial class ClipboardUtils
  {

    public static bool CopyToClipboard(UIElement element)
    {
      bool success = true;
      Clipboard.Clear();
      var wpfBitmap = MakeRenderTargetBitmap(element);
      PngBitmapEncoder encoder = new PngBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(wpfBitmap));
      var pngObject = new DataObject();
      using (Stream pngStream = new MemoryStream())
      {
        encoder.Save(pngStream);
        pngObject.SetData("PNG", pngStream);
        Clipboard.SetDataObject(pngObject, true);
      }
      return success;
    }

    private static RenderTargetBitmap MakeRenderTargetBitmap(UIElement element)
    {
      element.Measure(new System.Windows.Size(double.PositiveInfinity,
        double.PositiveInfinity));
      element.Arrange(new Rect(new System.Windows.Point(0, 0),
        element.DesiredSize));
      int w = (int)Math.Ceiling(element.RenderSize.Width);
      int h = (int)Math.Ceiling(element.RenderSize.Height);
      RenderTargetBitmap rtb = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
      System.Windows.Shapes.Rectangle fillBackground = new System.Windows.Shapes.Rectangle
      {
        Width = w,
        Height = h,
        Fill=new SolidColorBrush(System.Windows.Media.Color.FromArgb(0,255,255,255)),
        Opacity=1
      };
      fillBackground.Measure(new System.Windows.Size(double.PositiveInfinity,
        double.PositiveInfinity));
      fillBackground.Arrange(new Rect(new System.Windows.Point(0, 0),
        fillBackground.DesiredSize));
      rtb.Render(fillBackground);
      rtb.Render(element);
      return rtb;
    }

    private static string MakeRenderTargetXAML(UIElement element)
    {
      element.Measure(new System.Windows.Size(double.PositiveInfinity,
        double.PositiveInfinity));
      element.Arrange(new Rect(new System.Windows.Point(0, 0),
        element.DesiredSize));
      int w = (int)Math.Ceiling(element.RenderSize.Width);
      int h = (int)Math.Ceiling(element.RenderSize.Height);
      string result = XamlWriter.Save(element);
      return result;
    }

  }
}

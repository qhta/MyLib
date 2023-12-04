namespace Qhta.WPF.Utils;

/// <summary>
/// Helper class to handle a bitmap (get/set array of pixels).
/// </summary>
public static class ArrayGraphics
{
  /// <summary>
  /// Converts <see cref="BitmapSource"/> to array of pixels.
  /// </summary>
  /// <param name="source"></param>
  /// <returns></returns>
  public static Color[,] GetColorArray(this BitmapSource source)
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

  /// <summary>
  /// Sets array of pixels to <see cref="WriteableBitmap"/>
  /// </summary>
  /// <param name="target"></param>
  /// <param name="pixels"></param>
  public static void SetColorArray(this WriteableBitmap target, Color[,] pixels)
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
}


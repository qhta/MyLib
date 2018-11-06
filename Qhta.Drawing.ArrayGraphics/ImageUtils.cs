using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Qhta.Drawing
{
  public static class ImageUtils
  {

    public static PixelArray GetPixelArray(this Bitmap image)
    {
      var result = new PixelArray(image.Width, image.Height);
      for (int y = 0; y<image.Width; y++)
        for (int x = 0; x<image.Width; x++)
          result[x, y]=image.GetPixel(x, y);
      return result;
    }

    public static void SetPixelArray(this Bitmap image, PixelArray pixels)
    {
      for (int y = 0; y<image.Width; y++)
        for (int x = 0; x<image.Width; x++)
          image.SetPixel(x, y, pixels[x,y]);
    }
  }
}

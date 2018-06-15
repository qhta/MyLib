using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.WpfUtils
{
  public static class ArrayUtils
  {
    //bytes 84 21 ==> 0x8421 (BE) ==bin==> 1000 0100 0010 0001 ==split==> 10000 10000 10000 1 ==dec==> 16 16 16 1 (RGBA) ==adjust==> 128 128 128 255
    // values in constructor are: bytes per pixel, amount of bits and amount to shift for getting R, G, B and A components, and data endianness.
    private static PixelFormatter SixteenBppFormatter = new PixelFormatter(2, 5, 11, 5, 6, 5, 1, 1, 0, false);

    protected static Byte[] Convert16bTo32b(Byte[] imageData, Int32 startOffset, Int32 width, Int32 height, ref Int32 stride)
    {
      Int32 newImageStride = width * 4; ;
      Byte[] newImageData = new Byte[height * newImageStride];
      for (Int32 y = 0; y < height; y++)
      {
        for (Int32 x = 0; x < width; x++)
        {
          Int32 sourceOffset = y * stride + x * 2;
          Int32 targetOffset = y * newImageStride + x * 4;
          Color c = SixteenBppFormatter.GetColor(imageData, startOffset + sourceOffset);
          PixelFormatter.Format32BitArgb.WriteColor(newImageData, targetOffset, c);
        }
      }
      stride = newImageStride;
      return newImageData;
    }
  }
}

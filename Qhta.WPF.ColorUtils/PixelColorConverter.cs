using System.Windows.Media;
using Qhta.Drawing;
using DrawingColor = System.Drawing.Color;

namespace Qhta.WPF
{
  public static class PixelColorConverter
  {
    public static Pixel ToPixel(this Color value)
    {
      return new Pixel { A=value.A, R=value.R, G=value.G, B=value.B };
    }

    public static Color ToMediaColor(this Pixel value)
    {
      return Color.FromArgb(value.A, value.R, value.G, value.B);
    }
  }
}

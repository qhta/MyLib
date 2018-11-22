using System;
using System.Windows.Media;

namespace Qhta.WPF
{
  public static class ColorUtils
  {
    public static double Distance(this Color color1, Color color2)
    {
      var A1 = color1.A/255.0;
      var R1 = color1.R/255.0;
      var G1 = color1.G/255.0;
      var B1 = color1.B/255.0;
      var A2 = color2.A/255.0;
      var R2 = color2.R/255.0;
      var G2 = color2.G/255.0;
      var B2 = color2.B/255.0;
      var diff = Math.Sqrt((A1-A2)*(A1-A2) + (R1-R2)*(R1-R2) + (G1-G2)*(G1-G2) + (B1-B2)*(B1-B2));
      return diff;
    }

    public static Color Inverse(this Color color)
    {
      return Color.FromArgb(255, (byte)(255-color.R), (byte)(255-color.G), (byte)(255-color.B));
    }
  }
}

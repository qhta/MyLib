using System;
using System.Diagnostics;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF
{
  /// <summary>
  /// Some operation on colors.
  /// </summary>
  public static class ColorUtils
  {
    /// <summary>
    /// Distance between two colors treated as points in a 4-dimension color space (A, R, G, B).
    /// Each dimension has range from 0 to 1.
    /// Result is a square root of sum of squares of differences between point coordinates in each dimension.
    /// </summary>
    /// <param name="color1">First color</param>
    /// <param name="color2">Second color</param>
    /// <returns></returns>
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

    /// <summary>
    /// Inversion of color. The result is a complement in each dimension in RGB color space.
    /// Alpha channel is set to maximum opacity.
    /// If the <paramref name="contrast"> parameter causes conversion to HSV space and setting
    /// V value to 0 or 1 value, what makes the resulting color be in contrast to input color.
    /// </summary>
    /// <param name="color">Input color</param>
    /// <param name="contrast">If eqals <c>true</c>, then the V value of the resulted color 
    /// will be set to 0 or 1, depending on a threshold value (which is set to 0.4). 
    /// The A, H and S values remain intact.</param>
    /// <returns></returns>
    public static Color Inverse(this Color color, bool contrast=false)
    {
      if (contrast)
      {
        byte d = 0;

        double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B)/255;

        if (luminance > 0.51)
          d = 0; 
        else
          d = 255; 

        return Color.FromArgb(255, d, d, d);
      }
      return Color.FromArgb(255, (byte)(255-color.R), (byte)(255-color.G), (byte)(255-color.B));
    }

  }
}

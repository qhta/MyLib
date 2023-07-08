using System.Windows;
using System.Windows.Controls;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Helper class to measure string width.
  /// Used in <see cref="StringsWidthConverter"/>
  /// </summary>
  public static class MeasureUtils
  {
    /// <summary>
    /// Gets text width in pixels.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static double TextWidth(this string str)
    {
      TextBlock textBlock = new TextBlock();
      textBlock.Text = str;

      Size size = ShapeMeasure(textBlock);
      return size.Width;
    }

    /// <summary>
    /// Gets text width and height.
    /// </summary>
    /// <param name="tb"></param>
    /// <returns></returns>
    public static Size ShapeMeasure(TextBlock tb)
    {
      Size maxSize = new Size(
           double.PositiveInfinity,
           double.PositiveInfinity);
      tb.Measure(maxSize);
      return tb.DesiredSize;
    }
  }
}

using DrawingColor = System.Drawing.Color;

namespace Qhta.Drawing.ColorUtils
{
  public static class ColorComparer
  {
    /// <summary>
    /// Precise comparison based on ARGB value
    /// </summary>
    /// <param name="first">first DrawingColor</param>
    /// <param name="other">other DrawingColor</param>
    /// <returns></returns>
    public static bool IsEqual(this DrawingColor first, DrawingColor other)
    {
      return first.ToArgb()==other.ToArgb();
    }
  }
}

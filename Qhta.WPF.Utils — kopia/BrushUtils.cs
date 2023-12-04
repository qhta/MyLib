namespace Qhta.WPF.Utils;

/// <summary>
/// Brush utility class that checks if the brush is null or empty.
/// </summary>
public static class BrushUtils
{
  /// <summary>
  /// Checks if the brush is null or empty. 
  /// "Empty" brush is a solid color brush with Alpha component set to 0.
  /// </summary>
  /// <param name="brush"></param>
  /// <returns></returns>
  public static bool IsNullOrEmpty(this Brush brush)
  {
    if (brush==null)
      return true;
    if (brush is SolidColorBrush solidColorBrush)
      return solidColorBrush.Color.A==0;
    return false;
  }
}

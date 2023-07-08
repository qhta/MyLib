using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public static class BrushUtils
  {
    public static bool IsNullOrEmpty(this Brush brush)
    {
      if (brush==null)
        return true;
      if (brush is SolidColorBrush solidColorBrush)
        return solidColorBrush.Color.A==0;
      return false;
    }
  }
}

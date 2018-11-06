using System.Windows.Media;
using DrawingColor = System.Drawing.Color;

namespace Qhta.WPF
{

  public static class DrawingColorConverter
  {
    public static DrawingColor ToDrawingColor(this Color value)
    {
      return DrawingColor.FromArgb(value.A, value.R, value.G, value.B);
    }

    public static Color ToMediaColor(this DrawingColor value)
    {
      return Color.FromArgb(value.A, value.R, value.G, value.B);
    }

  }
}

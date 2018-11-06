using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qhta.WPF;

namespace Qhta.Drawing
{
  public static class DrawingBrushConverter
  {
    public static System.Drawing.Brush ToDrawingBrush(this System.Windows.Media.Brush brush)
    {
      if (brush is System.Windows.Media.SolidColorBrush solidBrush)
        return new System.Drawing.SolidBrush(solidBrush.Color.ToDrawingColor());
      throw new NotImplementedException("Only SolidColorBrush can be converted to DrawingColor");
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Qhta.WPF.Utils
{
  public static class MeasureUtils
  {
    public static double TextWidth(this string str)
    {
      TextBlock textBlock = new TextBlock();
      textBlock.Text = str;

      Size size = ShapeMeasure(textBlock);
      return size.Width;
    }

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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Qhta.WPF.IconDefinition
{
  public class Rectangle : Shape
  {
    protected override Qhta.Drawing.Shape DrawingShape { get; set; } = new Qhta.Drawing.Rectangle();

    #region WPF drawing
    protected override void DrawShape(System.Windows.Media.DrawingContext context,
      System.Windows.Media.Brush brush, System.Windows.Media.Pen pen, double left, double top, double width, double height)
    {
        context.DrawRectangle(brush, pen, new System.Windows.Rect(left, top, width, height));
    }
    #endregion

    #region Geometry 
    public override Geometry GetGeometry(double left, double top, double width, double height)
    {
      return new RectangleGeometry(new System.Windows.Rect(left, top, width, height));
    }
    #endregion
  }
}

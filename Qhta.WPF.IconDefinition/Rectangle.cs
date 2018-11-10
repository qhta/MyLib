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

    protected override void DrawShape(System.Windows.Media.DrawingContext context,
      System.Windows.Media.Brush brush, System.Windows.Media.Pen pen, double left, double top, double width, double height)
    {
      if (StrokeDashArray!=null && StrokeDashArray.Count()>1)
      {
        if (brush!=null)
        context.DrawRectangle(brush, null, new System.Windows.Rect(left, top, width, height));
        double right = left+width;
        double bottom = top+height;
        pen.StartLineCap = (System.Windows.Media.PenLineCap)StrokeStartLineCap;
        pen.EndLineCap = (System.Windows.Media.PenLineCap)StrokeEndLineCap;
        pen.DashCap = (System.Windows.Media.PenLineCap)StrokeDashCap;
        double notNaN = double.IsNaN(StrokeDashOffset) ? 0 : StrokeDashOffset;
        var dashStyle = new System.Windows.Media.DashStyle { Dashes = StrokeDashArray, Offset = notNaN };
        pen.DashStyle = dashStyle;
        context.DrawLine(pen, 
          new System.Windows.Point(left, top),
          new System.Windows.Point(right, top));
        context.DrawLine(pen,
          new System.Windows.Point(left, bottom),
          new System.Windows.Point(right, bottom));
        context.DrawLine(pen,
          new System.Windows.Point(right, top),
          new System.Windows.Point(right, bottom));
        context.DrawLine(pen,
          new System.Windows.Point(left, top),
          new System.Windows.Point(left, bottom));
      }
      else
      {
        context.DrawRectangle(brush, pen, new System.Windows.Rect(left, top, width, height));
      }
    }
  }
}

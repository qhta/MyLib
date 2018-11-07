using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.WPF
{
  public class Rectangle : Shape
  {
    //Qhta.Drawing.Rectangle DrawingRectangle = new Qhta.Drawing.Rectangle();

    protected override void FillInterior(Graphics graphics, Brush brush, float left, float top, float width, float height)
    {
      graphics.FillRectangle(brush, left, top, width, height);
    }

    protected override void DrawOutline(Graphics graphics, Pen pen, float left, float top, float width, float height)
    {
      if (StrokeDashArray!=null)
      {
        float right = left+width;
        float bottom = top+height;
        left += pen.Width/2;
        top += pen.Width/2;
        right -= pen.Width/2;
        bottom -= pen.Width/2;
        pen.DashOffset = (float)StrokeDashOffset;
        pen.StartCap = (LineCap)StrokeStartLineCap;
        pen.EndCap = (LineCap)StrokeEndLineCap;
        graphics.DrawLine(pen, left, top, right, top);
        graphics.DrawLine(pen, right, top, right, bottom);
        graphics.DrawLine(pen, left, bottom, right, bottom);
        graphics.DrawLine(pen, left, top, left, bottom);
      }
      else
        graphics.DrawRectangle(pen, left, top, width, height);
    }
  }
}

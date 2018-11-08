using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Qhta.Drawing
{
  public class Rectangle : Shape
  {
    public override void FillInterior(DrawingContext context, Brush brush, float left, float top, float width, float height)
    {
      context.Graphics.FillRectangle(brush, left, top, width, height);
    }

    public override void DrawOutline(DrawingContext context, Pen pen, float left, float top, float width, float height)
    {
      if (StrokeDashArray!=null && StrokeDashArray.Count()>1)
      {
        float right = left+width;
        float bottom = top+height;
        left += pen.Width/2;
        top += pen.Width/2;
        right -= pen.Width/2;
        bottom -= pen.Width/2;
        pen.StartCap = (LineCap)StrokeStartCap;
        pen.EndCap = (LineCap)StrokeEndCap;
        pen.DashCap = (DashCap)StrokeDashCap;
        double notNaN = double.IsNaN(StrokeDashOffset) ? 0 : StrokeDashOffset;
        float dashOffsetX = (float)notNaN;
        float dashOffsetY = (float)notNaN;
        pen.DashOffset = dashOffsetX;
        context.Graphics.DrawLine(pen, left, top, right, top);
        context.Graphics.DrawLine(pen, left, bottom, right, bottom);
        pen.DashOffset = dashOffsetY;
        context.Graphics.DrawLine(pen, right, top, right, bottom);
        context.Graphics.DrawLine(pen, left, top, left, bottom);
      }
      else
      {
        context.Graphics.DrawRectangle(pen, left, top, width, height);
      }
    }

  }
}

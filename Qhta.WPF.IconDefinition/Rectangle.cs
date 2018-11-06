using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.WPF
{
  public class Rectangle : ClosedFigure
  {
    Qhta.Drawing.Rectangle DrawingRectangle = new Qhta.Drawing.Rectangle();

    protected override void DrawOutline(Graphics graphics, Pen pen, float left, float top, float width, float height)
    {
      DrawingRectangle.DrawOutline(graphics, pen, left, top, width, height);
    }

    protected override void FillInterior(Graphics graphics, Brush brush, float left, float top, float width, float height)
    {
      DrawingRectangle.FillInterior(graphics, brush, left, top, width, height);
    }
  }
}

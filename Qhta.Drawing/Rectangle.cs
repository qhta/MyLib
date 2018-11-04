using System.Drawing;

namespace Qhta.Drawing
{
  public class Rectangle : ClosedFigure
  {
    protected override void FillInterior(Graphics graphics, Brush brush, float left, float top, float width, float height)
    {
      graphics.FillRectangle(brush, left, top, width, height);
    }

    protected override void DrawOutline(Graphics graphics, Pen pen, float left, float top, float width, float height)
    {
      graphics.DrawRectangle(pen, left, top, width, height);
    }

  }
}

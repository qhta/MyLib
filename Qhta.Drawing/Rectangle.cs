using System.Drawing;

namespace Qhta.Drawing
{
  public class Rectangle : Shape
  {
    protected override void FillInterior(Graphics graphics, Brush brush, float left, float top, float width, float height)
    {
      graphics.FillRectangle(brush, left, top, width, height);
    }

    protected override void DrawOutline(Graphics graphics, Pen pen, float left, float top, float width, float height)
    {
      //if (pen.DashPattern!=null)
      //{
      float right = left+width;
      float bottom = top+height;
      graphics.DrawLine(pen, left, top, right, top);
      graphics.DrawLine(pen, right, top, right, bottom);
      graphics.DrawLine(pen, left, bottom, right, bottom);
      graphics.DrawLine(pen, left, top, left, bottom);
      //}
      //else
      //  graphics.DrawRectangle(pen, left, top, width, height);
    }

  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.Drawing
{
  public class DrawingImage : DrawingItem
  {
    public System.Drawing.Image Image;

    public override void Draw(DrawingContext context)
    {
      var graphics = context.Graphics;
      var left = (float)context.TransformX(this.Left);
      var top = (float)context.TransformY(this.Top);
      var width = (float)context.TransformX(this.Width);
      var height = (float)context.TransformY(this.Height);
      graphics.DrawImage(Image, left, top, width, height);
    }
  }
}

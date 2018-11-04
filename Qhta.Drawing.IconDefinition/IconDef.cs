using System.Collections.Generic;
using System.Drawing;

namespace Qhta.Drawing
{
  public class IconDef
  {
    public Drawing Drawing { get; set; }
    public Dictionary<int, Bitmap> Images => Images;
    private Dictionary<int, Bitmap> _Images = new Dictionary<int, Bitmap>();

    public PixelArray GetPixelArray(int size)
    {
      if (Images.TryGetValue(size, out var image))
        return image.ToPixelArray();

      var bitmap = new Bitmap(size, size);
      using (var drawingContext = new DrawingContext
                                  {
                                    Graphics = Graphics.FromImage(bitmap),
                                    ScaleX = Drawing.Width/size,
                                    ScaleY = Drawing.Height/size,
                                  })
      {
        Drawing.Draw(drawingContext);
        _Images.Add(size, bitmap);
        return bitmap.ToPixelArray();
      }
    }
  }
}

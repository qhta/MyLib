using System.Collections.Generic;
using System.Windows.Markup;

namespace Qhta.Drawing
{
  [ContentProperty("Items")]
  public class Drawing: DrawingItem
  {
    public ICollection<DrawingItem> Items => _Items;
    List<DrawingItem> _Items = new List<DrawingItem>();

    public override void Draw(DrawingContext context)
    {
      var newContext = new DrawingContext
      {
        Graphics=context.Graphics,
        OffsetX=context.TransformX(Left),
        OffsetY=context.TransformY(Top),
      };
      foreach (var item in Items)
        item.Draw(newContext);

    }
  }
}

using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using DrawingContext = Qhta.Drawing.DrawingContext;

namespace Qhta.WPF.IconDefinition
{
  [ContentProperty("Items")]
  public class Drawing: FrameworkElement
  {
    public DrawingItemsCollection Items { get; set; } = new DrawingItemsCollection();

    public void Draw(DrawingContext context)
    {
      var newContext = new DrawingContext
      {
        Graphics=context.Graphics,
        //OffsetX=context.TransformX(Left),
        //OffsetY=context.TransformY(Top),
        ScaleX = context.ScaleX,
        ScaleY = context.ScaleY,
      };
      foreach (var item in Items)
        item.Draw(newContext);
    }

    public void Invalidate()
    {
      //base.Invalidate();
      if (Items!=null)
        foreach (var item in Items)
          item.Invalidate();
    }
  }
}

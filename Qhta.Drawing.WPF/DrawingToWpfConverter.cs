using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Qhta.WPF;
using Win = System.Windows;
using Wpf = System.Windows.Media;

namespace Qhta.Drawing.WPF
{
  public class DrawingToWpfConverter
  {
    public Wpf.DrawingGroup Convert(Drawing drawing)
    {
      Wpf.DrawingGroup result = new Wpf.DrawingGroup();
      foreach (var item in drawing.Items)
      {
        var newItem = Convert(item);
        if (newItem!=null)
        {
          result.Children.Add(newItem);
        }
      }
      return result;
    }

    public Wpf.Drawing Convert(DrawingItem item)
    {
      if (item is Rectangle rectangle)
        return Convert(rectangle);
      if (item is Drawing drawing)
        return Convert(drawing);
      return null;
    }

    public Wpf.Drawing Convert(Rectangle item)
    {
      Wpf.GeometryDrawing result = new Wpf.GeometryDrawing();
      if (item.Fill!=null)
        result.Brush = Convert(item.Fill);
      if (item.Outline!=null)
        result.Pen = Convert(item.Outline);
      result.Geometry = new Wpf.RectangleGeometry(new Win.Rect(item.Left, item.Top, item.Width, item.Height));
      return result;
    }

    public Wpf.Brush Convert(Brush brush)
    {
      if (brush is SolidBrush solidBrush)
        return Convert(solidBrush);
      throw new InvalidOperationException("Only SolidBrush allowed in drawing");
    }

    public Wpf.SolidColorBrush Convert(SolidBrush brush)
    {
      Wpf.SolidColorBrush result = new Wpf.SolidColorBrush(brush.Color.ToMediaColor());
      return result;
    }

    public Wpf.Pen Convert(Pen pen)
    {
      if (pen.PenType == PenType.SolidColor)
        return new Wpf.Pen(new Wpf.SolidColorBrush(pen.Color.ToMediaColor()),pen.Width);
      throw new InvalidOperationException("Only SolidColor Pen allowed in drawing");
    }

  }
}

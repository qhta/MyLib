using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using BrushConverter = Qhta.Drawing.DrawingBrushConverter;
using DrawingContext = Qhta.Drawing.DrawingContext;

namespace Qhta.WPF
{
  public abstract class ClosedFigure: DrawingItem
  {
    #region Fill property
    public Brush Fill
    {
      get => (Brush)GetValue(FillProperty);
      set => SetValue(FillProperty, value);
    }
    public static DependencyProperty FillProperty = DependencyProperty.Register
      ("Fill", typeof(Brush), typeof(DrawingItem),
       new PropertyMetadata(Brushes.Black));
    #endregion

    #region Stroke property
    public Brush Stroke
    {
      get => (Brush)GetValue(StrokeProperty);
      set => SetValue(StrokeProperty, value);
    }
    public static DependencyProperty StrokeProperty = DependencyProperty.Register
      ("Stroke", typeof(Brush), typeof(DrawingItem),
       new PropertyMetadata(Brushes.Black));
    #endregion

    public override void Draw(DrawingContext context)
    {
      var graphics = context.Graphics;
      var left = (float)context.TransformX(this.Left);
      var top = (float)context.TransformY(this.Top);
      var width = (float)context.TransformX(this.Width);
      var height = (float)context.TransformY(this.Height);
      if (Fill!=null && Fill!=Brushes.Transparent)
      {
        var brush = BrushConverter.ToDrawingBrush(Fill);
        FillInterior(graphics, brush, left, top, width, height);
      }
      if (Stroke!=null)
      {
        var lineWidth = 1.0;
        lineWidth=(float)context.ScaleXY(lineWidth);
        var outlineBrush = BrushConverter.ToDrawingBrush(Stroke) as System.Drawing.SolidBrush;
        var pen = new System.Drawing.Pen(outlineBrush.Color, (float)lineWidth);
        DrawOutline(graphics, pen, left, top, width, height);
      }
    }

    protected abstract void FillInterior(System.Drawing.Graphics graphics, System.Drawing.Brush brush, float left, float top, float width, float height);


    protected abstract void DrawOutline(System.Drawing.Graphics graphics, System.Drawing.Pen pen, float left, float top, float width, float height);

    public override void Invalidate()
    {
      base.Invalidate();
      BindingOperations.GetBindingExpressionBase(this, ClosedFigure.FillProperty)?.UpdateTarget();
      BindingOperations.GetBindingExpressionBase(this, ClosedFigure.StrokeProperty)?.UpdateTarget();
      //Debug.WriteLine($"Fill={Fill}");
    }
  }
}

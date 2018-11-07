using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using BrushConverter = Qhta.Drawing.DrawingBrushConverter;
using DrawingContext = Qhta.Drawing.DrawingContext;

namespace Qhta.WPF
{
  public abstract class Shape: DrawingItem
  {
    #region Fill property
    public Brush Fill
    {
      get => (Brush)GetValue(FillProperty);
      set => SetValue(FillProperty, value);
    }
    public static readonly DependencyProperty FillProperty = DependencyProperty.Register
      ("Fill", typeof(Brush), typeof(DrawingItem),
       new PropertyMetadata(null));
    #endregion

    #region Stroke property
    public Brush Stroke
    {
      get => (Brush)GetValue(StrokeProperty);
      set => SetValue(StrokeProperty, value);
    }
    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register
      ("Stroke", typeof(Brush), typeof(DrawingItem),
       new PropertyMetadata(null));
    #endregion

    #region StrokeThickness property
    [TypeConverter(typeof(LengthConverter))]
    public double StrokeThickness
    {
      get =>
          ((double)base.GetValue(StrokeThicknessProperty));
      set =>
          base.SetValue(StrokeThicknessProperty, value);
    }

    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register
      ("StrokeThickness", typeof(double), typeof(Shape), new PropertyMetadata(1.0));
    #endregion

    #region StrokeStartLineCap property
    public PenLineCap StrokeStartLineCap
    {
      get =>
          ((PenLineCap)base.GetValue(StrokeStartLineCapProperty));
      set =>
          base.SetValue(StrokeStartLineCapProperty, value);
    }

    public static readonly DependencyProperty StrokeStartLineCapProperty = DependencyProperty.Register
      ("StrokeStartLineCap", typeof(PenLineCap), typeof(Shape),
      new PropertyMetadata(PenLineCap.Flat));

    #endregion

    #region StrokeEndLineCap property
    public PenLineCap StrokeEndLineCap
    {
      get =>
          ((PenLineCap)base.GetValue(StrokeEndLineCapProperty));
      set =>
          base.SetValue(StrokeEndLineCapProperty, value);
    }

    public static readonly DependencyProperty StrokeEndLineCapProperty = DependencyProperty.Register
      ("StrokeEndLineCap", typeof(PenLineCap), typeof(Shape),
      new PropertyMetadata(PenLineCap.Flat));
    #endregion

    #region StrokeDashCap property
    public PenLineCap StrokeDashCap
    {
      get =>
          ((PenLineCap)base.GetValue(StrokeDashCapProperty));
      set =>
          base.SetValue(StrokeDashCapProperty, value);
    }

    public static readonly DependencyProperty StrokeDashCapProperty = DependencyProperty.Register
      ("StrokeDashCap", typeof(PenLineCap), typeof(Shape),
      new PropertyMetadata(PenLineCap.Flat));
    #endregion

    #region StrokeLineJoin property
    public PenLineJoin StrokeLineJoin
    {
      get =>
          ((PenLineJoin)base.GetValue(StrokeLineJoinProperty));
      set =>
          base.SetValue(StrokeLineJoinProperty, value);
    }

    public static readonly DependencyProperty StrokeLineJoinProperty = DependencyProperty.Register
      ("StrokeLineJoin", typeof(PenLineJoin), typeof(Shape),
      new PropertyMetadata(PenLineJoin.Miter));
    #endregion

    #region StrokeMiterLimit
    public double StrokeMiterLimit
    {
      get =>
          ((double)base.GetValue(StrokeMiterLimitProperty));
      set =>
          base.SetValue(StrokeMiterLimitProperty, value);
    }

    public static readonly DependencyProperty StrokeMiterLimitProperty = DependencyProperty.Register
      ("StrokeMiterLimit", typeof(double), typeof(Shape),
      new PropertyMetadata(10.0));
    #endregion

    #region StrokeDashOffset
    public double StrokeDashOffset
    {
      get =>
          ((double)base.GetValue(StrokeDashOffsetProperty));
      set =>
          base.SetValue(StrokeDashOffsetProperty, value);
    }

    public static readonly DependencyProperty StrokeDashOffsetProperty = DependencyProperty.Register
      ("StrokeDashOffset", typeof(double), typeof(Shape),
      new PropertyMetadata(0.0));
    #endregion

    #region StrokeDashArray property
    public DoubleCollection StrokeDashArray
    {
      get =>
          ((DoubleCollection)base.GetValue(StrokeDashArrayProperty));
      set =>
          base.SetValue(StrokeDashArrayProperty, value);
    }

    public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register
      ("StrokeDashArray", typeof(DoubleCollection), typeof(Shape),
      new PropertyMetadata(null));
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
        if (StrokeDashArray!=null)
        {
          pen.DashPattern = StrokeDashArray.Select(item => (float)item).ToArray();
          pen.DashOffset = (float)StrokeDashOffset;
        }
        DrawOutline(graphics, pen, left, top, width, height);
      }
    }

    protected abstract void FillInterior(System.Drawing.Graphics graphics, System.Drawing.Brush brush, float left, float top, float width, float height);


    protected abstract void DrawOutline(System.Drawing.Graphics graphics, System.Drawing.Pen pen, float left, float top, float width, float height);

    public override void Invalidate()
    {
      base.Invalidate();
      BindingOperations.GetBindingExpressionBase(this, Shape.FillProperty)?.UpdateTarget();
      BindingOperations.GetBindingExpressionBase(this, Shape.StrokeProperty)?.UpdateTarget();
      //Debug.WriteLine($"Fill={Fill}");
    }
  }
}

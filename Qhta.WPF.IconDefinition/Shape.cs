using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using BrushConverter = Qhta.Drawing.DrawingBrushConverter;
using DrawingContext = Qhta.Drawing.DrawingContext;
using PenAlignment = System.Drawing.Drawing2D.PenAlignment;
using LineJoin = System.Drawing.Drawing2D.LineJoin;
using LineCap = System.Drawing.Drawing2D.LineCap;
using DashCap = System.Drawing.Drawing2D.DashCap;
using Qhta.WPF.Utils;

namespace Qhta.WPF.IconDefinition
{
  public abstract class Shape : DrawingItem
  {
    #region Fill property
    public Brush Fill
    {
      get => (Brush)GetValue(FillProperty);
      set => SetValue(FillProperty, value);
    }
    public static readonly DependencyProperty FillProperty = DependencyProperty.Register
      ("Fill", typeof(Brush), typeof(Shape),
       new PropertyMetadata(null, DependencyPropertyChanged));
    #endregion

    #region Stroke property
    public Brush Stroke
    {
      get => (Brush)GetValue(StrokeProperty);
      set => SetValue(StrokeProperty, value);
    }
    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register
      ("Stroke", typeof(Brush), typeof(DrawingItem),
       new PropertyMetadata(null, DependencyPropertyChanged));
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
      ("StrokeThickness", typeof(double), typeof(Shape), 
      new PropertyMetadata(1.0, DependencyPropertyChanged));
    #endregion

    #region StrokePenAlignment property
    public PenAlignment StrokePenAlignment
    {
      get =>
          ((PenAlignment)base.GetValue(StrokePenAlignmentProperty));
      set =>
          base.SetValue(StrokePenAlignmentProperty, value);
    }

    public static readonly DependencyProperty StrokePenAlignmentProperty = DependencyProperty.Register
      ("StrokePenAlignment", typeof(PenAlignment), typeof(Shape), 
      new PropertyMetadata(PenAlignment.Center, DependencyPropertyChanged));
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
      new PropertyMetadata(PenLineCap.Flat, DependencyPropertyChanged));

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
      new PropertyMetadata(PenLineCap.Flat, DependencyPropertyChanged));
    #endregion

    #region StrokeDashCap property
    public DashCap StrokeDashCap
    {
      get =>
          ((DashCap)base.GetValue(StrokeDashCapProperty));
      set =>
          base.SetValue(StrokeDashCapProperty, value);
    }

    public static readonly DependencyProperty StrokeDashCapProperty = DependencyProperty.Register
      ("StrokeDashCap", typeof(DashCap), typeof(Shape),
      new PropertyMetadata(DashCap.Flat, DependencyPropertyChanged));
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
      new PropertyMetadata(PenLineJoin.Miter, DependencyPropertyChanged));
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
      new PropertyMetadata(10.0, DependencyPropertyChanged));
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
      ("StrokeDashOffset", typeof(double?), typeof(Shape),
      new PropertyMetadata(0.0, DependencyPropertyChanged));
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
      new PropertyMetadata(null, DependencyPropertyChanged));
    #endregion

    #region Invalidate
    public override void InvalidateBindings()
    {
      base.InvalidateBindings();
      BindingOperations.GetBindingExpressionBase(this, Shape.FillProperty)?.UpdateTarget();
      BindingOperations.GetBindingExpressionBase(this, Shape.StrokeProperty)?.UpdateTarget();
    }
    #endregion

    #region Graphic drawing
    protected abstract Qhta.Drawing.Shape DrawingShape { get; set; }

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
        FillInterior(context, brush, left, top, width, height);
      }
      if (Stroke!=null)
      {
        var lineWidth = StrokeThickness;
        lineWidth=(float)context.ScaleXY(lineWidth);
        var outlineBrush = BrushConverter.ToDrawingBrush(Stroke) as System.Drawing.SolidBrush;
        var pen = new System.Drawing.Pen(outlineBrush.Color, (float)lineWidth);
        if (StrokeDashArray!=null)
        {
          DrawingShape.StrokeDashArray=StrokeDashArray.ToArray();
          DrawingShape.StrokeDashOffset=StrokeDashOffset;
          pen.DashPattern = StrokeDashArray.Select(item => (float)item).ToArray();
          double notNaN = double.IsNaN(StrokeDashOffset) ? 0 : StrokeDashOffset;
          pen.DashOffset = (float)notNaN;
          pen.DashCap = DrawingShape.StrokeDashCap = (DashCap)StrokeDashCap;
        }
        pen.StartCap = DrawingShape.StrokeStartCap = (LineCap)StrokeStartLineCap;
        pen.EndCap = DrawingShape.StrokeEndCap = (LineCap)StrokeEndLineCap;
        pen.LineJoin = DrawingShape.StrokeLineJoin = (LineJoin)StrokeLineJoin;
        pen.MiterLimit = (float)(DrawingShape.StrokeMiterLimit = StrokeMiterLimit);
        pen.Alignment = DrawingShape.StrokePenAlignment = StrokePenAlignment;

        DrawOutline(context, pen, left, top, width, height);
      }
    }

    protected virtual void FillInterior(DrawingContext context, System.Drawing.Brush brush, float left, float top, float width, float height)
    {
      DrawingShape.FillInterior(context, brush, left, top, width, height);
    }


    protected virtual void DrawOutline(DrawingContext context, System.Drawing.Pen pen, float left, float top, float width, float height)
    {
      DrawingShape.DrawOutline(context, pen, left, top, width, height);
    }
    #endregion

    #region WPF drawing
    public override void Draw(System.Windows.Media.DrawingContext context)
    {
      var left = this.Left;
      var top = this.Top;
      var width = this.Width;
      var height = this.Height;
      var brush = Fill;
      var lineWidth = StrokeThickness;
      Pen pen = null;
      if (Stroke!=null)
      {
        pen = new Pen(Stroke, lineWidth);
        if (StrokeDashArray!=null)
        {
          double notNaN = double.IsNaN(StrokeDashOffset) ? 0 : StrokeDashOffset;
          var dashStyle = new System.Windows.Media.DashStyle { Dashes = StrokeDashArray, Offset = notNaN };
          pen.DashStyle = dashStyle;
          pen.DashCap = (System.Windows.Media.PenLineCap)StrokeDashCap;
        }
        pen.StartLineCap = (System.Windows.Media.PenLineCap)StrokeStartLineCap;
        pen.EndLineCap = (System.Windows.Media.PenLineCap)StrokeEndLineCap;
        pen.LineJoin = (System.Windows.Media.PenLineJoin)StrokeLineJoin;
        pen.MiterLimit = StrokeMiterLimit;
        var penAlignment = StrokePenAlignment;
        if (penAlignment!=PenAlignment.Center)
        {
          AdjustBounds(penAlignment, lineWidth, ref left, ref top, ref width, ref height);
        }
      }
      DrawShape(context, brush, pen, left, top, width, height);
    }

    protected abstract void DrawShape(System.Windows.Media.DrawingContext context,
      System.Windows.Media.Brush brush, System.Windows.Media.Pen pen, double left, double top, double width, double height);


    public void AdjustBounds(PenAlignment penAlignment, double lineWidth, 
      ref double left, ref double top, ref double width, ref double height)
    {
      switch (penAlignment)
      {
        case PenAlignment.Inset:
          left += lineWidth/2;
          top += lineWidth/2;
          width -= lineWidth;
          height -= lineWidth;
          break;
        case PenAlignment.Outset:
          left -= lineWidth/2;
          top -= lineWidth/2;
          width += lineWidth;
          height += lineWidth;
          break;
        case PenAlignment.Left:
          left -= lineWidth/2;
          top -= lineWidth/2;
          width -= lineWidth;
          height -= lineWidth;
          break;
        case PenAlignment.Right:
          left += lineWidth/2;
          top += lineWidth/2;
          width += lineWidth;
          height += lineWidth;
          break;
      }
    }
    #endregion

    #region Geometry logic
    public override Geometry GetFillGeometry()
    {
      var left = this.Left;
      var top = this.Top;
      var width = this.Width;
      var height = this.Height;
      var geometry = GetGeometry(left, top, width, height);
      return geometry;
    }

    public override Geometry GetOutlineGeometry()
    {
      var left = this.Left;
      var top = this.Top;
      var width = this.Width;
      var height = this.Height;
      AdjustBounds(this.StrokePenAlignment, this.StrokeThickness, ref left, ref top, ref width, ref height);
      var pen = new Pen(Brushes.Black, StrokeThickness);
      pen.StartLineCap = this.StrokeStartLineCap;
      pen.EndLineCap = this.StrokeEndLineCap;
      pen.LineJoin = this.StrokeLineJoin;
      pen.MiterLimit = this.StrokeMiterLimit;
      pen.DashCap = (System.Windows.Media.PenLineCap)this.StrokeDashCap;
      if (this.StrokeDashArray!=null)
        pen.DashStyle = new DashStyle { Offset = this.StrokeDashOffset, Dashes=this.StrokeDashArray };
      var geometry = GetGeometry(left, top, width, height);
      geometry = geometry.GetWidenedPathGeometry(pen).GetOutlinedPathGeometry();
      return geometry;
    }

    public abstract Geometry GetGeometry(double left, double top, double width, double height);

    public override bool Contains(Point point)
    {
      if (!Fill.IsNullOrEmpty() && GetFillGeometry().FillContains(point))
        return true;
      if (!Stroke.IsNullOrEmpty() && GetOutlineGeometry().FillContains(point))
        return true;
      return false;
    }

    #endregion
  }
}

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Qhta.WPF.Controls
{
  public class GradientEllipse : Shape
  {
    public GradientEllipse()
    {
    }

    #region Origin property
    [TypeConverter(typeof(PointConverter))]
    public Point Origin
    {
      get => (Point)GetValue(OriginProperty);
      set => SetValue(OriginProperty, value);
    }

    public static readonly DependencyProperty OriginProperty = DependencyProperty.Register
      ("Origin", typeof(Point), typeof(GradientEllipse),
        new FrameworkPropertyMetadata(new Point(0.5, 0.5), 
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region Center property
    [TypeConverter(typeof(PointConverter))]
    public Point Center
    {
      get => (Point)GetValue(CenterProperty);
      set => SetValue(CenterProperty, value);
    }

    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register
      ("Center", typeof(Point), typeof(GradientEllipse),
        new FrameworkPropertyMetadata(new Point(0.5, 0.5), 
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region RadiusX property
    public double RadiusX
    {
      get => (double)GetValue(RadiusXProperty);
      set => SetValue(RadiusXProperty, value);
    }

    public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register
      ("RadiusX", typeof(double), typeof(GradientEllipse),
        new FrameworkPropertyMetadata(0.5, 
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region RadiusY property
    public double RadiusY
    {
      get => (double)GetValue(RadiusYProperty);
      set => SetValue(RadiusYProperty, value);
    }

    public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register
      ("RadiusY", typeof(double), typeof(GradientEllipse),
        new FrameworkPropertyMetadata(0.5, 
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region GradientStops property
    public GradientStopCollection GradientStops
    {
      get => (GradientStopCollection)GetValue(GradientStopsProperty);
      set => SetValue(GradientStopsProperty, value);
    }

    public static readonly DependencyProperty GradientStopsProperty = DependencyProperty.Register
      ("GradientStops", typeof(GradientStopCollection), typeof(GradientEllipse),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region ClipToMargin property
    public bool ClipToMargin
    {
      get => (bool)GetValue(ClipToMarginProperty);
      set => SetValue(ClipToMarginProperty, value);
    }

    public static readonly DependencyProperty ClipToMarginProperty = DependencyProperty.Register
      ("ClipToMargin", typeof(bool), typeof(GradientEllipse),
        new FrameworkPropertyMetadata(false, 
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region ClipMargin property
    [TypeConverter(typeof(ThicknessConverter))]
    public Thickness ClipMargin
    {
      get => (Thickness)GetValue(ClipMarginProperty);
      set => SetValue(ClipMarginProperty, value);
    }

    public static readonly DependencyProperty ClipMarginProperty = DependencyProperty.Register
      ("ClipMargin", typeof(Thickness), typeof(GradientEllipse),
        new FrameworkPropertyMetadata(default(Thickness), 
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    protected override Geometry DefiningGeometry
    {
      get
      {
        var c0 = new Point(Center.X*ActualWidth, Center.Y* ActualHeight);
        var rx = RadiusX*ActualWidth;
        var ry = RadiusY*ActualHeight;
        var geometryGroup = new GeometryGroup();
        geometryGroup.Children.Add(new EllipseGeometry(c0, rx, ry));
        return geometryGroup;
      }
    }

    protected override Geometry GetLayoutClip(Size layoutSlotSize)
    {
      return ClipToBounds ? base.GetLayoutClip(layoutSlotSize) : null;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      var c0 = new Point(Center.X*ActualWidth, Center.Y* ActualHeight);
      var rx = RadiusX*ActualWidth;
      var ry = RadiusY*ActualHeight;

      if (ClipToMargin)
      {
        var margin = ClipMargin;
        drawingContext.PushClip(new RectangleGeometry(new Rect(-margin.Left, -margin.Top, 
          ActualWidth+margin.Left+margin.Right, ActualHeight+margin.Top+margin.Bottom)));
      }

      if (Fill!=null)
      {
        var outlinePen = new Pen(Fill, StrokeThickness+1);
        drawingContext.DrawEllipse(null, outlinePen, c0, rx, ry);
      }
      if (Stroke!=null)
      {
        var pen = new Pen(Stroke, StrokeThickness);
        drawingContext.DrawEllipse(null, pen, c0, rx, ry);
      }
      if (GradientStops!=null && GradientStops.Count>0)
      {
        var firstStop = GradientStops.First();
        var lastStop = GradientStops.Last();
        var gradientStops = new GradientStopCollection();
        foreach (var stop in GradientStops)
          gradientStops.Add(new GradientStop(stop.Color.Inverse(), stop.Offset));
        var lineBrush = new RadialGradientBrush(gradientStops);
        lineBrush.GradientOrigin = Origin;
        lineBrush.Center = Center;
        lineBrush.RadiusX = RadiusX;
        lineBrush.RadiusY = RadiusY;
        var linePen = new Pen(lineBrush, StrokeThickness);
        drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));
        drawingContext.DrawEllipse(null, linePen, c0, rx, ry);
        drawingContext.Pop();
      }

      if (ClipToMargin)
        drawingContext.Pop();

    }

  }
}


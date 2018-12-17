using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Qhta.WPF.Controls
{
  public class GradientLine : Shape
  {
    public GradientLine()
    {
    }

    #region StartPoint property
    [TypeConverter(typeof(PointConverter))]
    public Point StartPoint
    {
      get => (Point)GetValue(StartPointProperty);
      set => SetValue(StartPointProperty, value);
    }

    public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register
      ("StartPoint", typeof(Point), typeof(GradientLine),
        new FrameworkPropertyMetadata(new Point(0, 0), 
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region EndPoint property
    [TypeConverter(typeof(PointConverter))]
    public Point EndPoint
    {
      get => (Point)GetValue(EndPointProperty);
      set => SetValue(EndPointProperty, value);
    }

    public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register
      ("EndPoint", typeof(Point), typeof(GradientLine),
        new FrameworkPropertyMetadata(new Point(0, 0), 
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region GradientStops property
    public GradientStopCollection GradientStops
    {
      get => (GradientStopCollection)GetValue(GradientStopsProperty);
      set => SetValue(GradientStopsProperty, value);
    }

    public static readonly DependencyProperty GradientStopsProperty = DependencyProperty.Register
      ("GradientStops", typeof(GradientStopCollection), typeof(GradientLine),
        new FrameworkPropertyMetadata(null, 
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    #region ClipToMargin property
    public bool ClipToMargin
    {
      get => (bool)GetValue(ClipToMarginProperty);
      set => SetValue(ClipToMarginProperty, value);
    }

    public static readonly DependencyProperty ClipToMarginProperty = DependencyProperty.Register
      ("ClipToMargin", typeof(bool), typeof(GradientLine),
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
      ("ClipMargin", typeof(Thickness), typeof(GradientLine),
        new FrameworkPropertyMetadata(default(Thickness),
          FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    protected override Geometry DefiningGeometry
    {
      get
      {
        var p1 = new Point(StartPoint.X*ActualWidth, StartPoint.Y* ActualHeight);
        var p2 = new Point(EndPoint.X*ActualWidth, EndPoint.Y*ActualHeight);
        var angle = Math.Atan2(p2.Y-p1.Y, p2.X-p1.X);
        var geometryGroup = new GeometryGroup();
        geometryGroup.Children.Add(new LineGeometry(p1, p2));
        geometryGroup.Children.Add(new EllipseGeometry(p1, 2, 2));
        var path = new GeometryGroup();
        path.Children.Add(Geometry.Parse("M-1,0L-8,3L-8,-3Z"));
        var transform = new TransformGroup();
        transform.Children.Add(new RotateTransform(angle/Math.PI*180));
        transform.Children.Add(new TranslateTransform(p2.X, p2.Y));
        path.Transform = transform;
        geometryGroup.Children.Add(path);
        return geometryGroup;
      }
    }

    protected override Geometry GetLayoutClip(Size layoutSlotSize)
    {
      return ClipToBounds ? base.GetLayoutClip(layoutSlotSize) : null;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      var p1 = new Point(StartPoint.X*ActualWidth, StartPoint.Y*ActualHeight);
      var p2 = new Point(EndPoint.X*ActualWidth, EndPoint.Y*ActualHeight);
      var angle = Math.Atan2(p2.Y-p1.Y, p2.X-p1.X);
      var path = new GeometryGroup();
      path.Children.Add(Geometry.Parse("M-1,0L-8,3L-8,-3Z"));
      var transform = new TransformGroup();
      transform.Children.Add(new RotateTransform(angle/Math.PI*180));
      transform.Children.Add(new TranslateTransform(p2.X, p2.Y));
      path.Transform = transform;

      if (ClipToMargin)
      {
        var margin = ClipMargin;
        drawingContext.PushClip(new RectangleGeometry(new Rect(-margin.Left, -margin.Top,
          ActualWidth+margin.Left+margin.Right, ActualHeight+margin.Top+margin.Bottom)));
      }

      if (Fill!=null)
      {
        var outlinePen = new Pen(Fill, StrokeThickness+1);
        drawingContext.DrawLine(outlinePen, p1, p2);
        drawingContext.DrawEllipse(Fill, outlinePen, p1, 2, 2);
        drawingContext.DrawGeometry(Fill, outlinePen, path);
      }

      if (Stroke!=null)
      {
        var pen = new Pen(Stroke, StrokeThickness);
        drawingContext.DrawLine(pen, p1, p2);
        drawingContext.DrawEllipse(Stroke, pen, p1, 2, 2);
        drawingContext.DrawGeometry(Stroke, pen, path);
      }
      if (GradientStops!=null && GradientStops.Count>0)
      {
        var firstStop = GradientStops.First();
        var lastStop = GradientStops.Last();
        var startBrush = new SolidColorBrush(firstStop.Color.Inverse());
        var endBrush = new SolidColorBrush(lastStop.Color.Inverse());
        var gradientStops = new GradientStopCollection();
        foreach (var stop in GradientStops)
          gradientStops.Add(new GradientStop(stop.Color.Inverse(), stop.Offset));
        var lineBrush = new LinearGradientBrush(gradientStops, StartPoint, EndPoint);
        var startPen = new Pen(startBrush, StrokeThickness);
        var linePen = new Pen(lineBrush, StrokeThickness);
        var endPen = new Pen(endBrush, StrokeThickness);
        drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight)));
        drawingContext.DrawLine(linePen, p1, p2);
        drawingContext.DrawEllipse(startBrush, startPen, p1, 2, 2);
        drawingContext.DrawGeometry(endBrush, endPen, path);
        drawingContext.Pop();
      }

      if (ClipToMargin)
        drawingContext.Pop();
    }

  }
}


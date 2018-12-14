using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Qhta.WPF.Controls
{
  public class GradientLine : Shape
  {
    public GradientLine() { }

    //#region X1Property
    //[TypeConverter(typeof(LengthConverter))]
    //public double X1
    //{
    //  get => (double)GetValue(X1Property);
    //  set => SetValue(X1Property, value);
    //}

    //public static readonly DependencyProperty X1Property = DependencyProperty.Register
    //  ("X1", typeof(double), typeof(GradientLine),
    //    new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));

    //#endregion

    //#region Y1 property
    //[TypeConverter(typeof(LengthConverter))]
    //public double Y1
    //{
    //  get => (double)GetValue(Y1Property);
    //  set => SetValue(Y1Property, value);
    //}

    //public static readonly DependencyProperty Y1Property = DependencyProperty.Register
    //  ("Y1", typeof(double), typeof(GradientLine),
    //    new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    //#endregion

    //#region X2 property
    //public double X2
    //{
    //  get => (double)GetValue(X2Property);
    //  set => SetValue(X2Property, value);
    //}

    //public static readonly DependencyProperty X2Property = DependencyProperty.Register
    //  ("X2", typeof(double), typeof(GradientLine),
    //    new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    //#endregion

    //#region Y2 property
    //public double Y2
    //{
    //  get => (double)GetValue(Y2Property);
    //  set => SetValue(Y2Property, value);
    //}

    //public static readonly DependencyProperty Y2Property = DependencyProperty.Register
    //  ("Y2", typeof(double), typeof(GradientLine),
    //    new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    //#endregion

    #region StartPoint property
    [TypeConverter(typeof(PointConverter))]
    public Point StartPoint
    {
      get => (Point)GetValue(StartPointProperty);
      set => SetValue(StartPointProperty, value);
    }

    public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register
      ("StartPoint", typeof(Point), typeof(GradientLine),
        new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
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
        new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender));
    #endregion

    protected override Geometry DefiningGeometry
    {
      get
      {
        var geometryGroup = new GeometryGroup();
        geometryGroup.Children.Add(new LineGeometry(
          new Point(StartPoint.X*ActualWidth, StartPoint.Y*ActualHeight),
          new Point(EndPoint.X*ActualWidth, EndPoint.Y*ActualHeight)));
        return geometryGroup;
      }
    }

    private static GeometryConverter geometryConverter = new GeometryConverter();
    protected override void OnRender(DrawingContext drawingContext)
    {
      var pen = new Pen(Stroke, StrokeThickness);
      var p1 = new Point(StartPoint.X*ActualWidth, StartPoint.Y* ActualHeight);
      var p2 = new Point(EndPoint.X*ActualWidth, EndPoint.Y*ActualHeight);
      //Debug.WriteLine($"DrawLine2({p1.X}, {p1.Y})-({p2.X}, {p2.Y})");
      drawingContext.DrawLine(pen, p1, p2);
      drawingContext.DrawEllipse(Stroke, pen, p1, 3, 3);
      var angle = Math.Atan2(p2.Y-p1.Y, p2.X-p1.X);
      var path = Geometry.Parse("M0,0L-10,5L-10,-5Z");
      drawingContext.PushTransform(new TranslateTransform(p2.X, p2.Y));
      drawingContext.PushTransform(new RotateTransform(angle/Math.PI*180));
      //Debug.WriteLine($"angle={angle}");
      drawingContext.DrawGeometry(Stroke, null, path);
    }
  }
}


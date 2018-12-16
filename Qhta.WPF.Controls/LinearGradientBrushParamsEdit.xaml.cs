using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Qhta.WPF.Controls
{
  public partial class LinearGradientBrushParamsEdit : UserControl
  {
    public LinearGradientBrushParamsEdit()
    {
      InitializeComponent();
      PreviewMouseLeftButtonDown+=GradientLine_PreviewMouseLeftButtonDown;
    }

    #region EditedBrush property
    public LinearGradientBrush EditedBrush
    {
      get => (LinearGradientBrush)GetValue(EditedBrushProperty);
      set => SetValue(EditedBrushProperty, value);
    }

    public static readonly DependencyProperty EditedBrushProperty = DependencyProperty.Register
      ("EditedBrush", typeof(LinearGradientBrush), typeof(LinearGradientBrushParamsEdit),
       new FrameworkPropertyMetadata(null, EditedBrushPropertyChanged));

    private static void EditedBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as LinearGradientBrushParamsEdit).UpdateAngleDataViews();
    }

    #endregion

    #region IsAngleLocked property
    /// <summary>
    /// Setting this property to true guarantees that gradient angle is preserved when moving P1.
    /// </summary>
    public bool IsAnglePreserved
    {
      get => (bool)GetValue(IsAngleLockedProperty);
      set => SetValue(IsAngleLockedProperty, value);
    }

    public static readonly DependencyProperty IsAngleLockedProperty = DependencyProperty.Register
      ("IsAngleLocked", typeof(bool), typeof(LinearGradientBrushParamsEdit),
       new FrameworkPropertyMetadata(true));
    #endregion

    #region IsP1CornerLocked property
    /// <summary>
    /// Setting this property to true guarantees that P1 is auto-set to one of 4 corners of the brush enclosing rectangle.
    /// </summary>
    public bool IsP1Fixed
    {
      get => (bool)GetValue(IsP1CornerLockedProperty);
      set => SetValue(IsP1CornerLockedProperty, value);
    }

    public static readonly DependencyProperty IsP1CornerLockedProperty = DependencyProperty.Register
      ("IsP1CornerLocked", typeof(bool), typeof(LinearGradientBrushParamsEdit),
       new FrameworkPropertyMetadata(true));
    #endregion

    #region IsP2EdgeLocked property
    /// <summary>
    /// Setting this property to true guarantees that P2 is kept at the edge of the brush enclosing rectangle
    /// </summary>
    public bool IsP2Fixed
    {
      get => (bool)GetValue(IsP2EdgeLockedProperty);
      set => SetValue(IsP2EdgeLockedProperty, value);
    }

    public static readonly DependencyProperty IsP2EdgeLockedProperty = DependencyProperty.Register
      ("IsP2EdgeLocked", typeof(bool), typeof(LinearGradientBrushParamsEdit),
       new FrameworkPropertyMetadata(true));
    #endregion

    #region AngleDegrees property
    public int AngleDegrees
    {
      get => (int)GetValue(AngleDegreesProperty);
      set => SetValue(AngleDegreesProperty, value);
    }

    public static readonly DependencyProperty AngleDegreesProperty = DependencyProperty.Register
      ("AngleDegrees", typeof(int), typeof(LinearGradientBrushParamsEdit),
       new FrameworkPropertyMetadata(0));
    #endregion

    private void UpdateAngleDataViews()
    {
      if (EditedBrush!=null)
      {
        var startPoint = EditedBrush.StartPoint;
        var endPoint = EditedBrush.EndPoint;
        var dx = endPoint.X-startPoint.X;
        var dy = endPoint.Y-startPoint.Y;
        var angle = Math.Atan2(dy, dx);
        if (angle<0)
          angle+=2*Math.PI;
        AngleNumBox.Value=Math.Round((decimal)(angle/Math.PI*180.0));
        var startX = (decimal)(startPoint.X * 100.0);
        var startY = (decimal)(startPoint.Y * 100.0);
        StartXNumBox.Value = Math.Round(startX);
        StartYNumBox.Value = Math.Round(startY);
        var endX = (decimal)(endPoint.X * 100.0);
        var endY = (decimal)(endPoint.Y * 100.0);
        EndXNumBox.Value = Math.Round(endX);
        EndYNumBox.Value = Math.Round(endY);
        GradientLine.StartPoint=new Point((double)startX/100, (double)startY/100);
        GradientLine.EndPoint=new Point((double)endX/100, (double)endY/100);
        //GradientLine.X1=(double)startX;
        //GradientLine.Y1=(double)startY;
        //GradientLine.X2=(double)endX;
        //GradientLine.Y2=(double)endY;
      }
    }

    bool isEdited;
    private void AngleNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (!isEdited)
      {
        isEdited = true;
        var startPoint = EditedBrush.StartPoint;
        var endPoint = EditedBrush.EndPoint;
        var dx = endPoint.X-startPoint.X;
        var dy = endPoint.Y-startPoint.Y;
        var radius = Math.Sqrt(dx*dx+dy*dy);
        var angle = (double)AngleNumBox.Value;
        if (angle>=360)
          angle -= 360;
        if (angle<0)
          angle += 360;
        if (IsP1Fixed)
        {
          if (angle>=0 && angle<=90)
            startPoint=new Point(0, 0);
          else if (angle>90 && angle<=180)
            startPoint=new Point(1, 0);
          else if (angle>180 && angle<=270)
            startPoint=new Point(1, 1);
          else if (angle>270 && angle<360)
            startPoint=new Point(0, 1);
        }
        endPoint = GradientEndPoint(startPoint, angle, radius);
        EditedBrush = new LinearGradientBrush(EditedBrush.GradientStops, startPoint, endPoint);
        isEdited = false;
      }
    }

    /// <summary>
    /// Evaluate gradient end point basing on startPoint, angle and radius. 
    /// If startPoint is at any of (0,0)-(1,1) rectangle corner, then EndPoint lays on the rectangle edge.
    /// </summary>
    /// <param name="startPoint">start point coords (range 0..1)</param>
    /// <param name="angle">angle from X axis incrementing down (in degrees)</param>
    /// <param name="radius">radius of the vector (range 0..1)</param>
    /// <returns></returns>
    private Point GradientEndPoint(Point startPoint, double angle, double radius)
    {
      double alpha;
      if (IsP2Fixed)
      {
        if (angle>=0 && angle<=45)
        {
          alpha = angle/180.0 * Math.PI;
          return new Point(1, (1-startPoint.X)*Math.Tan(alpha));
        }
        else
        if (angle>45 && angle<90)
        {
          alpha = angle/180.0 * Math.PI;
          return new Point((1-startPoint.Y)/Math.Tan(alpha), 1);
        }
        else
        if (angle==90)
        {
          return new Point(startPoint.X, 1);
        }
        else
        if (angle>90 && angle<=135)
        {
          alpha = (angle-90)/180 * Math.PI;
          return new Point(1-(1-startPoint.Y)*Math.Tan(alpha), 1);
        }
        else
        if (angle>135 && angle<180)
        {
          alpha = (angle-90)/180 * Math.PI;
          return new Point(0, startPoint.X/Math.Tan(alpha));
        }
        else
        if (angle==180)
          return new Point(0, startPoint.Y);
        else
        if (angle>180 && angle<=225)
        {
          alpha = (angle-180)/180 * Math.PI;
          return new Point(0, 1-startPoint.X*Math.Tan(alpha));
        }
        if (angle>225 && angle<270)
        {
          alpha = (angle-180)/180 * Math.PI;
          return new Point(1-startPoint.Y/Math.Tan(alpha), 0);
        }
        else if (angle==270)
          return new Point(startPoint.X, 0);
        else
        if (angle>270 && angle<315)
        {
          alpha = (angle-270)/180 * Math.PI;
          return new Point(startPoint.Y*Math.Tan(alpha), 0);
        }
        else
        if (angle>=315 && angle<360)
        { 
          alpha = (angle-270)/180 * Math.PI;
          return new Point(1, 1-(1-startPoint.X)/Math.Tan(alpha));
        }
      }
      alpha = angle/180.0 * Math.PI;
      return new Point(startPoint.X+Math.Cos(alpha)*radius, startPoint.Y+Math.Sin(alpha)*radius);
    }

    private void StartXNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (!isEdited)
      {
        isEdited = true;
        var startPoint = EditedBrush.StartPoint;
        var endPoint = EditedBrush.EndPoint;
        startPoint.X=(double)args.NewValue/100;
        if (IsAnglePreserved)
        {
          var dX = startPoint.X - EditedBrush.StartPoint.X;
          endPoint.X += dX;
        }
        EditedBrush = new LinearGradientBrush(EditedBrush.GradientStops, startPoint, endPoint);
        isEdited = false;
      }
    }

    private void StartYNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (!isEdited)
      {
        isEdited = true;
        var startPoint = EditedBrush.StartPoint;
        var endPoint = EditedBrush.EndPoint;
        var angle = Math.Atan2(endPoint.X-startPoint.X, endPoint.Y-startPoint.Y);
        startPoint.Y=(double)args.NewValue/100;
        if (IsAnglePreserved && IsP2Fixed)
        {

          var dY = startPoint.Y - EditedBrush.StartPoint.Y;
          endPoint.Y += dY;
        }
        else if (IsAnglePreserved)
        {
          var dY = startPoint.Y - EditedBrush.StartPoint.Y;
          endPoint.Y += dY;
        }
        EditedBrush = new LinearGradientBrush(EditedBrush.GradientStops, startPoint, endPoint);
        isEdited = false;
      }
    }

    private void GradientLine_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    {
      var p1 = new Point(EditedBrush.StartPoint.X*GradientLine.ActualWidth, EditedBrush.StartPoint.Y*GradientLine.ActualHeight);
      var p2 = new Point(EditedBrush.EndPoint.X*GradientLine.ActualWidth, EditedBrush.EndPoint.Y*GradientLine.ActualHeight);
      var pos = args.GetPosition(GradientLine);
      var dx = pos.X-p1.X;
      var dy = pos.Y-p1.Y;
      clickPos = pos;
      isStartPointClicked= (Math.Sqrt(dx*dx+dy*dy)<=3);
      dx = pos.X-p2.X;
      dy = pos.Y-p2.Y;
      isEndPointClicked = (Math.Sqrt(dx*dx+dy*dy)<=3);
    }

    bool isStartPointClicked;
    bool isEndPointClicked;
    Point clickPos;

  }
}

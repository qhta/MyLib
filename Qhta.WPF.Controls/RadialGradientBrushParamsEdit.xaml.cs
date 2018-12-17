using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Qhta.WPF.Controls
{
  public partial class RadialGradientBrushParamsEdit : UserControl
  {
    public RadialGradientBrushParamsEdit()
    {
      InitializeComponent();
      //PreviewMouseLeftButtonDown+=GradientLine_PreviewMouseLeftButtonDown;
      //PreviewMouseMove+=RadialGradientBrushParamsEdit_PreviewMouseMove;
      //PreviewMouseLeftButtonUp+=RadialGradientBrushParamsEdit_PreviewMouseLeftButtonUp;
    }

    #region EditedBrush property
    public RadialGradientBrush EditedBrush
    {
      get => (RadialGradientBrush)GetValue(EditedBrushProperty);
      set => SetValue(EditedBrushProperty, value);
    }

    public static readonly DependencyProperty EditedBrushProperty = DependencyProperty.Register
      ("EditedBrush", typeof(RadialGradientBrush), typeof(RadialGradientBrushParamsEdit),
       new FrameworkPropertyMetadata(null, EditedBrushPropertyChanged));

    private static void EditedBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as RadialGradientBrushParamsEdit).UpdateBrushDataViews();
    }

    #endregion

    #region IsCenterAlignedToOrigin property
    /// <summary>
    /// Setting this property to true guarantees that gradient center is aligned to gradient origin.
    /// </summary>
    public bool IsCenterAlignedToOrigin
    {
      get => (bool)GetValue(IsCenterAlignedToOriginProperty);
      set => SetValue(IsCenterAlignedToOriginProperty, value);
    }

    public static readonly DependencyProperty IsCenterAlignedToOriginProperty = DependencyProperty.Register
      ("IsCenterAlignedToOrigin", typeof(bool), typeof(RadialGradientBrushParamsEdit),
       new FrameworkPropertyMetadata(true));
    #endregion

    private void UpdateBrushDataViews()
    {
      if (EditedBrush!=null)
      {
        var origin = EditedBrush.GradientOrigin;
        var center = EditedBrush.Center;
        var rx = EditedBrush.RadiusX;
        var ry = EditedBrush.RadiusY;
        var originX = (decimal)(origin.X * 100.0);
        var originY = (decimal)(origin.Y * 100.0);
        OriginXNumBox.Value = Math.Round(originX);
        OriginYNumBox.Value = Math.Round(originY);
        var centerX = (decimal)(center.X * 100.0);
        var centerY = (decimal)(center.Y * 100.0);
        CenterXNumBox.Value = Math.Round(centerX);
        CenterYNumBox.Value = Math.Round(centerY);
        var radiusX = (decimal)(rx * 100.0);
        var radiusY = (decimal)(ry * 100.0);
        RadiusXNumBox.Value = Math.Round(radiusX);
        RadiusYNumBox.Value = Math.Round(radiusY);

        //Canvas.SetLeft(FocalCircle, (double)focalX/100-FocalCircle.Width/2);
        //Canvas.SetTop(FocalCircle, (double)focalY/100-FocalCircle.Height/2);
        GradientEllipse.Origin=origin;
        GradientEllipse.Center=center;
        GradientEllipse.RadiusX=rx;
        GradientEllipse.RadiusY=ry;
        //GradientLine.Y1=(double)startY;
        //GradientLine.X2=(double)endX;
        //GradientLine.Y2=(double)endY;
      }
    }

    bool isEdited;

    private void OriginXNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (!isEdited)
      {
        isEdited = true;
        var originPoint = EditedBrush.GradientOrigin;
        var brush = EditedBrush.Clone();
        originPoint.X=(double)args.NewValue/100;
        if (IsCenterAlignedToOrigin)
        {
          var dx = originPoint.X - EditedBrush.GradientOrigin.X;
          var center = EditedBrush.Center;
          center.X += dx;
          brush.Center = center;
        }
        brush.GradientOrigin = originPoint;
        EditedBrush=brush;
        isEdited = false;
      }
    }

    private void OriginYNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (!isEdited)
      {
        isEdited = true;
        var originPoint = EditedBrush.GradientOrigin;
        var brush = EditedBrush.Clone();
        originPoint.Y=(double)args.NewValue/100;
        if (IsCenterAlignedToOrigin)
        {
          var dy = originPoint.Y - EditedBrush.GradientOrigin.Y;
          var center = EditedBrush.Center;
          center.Y += dy;
          brush.Center = center;
        }
        brush.GradientOrigin = originPoint;
        EditedBrush=brush;
        isEdited = false;
      }
    }

    private void CenterXNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (!isEdited)
      {
        isEdited = true;
        var center = EditedBrush.Center;
        center.X=(double)args.NewValue/100;
        var brush = EditedBrush.Clone();
        brush.Center = center;
        EditedBrush=brush;
        isEdited = false;
      }
    }

    private void CenterYNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (!isEdited)
      {
        isEdited = true;
        var center = EditedBrush.Center;
        center.Y=(double)args.NewValue/100;
        var brush = EditedBrush.Clone();
        brush.Center = center;
        EditedBrush=brush;
        isEdited = false;
      }
    }

    private void RadiusXNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (!isEdited)
      {
        isEdited = true;
        var radiusX = (double)args.NewValue/100;
        var brush = EditedBrush.Clone();
        brush.RadiusX = radiusX;
        EditedBrush=brush;
        isEdited = false;
      }
    }

    private void RadiusYNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (!isEdited)
      {
        isEdited = true;
        var radiusY = (double)args.NewValue/100;
        var brush = EditedBrush.Clone();
        brush.RadiusY = radiusY;
        EditedBrush=brush;
        isEdited = false;
      }
    }



    //private void GradientLine_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    //{
    //  //var p1 = new Point(EditedBrush.StartPoint.X*GradientLine.ActualWidth, EditedBrush.StartPoint.Y*GradientLine.ActualHeight);
    //  //var p2 = new Point(EditedBrush.EndPoint.X*GradientLine.ActualWidth, EditedBrush.EndPoint.Y*GradientLine.ActualHeight);
    //  //var pos = args.GetPosition(GradientLine);
    //  //var dx = pos.X-p1.X;
    //  //var dy = pos.Y-p1.Y;
    //  //clickPos = pos;
    //  //if (Math.Sqrt(dx*dx+dy*dy)<=3)
    //  //{
    //  //  isStartPointClicked = true;
    //  //  CaptureMouse();
    //  //}
    //  //dx = pos.X-p2.X;
    //  //dy = pos.Y-p2.Y;
    //  //if (Math.Sqrt(dx*dx+dy*dy)<=6)
    //  //{
    //  //  isEndPointClicked = true;
    //  //  isStartPointClicked = false;
    //  //  CaptureMouse();
    //  //}
    //}

    ////bool isStartPointClicked;
    ////bool isEndPointClicked;
    ////Point clickPos;
    ////bool isStartPointDragged;
    ////bool isEndPointDragged;

    //private void RadialGradientBrushParamsEdit_PreviewMouseMove(object sender, MouseEventArgs args)
    //{
    //  //if (isStartPointClicked || isEndPointClicked)
    //  //{
    //  //  var pos = args.GetPosition(GradientLine);
    //  //  var dx = pos.X-clickPos.X;
    //  //  var dy = pos.Y-clickPos.Y;
    //  //  if (!isStartPointDragged && !isEndPointDragged)
    //  //  {
    //  //    if (Math.Sqrt(dx*dx+dy*dy)<=3)
    //  //    {
    //  //      if (isStartPointClicked)
    //  //      {
    //  //        isStartPointDragged = true;
    //  //        isEdited=true;
    //  //      }
    //  //      else
    //  //      if (isEndPointClicked)
    //  //      {
    //  //        isEndPointDragged = true;
    //  //        isEdited=true;
    //  //      }
    //  //    }
    //  //  }
    //  //  if (isStartPointDragged)
    //  //  {
    //  //    var startPoint = new Point(pos.X / GradientLine.ActualWidth, pos.Y / GradientLine.ActualHeight);
    //  //    var endPoint = EditedBrush.EndPoint;
    //  //    EditedBrush = new RadialGradientBrush(EditedBrush.GradientStops, startPoint, endPoint);
    //  //  }
    //  //  else
    //  //  if (isEndPointDragged)
    //  //  {
    //  //    var startPoint = EditedBrush.StartPoint;
    //  //    var endPoint = new Point(pos.X / GradientLine.ActualWidth, pos.Y / GradientLine.ActualHeight);
    //  //    EditedBrush = new RadialGradientBrush(EditedBrush.GradientStops, startPoint, endPoint);
    //  //  }
    //  //}
    //}

    //private void RadialGradientBrushParamsEdit_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
    //{
    //  //isStartPointClicked=false;
    //  //isEndPointClicked=false;
    //  //isStartPointDragged=false;
    //  //isEndPointDragged=false;
    //  isEdited=false;
    //  Mouse.Capture(null);
    //}
  }
}
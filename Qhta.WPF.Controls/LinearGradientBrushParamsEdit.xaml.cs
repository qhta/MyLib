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
    public bool IsAngleLocked
    {
      get => (bool)GetValue(IsAngleLockedProperty);
      set => SetValue(IsAngleLockedProperty, value);
    }

    public static readonly DependencyProperty IsAngleLockedProperty = DependencyProperty.Register
      ("IsAngleLocked", typeof(bool), typeof(LinearGradientBrushParamsEdit),
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
        AngleNumBox.Value=(decimal)(angle/Math.PI*180.0);
        StartXNumBox.Value = (decimal)(startPoint.X * 100.0);
        StartYNumBox.Value = (decimal)(startPoint.Y * 100.0);
        EndXNumBox.Value = (decimal)(endPoint.X * 100.0);
        EndYNumBox.Value = (decimal)(endPoint.Y * 100.0);
      }
    }

    private void AngleNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      //var startPoint = EditedBrush.StartPoint;
      //var endPoint = EditedBrush.EndPoint;
      //var dx = endPoint.X-startPoint.X;
      //var dy = endPoint.Y-startPoint.Y;
      //var radius = Math.Sqrt(dx*dx+dy*dy);
      //var angle = (double)AngleNumBox.Value/180.0 * Math.PI;
      //endPoint.Y = startPoint.Y+Math.Sin(angle)*radius;
      //endPoint.X = startPoint.X+Math.Cos(angle)*radius;
      EditedBrush = new LinearGradientBrush(EditedBrush.GradientStops, (double)args.NewValue);
    }
  }

}

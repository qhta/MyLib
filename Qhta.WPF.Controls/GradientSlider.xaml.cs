using System;
using System.Collections.Generic;
using System.Diagnostics;
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
  public partial class GradientSlider : UserControl
  {
    public GradientSlider()
    {
      InitializeComponent();
    }

    public override void OnApplyTemplate()
    {
      GradientStopsView.GradientStopsChanged+=GradientStopsView_GradientStopsChanged;
    }

    #region EditedBrush property
    public GradientBrush EditedBrush
    {
      get => (GradientBrush)GetValue(EditedBrushProperty);
      set => SetValue(EditedBrushProperty, value);
    }

    public static readonly DependencyProperty EditedBrushProperty = DependencyProperty.Register
      ("EditedBrush", typeof(GradientBrush), typeof(GradientSlider),
       new FrameworkPropertyMetadata(null, EditedBrushPropertyChanged));

    public static void EditedBrushPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as GradientSlider).EditedBrushChanged(args);
    }

    public void EditedBrushChanged(DependencyPropertyChangedEventArgs args)
    {
      var brush = args.NewValue as GradientBrush;
      if (brush!=null)
      {
        if (brush.GradientStops.Count==0)
          brush.GradientStops.Add(new GradientStop(Colors.White, 0));
        if (brush.GradientStops.Count==1)
          brush.GradientStops.Add(new GradientStop(Colors.Black, 1));
      }
      ShownBrush=brush.Clone();
    }
    #endregion

    #region ShownBrush property
    public GradientBrush ShownBrush
    {
      get => (GradientBrush)GetValue(ShownBrushProperty);
      set => SetValue(ShownBrushProperty, value);
    }

    public static readonly DependencyProperty ShownBrushProperty = DependencyProperty.Register
      ("ShownBrush", typeof(GradientBrush), typeof(GradientSlider),
       new FrameworkPropertyMetadata(null));
    #endregion

    public event ValueChangedEventHandler<GradientStopCollection> GradientStopsChanged;

    private void GradientStopsView_GradientStopsChanged(object sender, ValueChangedEventArgs<GradientStopCollection> args)
    {
      GradientStopsChanged?.Invoke(sender, args);
    }


  }
}

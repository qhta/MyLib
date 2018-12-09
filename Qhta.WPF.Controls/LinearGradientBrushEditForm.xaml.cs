using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF.Controls
{
  public partial class LinearGradientBrushEditForm : UserControl
  {
    public LinearGradientBrushEditForm()
    {
      InitializeComponent();
    }

    public override void OnApplyTemplate()
    {
      //GradientSlider.EditedBrush = this.EditedBrush;
      GradientSlider.GradientStopsChanged+=GradientSlider_GradientStopsChanged;
    }

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(LinearGradientBrushEditForm),
       new FrameworkPropertyMetadata(Colors.Transparent, SelectedColorPropertyChanged));

    public static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = sender as LinearGradientBrushEditForm;
      var brush = c.EditedBrush;
      if (brush==null)
      {
        var startColor = (Color)args.NewValue;
        var hsv = startColor.ToDrawingColor().ToAhsv();
        hsv.S=0;
        hsv.V=1;
        startColor = hsv.ToColor().ToMediaColor();
        hsv.S=1;
        hsv.V=0;
        var endColor = hsv.ToColor().ToMediaColor();
        brush = new LinearGradientBrush(startColor, endColor, 0);
        c.EditedBrush=brush;
      }
    }
    #endregion

    #region EditedBrush property
    public LinearGradientBrush EditedBrush
    {
      get => (LinearGradientBrush)GetValue(EditedBrushProperty);
      set => SetValue(EditedBrushProperty, value);
    }

    public static readonly DependencyProperty EditedBrushProperty = DependencyProperty.Register
      ("EditedBrush", typeof(LinearGradientBrush), typeof(LinearGradientBrushEditForm),
       new FrameworkPropertyMetadata(null));
    #endregion

    private void GradientSlider_GradientStopsChanged(object sender, ValueChangedEventArgs<GradientStopCollection> args)
    {
      var newBrush = new LinearGradientBrush(args.NewValue);
      EditedBrush = newBrush;
    }

  }
}

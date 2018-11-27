using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qhta.Drawing;

namespace Qhta.WPF.Controls
{
  public partial class CustomColorForm : UserControl
  {
    public CustomColorForm()
    {
      InitializeComponent();
    }

    public override void OnApplyTemplate()
    {
      SetColorPad(0);
      //SetHueSlider(Colors.Red);
    }

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(CustomColorForm),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
         SelectedColorPropertyChanged));

    private static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      //(sender as CustomColorForm).UpdatePreview();
    }
    #endregion

    //private void SetHueSlider(Color color)
    //{
    //  var HSV = color.ToDrawingColor().ToAhsv();
    //  HSV.S=0;
    //  this.HueSlider.HueChange=HueChange.None;
    //  this.HueSlider.Color1=HSV.ToColor().ToMediaColor();
    //}

    private void HueTextEdit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> args)
    {
      double hue = (double)args.NewValue/360.0;
      SetColorPad(hue);
      HueSlider.Position=hue;
    }

    private void SetColorPad(double hue)
    {
      var HSV = new AhsvColor(1, hue, 0, 0);
      this.ColorPad.Color00=HSV.ToColor().ToMediaColor();
      HSV.S=0;
      HSV.V=1;
      this.ColorPad.Color01=HSV.ToColor().ToMediaColor();
      HSV.S=1;
      HSV.V=1;
      this.ColorPad.Color11=HSV.ToColor().ToMediaColor();
      HSV.S=1;
      HSV.V=0;
      this.ColorPad.Color10=HSV.ToColor().ToMediaColor();
      this.ColorPad.InvariantHue=(int)(hue*360);
    }

  }
}

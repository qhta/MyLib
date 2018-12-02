using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Qhta.WPF.Utils;
using Qhta.Drawing;

namespace Qhta.WPF.Controls
{
  public partial class ColorEditForm : UserControl
  {
    public ColorEditForm()
    {
      InitializeComponent();
    }

    public override void OnApplyTemplate()
    {
      CurrentColor = Colors.Red;
    }

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(ColorEditForm),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
         SelectedColorPropertyChanged));

    private static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      //(sender as CustomColorForm).UpdatePreview();
    }
    #endregion

    #region CurrentColor property
    public Color CurrentColor
    {
      get => (Color)GetValue(CurrentColorProperty);
      set => SetValue(CurrentColorProperty, value);
    }

    public static readonly DependencyProperty CurrentColorProperty = DependencyProperty.Register
      ("CurrentColor", typeof(Color), typeof(ColorEditForm),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
         CurrentColorPropertyChanged));

    private static void CurrentColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      //(sender as CustomColorForm).UpdatePreview();
    }
    #endregion

    //private bool isBackChanging;

    //private void HueTextEdit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> args)
    //{
    //  if (isBackChanging)
    //    return;
    //  double hue = (double)args.NewValue/360.0;
    //  HueSlider.Position=hue;
    //  SetColorPad(hue);
    //}

    //private void SetColorPad(double hue)
    //{
    //  var HSV = new AhsvColor(1, hue, 0, 0);
    //  this.ColorPad.Color00=HSV.ToColor().ToMediaColor();
    //  HSV.S=0;
    //  HSV.V=1;
    //  this.ColorPad.Color01=HSV.ToColor().ToMediaColor();
    //  HSV.S=1;
    //  HSV.V=1;
    //  this.ColorPad.Color11=HSV.ToColor().ToMediaColor();
    //  HSV.S=1;
    //  HSV.V=0;
    //  this.ColorPad.Color10=HSV.ToColor().ToMediaColor();
    //  this.ColorPad.InvariantHue=(int)(hue*360);
    //  this.ColorPad.SelectedColor = this.ColorPad.Position2Color(this.ColorPad.Position);
    //  //ColorPad_SelectedColorChanged();
    //}

    //private void ColorPad_SelectedColorChanged()
    //{
      //var color = this.ColorPad.SelectedColor;
      //var hsv = color.ToDrawingColor().ToAhsv();
      //this.SaturationTextEdit.Value=(decimal)(hsv.S*100);
      //this.ValueTextEdit.Value=(decimal)(hsv.V*100);
    //}


    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
      SelectedColor = CurrentColor;
      CloseForm(true);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      CloseForm(false);
    }

    private void CloseForm(bool ok)
    {
      var window = this.FindParentWindow();
      if (window!=null)
      {
        window.DialogResult = ok;
        window.Close();
      }
    }

  }
}

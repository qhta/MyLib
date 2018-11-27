using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Qhta.WPF.Utils;
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
      ColorPad.Position=new Point(1, 1);
      SetColorPad(0);
      SetAlphaSlider(Colors.Red);
      SetProbeRectangle(Colors.Red);
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

    private bool isBackChanging;

    private void HueTextEdit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> args)
    {
      if (isBackChanging)
        return;
      double hue = (double)args.NewValue/360.0;
      HueSlider.Position=hue;
      SetColorPad(hue);
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
      this.ColorPad.SelectedColor = this.ColorPad.Position2Color(this.ColorPad.Position);
      ColorPad_SelectedColorChanged();
    }

    private bool isColorPadValueChanged;
    private void ColorPad_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Point> args)
    {
      isColorPadValueChanged = true;
      ColorPad_SelectedColorChanged();
      isColorPadValueChanged = false;
    }

    private void ColorPad_SelectedColorChanged()
    {
      var color = this.ColorPad.SelectedColor;
      this.RedTextEdit.Value=color.R;
      this.BlueTextEdit.Value=color.B;
      this.GreenTextEdit.Value=color.G;

      SetSlider(RedSlider, 'R', color);
      SetSlider(GreenSlider, 'G', color);
      SetSlider(BlueSlider, 'B', color);

      var hsv = color.ToDrawingColor().ToAhsv();
      this.SaturationTextEdit.Value=(decimal)(hsv.S*100);
      this.ValueTextEdit.Value=(decimal)(hsv.V*100);
      color.A = (byte)(AlphaSlider.Position*255);
      SetAlphaSlider(color);
    }

    private void SetSlider(ColorSlider slider, char member, Color selectedColor)
    {
      var color0 = selectedColor;
      var color1 = selectedColor;
      switch (member)
      {
        case 'R':
          color0.R=0;
          color1.R=255;
          break;
        case 'G':
          color0.G=0;
          color1.G=255;
          break;
        case 'B':
          color0.B=0;
          color1.B=255;
          break;
      }
      slider.Color0 = color0;
      slider.Color1 = color1;
      slider.SelectedColor = selectedColor;
    }

    private bool isColorEditChanging;
    private void RedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Color> args)
    {
      if (!isColorEditChanging)
      {
        //Debug.WriteLine($"RedSliderValueChanged");
        isColorEditChanging = true;
        RedTextEdit.Value=args.NewValue.R;
        SetHSVAndColorPadFromRGBSliders();
        isColorEditChanging = false;
      }
    }

    private void GreenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Color> args)
    {
      if (!isColorEditChanging)
      {
        //Debug.WriteLine($"GreenSliderValueChanged");
        isColorEditChanging = true;
        GreenTextEdit.Value=args.NewValue.G;
        SetHSVAndColorPadFromRGBSliders();
        isColorEditChanging = false;
      }
    }

    private void BlueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Color> args)
    {
      if (!isColorEditChanging)
      {
        //Debug.WriteLine($"BlueSliderValueChanged");
        isColorEditChanging = true;
        BlueTextEdit.Value=args.NewValue.B;
        SetHSVAndColorPadFromRGBSliders();
        isColorEditChanging = false;
      }
    }

    private void RedTextEdit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> args)
    {
      if (!isColorEditChanging)
      {
        //Debug.WriteLine($"RedTextEditChanged");
        isColorEditChanging = true;
        RedSlider.Position = (double)args.NewValue/255.0;
        SetHSVAndColorPadFromRGBSliders();
        isColorEditChanging = false;
      }
    }

    private void GreenTextEdit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> args)
    {
      if (!isColorEditChanging)
      {
        //Debug.WriteLine($"GreenTextEditChanged");
        isColorEditChanging = true;
        GreenSlider.Position = (double)args.NewValue/255.0;
        SetHSVAndColorPadFromRGBSliders();
        isColorEditChanging = false;
      }
    }

    private void BlueTextEdit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> args)
    {
      if (!isColorEditChanging)
      {
        //Debug.WriteLine($"BlueTextEditChanged");
        isColorEditChanging = true;
        RedSlider.Position = (double)args.NewValue/255.0;
        SetHSVAndColorPadFromRGBSliders();
        isColorEditChanging = false;
      }
    }

    private void SetHSVAndColorPadFromRGBSliders()
    {
      if (isColorPadValueChanged)
        return;
      if (!isBackChanging)
      {
        isBackChanging=true;
        var r = RedSlider.Position;
        var g = GreenSlider.Position;
        var b = BlueSlider.Position;
        var rgb = new ArgbColor(1, r, g, b);
        var hsv = rgb.ToAhsv();
        if (hsv.H!=HueSlider.Position)
        {
          //Debug.WriteLine($"HueSlider.Position {HueSlider.Position}->{hsv.H}");
          this.HueSlider.Position=hsv.H;
          SetColorPad(hsv.H);
          this.HueTextEdit.Value=(int)(hsv.H*360);
          this.SaturationTextEdit.Value=(int)(hsv.S*100);
          this.ValueTextEdit.Value=(int)(hsv.V*100);
        }
        hsv.A = AlphaSlider.Position;
        SetProbeRectangle(hsv.ToColor().ToMediaColor());
        isBackChanging=false;
      }
    }

    private void SetAlphaSlider(Color color)
    {
      var color0 = color;
      color0.A = 0;
      var color1 = color;
      color1.A = 255;
      AlphaSlider.Color0 = color0;
      AlphaSlider.Color1 = color1;
      AlphaSlider.Position = color.A/255.0;
    }

    private void AlphaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Color> args)
    {
      if (!isColorEditChanging)
      {
        //Debug.WriteLine($"AlphaSliderValueChanged");
        isColorEditChanging = true;
        AlphaTextEdit.Value=args.NewValue.A;
        SetProbeRectangle(args.NewValue.A/255.0);
        isColorEditChanging = false;
      }
    }

    private void AlphaTextEdit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> args)
    {
      if (!isColorEditChanging)
      {
        //Debug.WriteLine($"AlphaTextEditChanged");
        isColorEditChanging = true;
        AlphaSlider.Position = (double)args.NewValue/255.0;
        SetProbeRectangle(AlphaSlider.Position);
        isColorEditChanging = false;
      }
    }

    private void SetProbeRectangle(Color color)
    {
      this.ProbeRectangle.Fill=new SolidColorBrush(color);
    }

    private void SetProbeRectangle(double alpha)
    {
      if (this.ProbeRectangle.Fill is SolidColorBrush brush)
      {
        var color = brush.Color;
        color.A = (byte)(alpha*255);
        this.ProbeRectangle.Fill=new SolidColorBrush(color);
      }
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
      if (this.ProbeRectangle.Fill is SolidColorBrush brush)
      {
        var color = brush.Color;
        SelectedColor = color;
        CloseForm(true);
      }
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

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF.Controls
{

  public enum ColorMember
  {
    Alpha = 1,
    Red,
    Green,
    Blue,
    Cyan,
    Magenta,
    Yellow,
    Hue,
    Saturation,
    Brightness,
  }

  public class ColorMemberEdit : Control
  {
    public ColorMemberEdit()
    {
      DefaultStyleKey = typeof(ColorMemberEdit);
    }

    protected NumericEditBox ColorNumBox;
    protected ColorSlider ColorSlider;
    public override void OnApplyTemplate()
    {
      ColorNumBox = Template.FindName("ColorNumBox", this) as NumericEditBox;
      ColorSlider = Template.FindName("ColorSlider", this) as ColorSlider;
      ColorSlider.Name = "_"+this.Name;
      if (Member==ColorMember.Hue)
        ColorSlider.HueChange = HueChange.Positive;
      ColorPropertyChanged();

      if (double.IsNaN(Value))
      {
        if (SelectedColor!=Colors.Transparent)
        {
          SetValue(this.SelectedColor);
        }
        else
          Value=1.0;
      }
      ColorNumBox.Value = (decimal)(int)(this.Value*ValueScale);
      ColorSlider.Position=this.Value;
      ColorNumBox.ValueChanged+=ColorNumBox_ValueChanged;
      ColorSlider.ValueChanged+=ColorSlider_ValueChanged;
    }

    #region Member property
    public ColorMember Member
    {
      get => (ColorMember)GetValue(MemberProperty);
      set => SetValue(MemberProperty, value);
    }

    public static readonly DependencyProperty MemberProperty = DependencyProperty.Register
      ("Member", typeof(ColorMember), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(ColorMember.Alpha, MemberPropertyChanged));
    private static void MemberPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = sender as ColorMemberEdit;
      if ((ColorMember)args.NewValue==ColorMember.Hue)
        c.ValueScale = 360;
      else
        c.ValueScale = 255;
    }
    #endregion

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(Colors.Transparent, SelectedColorPropertyChanged));

    private static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = sender as ColorMemberEdit;
      if (c.ColorSlider!=null)
      {
        if (c.Member>=ColorMember.Hue)
          c.SetValue((Color)args.NewValue);
        else
          c.ColorSlider.SelectedColor = (Color)args.NewValue;
      }
    }
    #endregion

    #region BaseColor property
    public Color BaseColor
    {
      get => (Color)GetValue(BaseColorProperty);
      set => SetValue(BaseColorProperty, value);
    }

    public static readonly DependencyProperty BaseColorProperty = DependencyProperty.Register
      ("BaseColor", typeof(Color), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(Colors.Black, BaseColorPropertyChanged));

    private static void BaseColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = sender as ColorMemberEdit;
      c.Color0=(Color)args.NewValue;
      c.Color1=(Color)args.NewValue;
      //if (c.ColorSlider!=null)
      //  c.ColorSlider.SelectedColor = (Color)args.NewValue;
    }
    #endregion

    #region Color0 property
    public Color Color0
    {
      get => (Color)GetValue(Color0Property);
      set => SetValue(Color0Property, value);
    }

    public static readonly DependencyProperty Color0Property = DependencyProperty.Register
      ("Color0", typeof(Color), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(Colors.Black, ColorPropertyChanged));
    #endregion

    #region Color1 property
    public Color Color1
    {
      get => (Color)GetValue(Color1Property);
      set => SetValue(Color1Property, value);
    }

    public static readonly DependencyProperty Color1Property = DependencyProperty.Register
      ("Color1", typeof(Color), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(Colors.Black, ColorPropertyChanged));
    #endregion

    #region Resolution property
    public int Resolution
    {
      get => (int)GetValue(ResolutionProperty);
      set => SetValue(ResolutionProperty, value);
    }

    public static readonly DependencyProperty ResolutionProperty = DependencyProperty.Register
      ("Resolution", typeof(int), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(100));
    #endregion

    #region ValueScale property
    public double ValueScale
    {
      get => (double)GetValue(ValueScaleProperty);
      set => SetValue(ValueScaleProperty, value);
    }

    public static readonly DependencyProperty ValueScaleProperty = DependencyProperty.Register
      ("ValueScale", typeof(double), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(255.0));
    #endregion

    #region LabelText property
    public string LabelText
    {
      get => (string)GetValue(LabelTextProperty);
      set => SetValue(LabelTextProperty, value);
    }

    public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register
      ("LabelText", typeof(string), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(null));
    #endregion

    #region LabelWidth property
    public double LabelWidth
    {
      get => (double)GetValue(LabelWidthProperty);
      set => SetValue(LabelWidthProperty, value);
    }

    public static readonly DependencyProperty LabelWidthProperty = DependencyProperty.Register
      ("LabelWidth", typeof(double), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(double.NaN));
    #endregion

    #region LabelVisibility property
    public Visibility LabelVisibility
    {
      get => (Visibility)GetValue(LabelVisibilityProperty);
      set => SetValue(LabelVisibilityProperty, value);
    }

    public static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.Register
      ("LabelVisibility", typeof(Visibility), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(Visibility.Visible));
    #endregion

    #region NumBoxVisibility property
    public Visibility NumBoxVisibility
    {
      get => (Visibility)GetValue(NumBoxVisibilityProperty);
      set => SetValue(NumBoxVisibilityProperty, value);
    }

    public static readonly DependencyProperty NumBoxVisibilityProperty = DependencyProperty.Register
      ("NumBoxVisibility", typeof(Visibility), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(Visibility.Visible));
    #endregion

    #region SliderVisibility property
    public Visibility SliderVisibility
    {
      get => (Visibility)GetValue(SliderVisibilityProperty);
      set => SetValue(SliderVisibilityProperty, value);
    }

    public static readonly DependencyProperty SliderVisibilityProperty = DependencyProperty.Register
      ("SliderVisibility", typeof(Visibility), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(Visibility.Visible));
    #endregion

    private static void ColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = sender as ColorMemberEdit;
      c.ColorPropertyChanged();
    }

    protected virtual void ColorPropertyChanged()
    {
      if (ColorSlider!=null)
      {
        var color0 = Color0;
        var color1 = Color1;
        if (Member!=ColorMember.Alpha)
        {
          color0.A = 255;
          color1.A = 255;
        }
        AhsvColor hsv0;
        AhsvColor hsv1;
        switch (Member)
        {
          case ColorMember.Alpha:
            color0.A = 0;
            color1.A = 255;
            break;
          case ColorMember.Red:
            color0.R = 0;
            color1.R = 255;
            break;
          case ColorMember.Green:
            color0.G = 0;
            color1.G = 255;
            break;
          case ColorMember.Blue:
            color0.B = 0;
            color1.B = 255;
            break;
          case ColorMember.Cyan:
            color0.G = 0;
            color1.G = 255;
            color0.B = 0;
            color1.B = 255;
            break;
          case ColorMember.Magenta:
            color0.R = 0;
            color1.R = 255;
            color0.B = 0;
            color1.B = 255;
            break;
          case ColorMember.Yellow:
            color0.R = 0;
            color1.R = 255;
            color0.G = 0;
            color1.G = 255;
            break;
          case ColorMember.Hue:
            hsv0 = color0.ToDrawingColor().ToAhsv();
            hsv1 = color1.ToDrawingColor().ToAhsv();
            hsv0.H=0;
            hsv1.H=1;
            color0 = hsv0.ToColor().ToMediaColor();
            color1 = hsv1.ToColor().ToMediaColor();
            break;
          case ColorMember.Saturation:
            hsv0 = color0.ToDrawingColor().ToAhsv();
            hsv1 = color1.ToDrawingColor().ToAhsv();
            hsv0.S=0;
            hsv1.S=1;
            color0 = hsv0.ToColor().ToMediaColor();
            color1 = hsv1.ToColor().ToMediaColor();
            break;
          case ColorMember.Brightness:
            hsv0 = color0.ToDrawingColor().ToAhsv();
            hsv1 = color1.ToDrawingColor().ToAhsv();
            hsv0.V=0;
            hsv1.V=1;
            color0 = hsv0.ToColor().ToMediaColor();
            color1 = hsv1.ToColor().ToMediaColor();
            break;
        }
        ColorSlider.Color0 = color0;
        ColorSlider.Color1 = color1;
      }
    }

    public event ValueChangedEventHandler<double> ValueChanged;

    #region Value property
    public double Value
    {
      get => (double)GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
      ("Value", typeof(double), typeof(ColorMemberEdit),
        new FrameworkPropertyMetadata(double.NaN, ValuePropertyChanged));

    private static void ValuePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = sender as ColorMemberEdit;
      if (c.ColorSlider!=null)
        c.ColorSlider.Position=(double)args.NewValue;
    }

    private void SetValue(Color color)
    {
      isColorNumBoxValueChanging = true;
      AhsvColor hsv;
      switch (Member)
      {
        case ColorMember.Alpha:
          Value = color.A/ValueScale;
          break;
        case ColorMember.Red:
          Value = color.R/ValueScale;
          break;
        case ColorMember.Green:
          Value = color.G/ValueScale;
          break;
        case ColorMember.Blue:
          Value = color.B/ValueScale;
          break;
        case ColorMember.Cyan:
          Value = ((int)color.G + (int)color.B)/2.0/ValueScale;
          break;
        case ColorMember.Magenta:
          Value = ((int)color.R + (int)color.B)/2.0/ValueScale;
          break;
        case ColorMember.Yellow:
          Value = ((int)color.R + (int)color.G)/2.0/ValueScale;
          break;
        case ColorMember.Hue:
          hsv = color.ToDrawingColor().ToAhsv();
          if (double.IsNaN(hsv.H))
            hsv.H=0;
          Value = hsv.H;
          break;
        case ColorMember.Saturation:
          hsv = color.ToDrawingColor().ToAhsv();
          Value = hsv.S;
          break;
        case ColorMember.Brightness:
          hsv = color.ToDrawingColor().ToAhsv();
          Value = hsv.V;
          break;
      }
      isColorNumBoxValueChanging = false;
    }
    #endregion

    private bool isColorSliderValueChanging;
    private bool isColorNumBoxValueChanging;
    private void ColorSlider_ValueChanged(object sender, ValueChangedEventArgs<Color> args)
    {
      isColorSliderValueChanging = true;
      if (!isColorNumBoxValueChanging)
      {
        ColorNumBox.Value = (decimal)(int)(ColorSlider.Position*ValueScale);
      }
      var color = SelectedColor;
      AhsvColor hsv;
      switch (Member)
      {
        case ColorMember.Alpha:
          color.A = (byte)(ColorSlider.Position*255);
          break;
        case ColorMember.Red:
          color.R = (byte)(ColorSlider.Position*255);
          break;
        case ColorMember.Green:
          color.G = (byte)(ColorSlider.Position*255);
          break;
        case ColorMember.Blue:
          color.B = (byte)(ColorSlider.Position*255);
          break;
        case ColorMember.Cyan:
          color.G = (byte)(ColorSlider.Position*255);
          color.B = (byte)(ColorSlider.Position*255);
          break;
        case ColorMember.Magenta:
          color.R = (byte)(ColorSlider.Position*255);
          color.B = (byte)(ColorSlider.Position*255);
          break;
        case ColorMember.Yellow:
          color.R = (byte)(ColorSlider.Position*255);
          color.G = (byte)(ColorSlider.Position*255);
          break;
        case ColorMember.Hue:
          hsv = color.ToDrawingColor().ToAhsv();
          hsv.H = ColorSlider.Position;
          color = hsv.ToColor().ToMediaColor();
          break;
        case ColorMember.Saturation:
          hsv = color.ToDrawingColor().ToAhsv();
          hsv.S = ColorSlider.Position;
          color = hsv.ToColor().ToMediaColor();
          break;
        case ColorMember.Brightness:
          hsv = color.ToDrawingColor().ToAhsv();
          hsv.V = ColorSlider.Position;
          color = hsv.ToColor().ToMediaColor();
          break;
      }
      SelectedColor = color;
      isColorSliderValueChanging = false;
    }

    private void ColorNumBox_ValueChanged(object sender, ValueChangedEventArgs<decimal> args)
    {
      if (args.NewValue==253)
        Debug.Assert(true);
      var value = ((double)args.NewValue)/ValueScale;
      isColorNumBoxValueChanging = true;
      if (!isColorSliderValueChanging)
      {
        ColorSlider.Position = value;
      }
      ValueChanged?.Invoke(this, new ValueChangedEventArgs<double>(value));
      isColorNumBoxValueChanging = false;
    }


  }
}

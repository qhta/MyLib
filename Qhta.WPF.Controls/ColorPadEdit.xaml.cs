using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Qhta.Drawing;

namespace Qhta.WPF.Controls
{

  public partial class ColorPadEdit : UserControl
  {
    public ColorPadEdit()
    {
      InitializeComponent();
    }

    public override void OnApplyTemplate()
    {
      ColorPad.ValueChanged+=ColorPad_ValueChanged;
      SSlider.ValueChanged+=SSlider_ValueChanged;
      VSlider.ValueChanged+=VSlider_ValueChanged;
    }

    //#region BaseColor property
    //public Color BaseColor
    //{
    //  get => (Color)GetValue(BaseColorProperty);
    //  set => SetValue(BaseColorProperty, value);
    //}

    //public static readonly DependencyProperty BaseColorProperty = DependencyProperty.Register
    //  ("BaseColor", typeof(Color), typeof(ColorPadEdit),
    //   new FrameworkPropertyMetadata(Colors.Transparent,
    //     FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
    //     BaseColorPropertyChanged));

    //private static void BaseColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    //{
    //  var c = sender as ColorPadEdit;
    //  c.SetColorPad((Color)args.NewValue);
    //}
    //#endregion

    #region SelectedColor property
    public Color SelectedColor
    {
      get => (Color)GetValue(SelectedColorProperty);
      set => SetValue(SelectedColorProperty, value);
    }

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
      ("SelectedColor", typeof(Color), typeof(ColorPadEdit),
       new FrameworkPropertyMetadata(Colors.Transparent,
         FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
         SelectedColorPropertyChanged));

    private static void SelectedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c= sender as ColorPadEdit;
      c.SetColorPad((Color)args.NewValue);
    }
    #endregion

    private void SetColorPad(Color baseColor)
    {
      var hsv = baseColor.ToDrawingColor().ToAhsv();
      double h = hsv.H;
      if (double.IsNaN(h))
        h = 0;
      var HSV = new AhsvColor(1, h, 0, 0);
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
      this.ColorPad.InvariantHue=(int)(h*360);
      this.ColorPad.SelectedColor = baseColor;
    }

    private void ColorPad_ValueChanged(object sender, RoutedPropertyChangedEventArgs<Point> args)
    {
      SelectedColor = ColorPad.Position2Color(args.NewValue);
    }
    private void SSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
    {
      var position = ColorPad.Position;
      position.X = args.NewValue;
      ColorPad.Position=position;
    }

    private void VSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
    {
      var position = ColorPad.Position;
      position.Y = args.NewValue;
      ColorPad.Position=position;
    }

  }
}

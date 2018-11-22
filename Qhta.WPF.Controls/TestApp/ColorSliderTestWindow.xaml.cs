using System.Windows;
using System.Windows.Media;

namespace TestApp
{

  public partial class ColorSliderTestWindow : Window
  {
    public ColorSliderTestWindow()
    {
      InitializeComponent();
      EditedColor = Color.FromArgb(255, 128, 128, 128);
    }

    public override void OnApplyTemplate()
    {
      HueSlider_ValueChanged();
      AlphaSlider_ValueChanged();
    }

    #region EditedColor property
    public Color EditedColor
    {
      get => (Color)GetValue(EditedColorProperty);
      set => SetValue(EditedColorProperty, value);
    }

    public static readonly DependencyProperty EditedColorProperty = DependencyProperty.Register
      ("EditedColor", typeof(Color), typeof(ColorSliderTestWindow),
        new PropertyMetadata(Colors.Transparent, EditedColorPropertyChanged));

    public static void EditedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = sender as ColorSliderTestWindow;
      c.EditedBrush = new SolidColorBrush(c.EditedColor);
    }
    #endregion

    #region EditedBrush property
    public Brush EditedBrush
    {
      get => (Brush)GetValue(EditedBrushProperty);
      set => SetValue(EditedBrushProperty, value);
    }

    public static readonly DependencyProperty EditedBrushProperty = DependencyProperty.Register
      ("EditedBrush", typeof(Brush), typeof(ColorSliderTestWindow),
        new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent),
          FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    #endregion

    private void OK_Click(object sender, RoutedEventArgs e)
    {
      EditedBrush = new SolidColorBrush(EditedColor);
      ColorRectangle.InvalidateVisual();
    }

    private void VerticalSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
    {
      HueSlider_ValueChanged();
    }

    private void HueSlider_ValueChanged()
    {
      if (HorizontalSlider!=null)
      {
        var pos = HorizontalSlider.Position;
        var endColor = VerticalSlider.SelectedColor;
        var startColor = Color.FromArgb(0, endColor.R, endColor.G, endColor.B);
        HorizontalSlider.Color0 = startColor;
        HorizontalSlider.Color1 = endColor;
        HorizontalSlider.SelectedColor = HorizontalSlider.Position2Color(pos);
      }
    }

    private void HorizontalSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      AlphaSlider_ValueChanged();
    }


    private void AlphaSlider_ValueChanged()
    {
      EditedColor = HorizontalSlider.SelectedColor;
      //EditedBrush = new SolidColorBrush();
    }
  }
}

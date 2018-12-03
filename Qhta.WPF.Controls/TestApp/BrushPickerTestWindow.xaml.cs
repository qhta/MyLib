using System.Windows;
using System.Windows.Media;

namespace TestApp
{

  public partial class BrushPickerTestWindow : Window
  {
    public BrushPickerTestWindow()
    {
      InitializeComponent();
      EditedColor = Color.FromArgb(255, 128, 128, 128);
    }

    #region EditedColor property
    public Color EditedColor
    {
      get => (Color)GetValue(EditedColorProperty);
      set => SetValue(EditedColorProperty, value);
    }

    public static readonly DependencyProperty EditedColorProperty = DependencyProperty.Register
      ("EditedColor", typeof(Color), typeof(BrushPickerTestWindow),
        new PropertyMetadata(Colors.Transparent, EditedColorPropertyChanged));

    public static void EditedColorPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      var c = sender as BrushPickerTestWindow;
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
      ("EditedBrush", typeof(Brush), typeof(BrushPickerTestWindow),
        new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
    #endregion

    private void OK_Click(object sender, RoutedEventArgs e)
    {
      EditedBrush = new SolidColorBrush(EditedColor);
      ColorRectangle.InvalidateVisual();
    }
  }
}

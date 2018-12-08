using System.Windows;
using System.Windows.Media;

namespace TestApp
{

  public partial class BrushPickerTestWindow : Window
  {
    public BrushPickerTestWindow()
    {
      InitializeComponent();
      EditedBrush = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
    }

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
      EditedBrush = EditedBrush;
      ColorRectangle.InvalidateVisual();
    }
  }
}

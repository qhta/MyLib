using System.Windows;

namespace TestApp
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void NumericUpDownTestButton_Click(object sender, RoutedEventArgs e)
    {
      new NumericEditBoxTestWindow().Show();
    }

    private void Border3DTestButton_Click(object sender, RoutedEventArgs e)
    {
      new Border3DTestWindow().Show();
    }

    private void ColorPickerTestButton_Click(object sender, RoutedEventArgs e)
    {
      new ColorPickerTestWindow().Show();
    }

    private void ColorSliderTestButton_Click(object sender, RoutedEventArgs e)
    {
      new ColorSliderTestWindow().Show();
    }

    private void ColorPadTestButton_Click(object sender, RoutedEventArgs e)
    {      
      new ColorPadTestWindow().Show();
    }

  }
}

using System.Windows;

namespace TestApp
{

  public partial class NumericEditBoxTestWindow : Window
  {
    public NumericEditBoxTestWindow()
    {
      InitializeComponent();
    }

    private void NumericEditBox_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<decimal> e)
    {
      ResultTextBox.Text = NumericEditBox.Value.ToString();
    }

    private void Increment10Button_Click(object sender, RoutedEventArgs e)
    {
      NumericEditBox.Increment=1.0m;
    }

    private void Increment01Button_Click(object sender, RoutedEventArgs e)
    {
      NumericEditBox.Increment=0.1m;
    }
  }
}

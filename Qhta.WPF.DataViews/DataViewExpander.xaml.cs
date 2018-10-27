using System.Windows;
using System.Windows.Controls;

namespace Qhta.WPF.DataViews
{
  /// <summary>
  /// Interaction logic for DataViewExpander.xaml
  /// </summary>
  public partial class DataViewExpander : UserControl
  {
    public DataViewExpander()
    {
      InitializeComponent();
    }

    public event RoutedEventHandler Expanded;

    public event RoutedEventHandler Collapsed;

    private void Expander_Expanded(object sender, RoutedEventArgs e)
    {
      Expanded?.Invoke(this, e);
    }

    private void Expander_Collapsed(object sender, RoutedEventArgs e)
    {
      Collapsed?.Invoke(this, e);
    }
  }
}

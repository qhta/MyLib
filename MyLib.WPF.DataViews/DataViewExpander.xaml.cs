using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyLib.WPF.DataViews
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

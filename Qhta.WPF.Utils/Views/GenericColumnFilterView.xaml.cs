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

namespace Qhta.WPF.Utils.Views;
/// <summary>
/// Interaction logic for GenericColumnFilterView.xaml
/// </summary>
public partial class GenericColumnFilterView : UserControl
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public GenericColumnFilterView()
  {
    InitializeComponent();
  }
  private void ColumnSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
  {
    if (DataContext is FilterViewModel viewModel &&
      args.AddedItems.Count == 1 && args.AddedItems[0] is FilterableColumnInfo info)
    {
      viewModel.Column = info;
    }
  }

}

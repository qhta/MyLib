using System.Windows;
using System.Windows.Controls;

using Qhta.UnicodeBuild.Helpers;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Views
{
  /// <summary>
  /// Interaction logic for UcdRangesView.xaml
  /// </summary>
  public partial class UcdRangesView : UserControl
  {
    public UcdRangesView()
    {
      InitializeComponent();
    }

    private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
    {
      LongTextColumn.DataGrid_OnQueryRowHeight(sender, e);
    }

    private void UcdRangeDataGrid_Loaded(object sender, RoutedEventArgs e)
    {
      if (UcdRangeDataGrid.Columns["Range"] is GridTextColumn rangeColumn)
      {
        UcdRangeDataGrid.SortComparers.Add(new SortComparer
        {
          PropertyName = "Range",
          Comparer = new RangeModelComparer()
        });
      }
    }

  }
}

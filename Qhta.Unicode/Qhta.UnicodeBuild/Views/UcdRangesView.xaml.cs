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
      UcdRangeDataGrid.CurrentCellValidating += DataGrid_CurrentCellValidating;
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

    private void DataGrid_CurrentCellValidating(object? sender, Syncfusion.UI.Xaml.Grid.CurrentCellValidatingEventArgs e)
    {
      if (e.NewValue!=null && e.Column.MappingName == "Range")
      {
        // Validate the Range value
        if (!RangeModel.TryParse(e.NewValue.ToString()!, out var range))
        {
          e.ErrorMessage = "Invalid range format. Expected format: XXXX..YYYY.";
          e.IsValid = false;
        }
        else if (range != null && range.End.HasValue && range.End < range.Start)
        {
          e.ErrorMessage = "End value must be greater than or equal to Start value.";
          e.IsValid = false;
        }
      }
    }
  }
}

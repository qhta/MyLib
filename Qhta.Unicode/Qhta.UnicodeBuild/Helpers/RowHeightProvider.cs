using Qhta.UnicodeBuild.ViewModels;
using Syncfusion.UI.Xaml.Grid;
using System.Windows.Media;

namespace Qhta.UnicodeBuild.Helpers;

public static class RowHeightProvider
{
  public static void OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    if (sender is SfDataGrid dataGrid && e.RowIndex > 0 && e.RowIndex <= dataGrid.View.Records.Count)
    {
      if (dataGrid.View.Records[e.RowIndex - 1].Data is IRowHeightProvider viewModel)
      {
        if (Double.IsNaN(viewModel.RowHeight))
          return;
        e.Height = viewModel.RowHeight;
        e.Handled = true;
      }
    }
  }
}
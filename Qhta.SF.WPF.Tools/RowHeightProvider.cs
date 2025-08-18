using System.Diagnostics;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Provides functionality to determine the height of rows in a data grid.
/// </summary>
/// <remarks>This class contains methods that interact with data grid events to set row heights based on the data
/// context.</remarks>
public static class RowHeightProvider
{
  /// <summary>
  /// Handles the query for row height in a data grid, setting the height based on the data context.
  /// </summary>
  /// <remarks>This method checks if the data context for the specified row index implements <see
  /// cref="IRowHeightProvider"/>. If so, and the row height is not set to <see cref="Double.NaN"/>, it assigns the row
  /// height from the data context to the event arguments and marks the event as handled.</remarks>
  /// <param name="sender">The source of the event, typically an instance of <see cref="SfDataGrid"/>.</param>
  /// <param name="e">The <see cref="QueryRowHeightEventArgs"/> containing event data, including the row index and a property to set the
  /// row height.</param>
  public static void OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    if (sender is SfDataGrid dataGrid)
    {
      int rowIndex = e.RowIndex;
      //Debug.WriteLine($"OnQueryRowHeight invoked for {dataGrid.Name} in row {rowIndex}");
      if (rowIndex > 0 && rowIndex <= dataGrid.View.Records.Count)
      {
        if (dataGrid.View.Records[rowIndex - 1].Data is IRowHeightProvider viewModel)
        {
          if (Double.IsNaN(viewModel.RowHeight))
            return;
          e.Height = viewModel.RowHeight;
          e.Handled = true;
        }
      }
    }
  }
}
using System.Diagnostics;
using System.Windows.Media;

using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Helpers;

public class GridLongTextColumn : GridTextColumn
{
  public static void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    //Debug.WriteLine($"DataGrid_OnQueryRowHeight invoked for row {e.RowIndex}");
    if (e.RowIndex > 0 && sender is SfDataGrid dataGrid)
    {
      if (dataGrid.View.Records[e.RowIndex - 1].Data is ILongTextViewModel viewModel)
      {
        var longTextColumns = dataGrid.Columns.OfType<GridLongTextColumn>();
        var maxRowHeight = 0.0;
        foreach (var longTextColumn in longTextColumns)
        {
          //Debug.WriteLine($"Found long text column: {longTextColumn.MappingName}");
          var longText = GetCellText(longTextColumn, dataGrid.View.Records[e.RowIndex - 1].Data);
          if (string.IsNullOrEmpty(longText))
            return;
          if (!viewModel.IsRowHeightExpanded)
            return;
          var maxWidth = longTextColumn.ActualWidth - 6;
          var formattedText = new FormattedText(
            longText,
            System.Globalization.CultureInfo.CurrentCulture,
            System.Windows.FlowDirection.LeftToRight, 
            new Typeface("Segoe UI"),
            12,
            Brushes.Black,
            new NumberSubstitution(),
            1);
          formattedText.MaxTextWidth = maxWidth;
          var cellHeight = formattedText.Height + 12; // Add some padding
          if (cellHeight > maxRowHeight)
            maxRowHeight = cellHeight;
        }
        e.Height = maxRowHeight;
        e.Handled = true;
      }
    }
  }

  private static string GetCellText(GridTextColumn column, object dataItem)
  {
    var propertyInfo = dataItem.GetType().GetProperty(column.MappingName);
    return propertyInfo?.GetValue(dataItem)?.ToString() ?? string.Empty;
  }

}
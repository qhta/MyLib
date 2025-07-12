using System.Diagnostics;
using System.Windows.Media;
using Qhta.MVVM;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public class LongTextColumn : GridTemplateColumn
{
  public static void OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    if (sender is SfDataGrid dataGrid && e.RowIndex > 0 && e.RowIndex <= dataGrid.View.Records.Count)
    {
      if (dataGrid.View.Records[e.RowIndex - 1].Data is ILongTextViewModel viewModel)
      {
        var longTextColumns = dataGrid.Columns.OfType<LongTextColumn>();
        var maxRowHeight = 0.0;
        foreach (var longTextColumn in longTextColumns)
        {
          //Debug.WriteLine($"Found long text column: {longTextColumn.MappingName}");
          var longText = GetCellText(longTextColumn, dataGrid.View.Records[e.RowIndex - 1].Data);
          if (string.IsNullOrEmpty(longText))
            return;
          if (!viewModel.IsLongTextExpanded)
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

  private static string GetCellText(GridColumn column, object dataItem)
  {
    var propertyInfo = dataItem.GetType().GetProperty(column.MappingName);
    return propertyInfo?.GetValue(dataItem)?.ToString() ?? string.Empty;
  }

}
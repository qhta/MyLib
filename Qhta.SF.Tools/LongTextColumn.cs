using System.Diagnostics;
using System.Windows.Media;

using Qhta.MVVM;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

/// <summary>
/// Represents a column in a data grid that adjusts its row height based on the content length.
/// </summary>
/// <remarks>The <see cref="LongTextColumn"/> class is designed to work with data grids where the row height needs
/// to be dynamically adjusted to fit the content of cells containing long text. It listens to the <see
/// cref="QueryRowHeightEventArgs"/> to determine the appropriate height for each row based on the text content and the
/// expansion state of the text.</remarks>
public class LongTextColumn : GridTemplateColumn
{
  /// <summary>
  /// Gets the default font name used by the application.
  /// </summary>
  public static string DefaultFont { get; set; } = "Segoe UI";
  /// <summary>
  /// Gets the default font size used by the application.
  /// </summary>
  public static double DefaultFontSize { get; set; } = 12;

  /// <summary>
  /// Adjusts the height of a row in a <see cref="SfDataGrid"/> based on the content of long text columns.
  /// Default font is <see cref="DefaultFont"/> and default font size is <see cref="DefaultFontSize"/>.
  /// These properties can be set to change the font used for text evaluation.
  /// </summary>
  /// <remarks>This method calculates the required height for a row by evaluating the content of columns that
  /// implement <see cref="LongTextColumn"/>. If the data associated with the row implements <see
  /// cref="ILongTextViewModel"/> and the long text is expanded, the method sets the row height to accommodate the text.
  /// The height is adjusted only if the row index is valid and the text is not empty. The method marks the event as
  /// handled after setting the height.</remarks>
  /// <param name="sender">The source of the event, typically an instance of <see cref="SfDataGrid"/>.</param>
  /// <param name="e">The event data containing information about the row height query.</param>
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
          var cellHeight = EvaluateTextHeight(longText, maxWidth) + +12; // Add some padding;
          if (cellHeight > maxRowHeight)
            maxRowHeight = cellHeight;
        }
        e.Height = maxRowHeight;
        e.Handled = true;
      }
    }
  }

  /// <summary>
  /// Evaluates the height of a given long text string based on a specified maximum width.
  /// </summary>
  /// <param name="longText">Text to evaluate</param>
  /// <param name="maxWidth">Maximum text width i one line</param>
  /// <param name="fontName">Name of the font used for evaluation</param>
  /// <param name="fontSize">Em size of the font used for evaluation</param>
  /// <returns></returns>
  public static double EvaluateTextHeight(string longText, double maxWidth, string fontName = "Segoe UI", double fontSize = 12.0 )
  {
    var formattedText = new FormattedText(
      longText,
      System.Globalization.CultureInfo.CurrentCulture,
      System.Windows.FlowDirection.LeftToRight,
      new Typeface(fontName),
      fontSize,
      Brushes.Black,
      new NumberSubstitution(),
      1);
    formattedText.MaxTextWidth = maxWidth;
    var textHeight = formattedText.Height;
    return textHeight;
  }

  /// <summary>
  /// Retrieves the text representation of a cell's value from a specified data item and column.
  /// </summary>
  /// <param name="column">The column that specifies which property of the data item to retrieve.</param>
  /// <param name="dataItem">The data item from which the cell value is extracted.</param>
  /// <returns>A string representation of the cell's value. Returns an empty string if the property is not found or the value is
  /// null.</returns>
  private static string GetCellText(GridColumn column, object dataItem)
  {
    var propertyInfo = dataItem.GetType().GetProperty(column.MappingName);
    return propertyInfo?.GetValue(dataItem)?.ToString() ?? string.Empty;
  }

}
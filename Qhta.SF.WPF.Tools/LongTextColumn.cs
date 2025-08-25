using System.Diagnostics;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Media;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;

namespace Qhta.SF.WPF.Tools;

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
  /// Initializes a new instance of the <see cref="LongTextColumn"/> class.
  /// </summary>
  public LongTextColumn()
  {
    CellTemplate = (DataTemplate)Application.Current.Resources["LongTextCellTemplate"]!;
    EditTemplate = (DataTemplate)Application.Current.Resources["LongTextEditTemplate"]!;
  }

  /// <summary>
  /// This method is needed to invalidate ShowPopupButton visibility when the ActualWidth changes.
  /// </summary>
  /// <param name="e"></param>
  protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
  {
    if (e.Property.Name == nameof(ActualWidth))
    {
      if (DataGrid != null)
      {
        // Access the visual container of the DataGrid
        var visualContainer = DataGrid.GetVisualContainer();
        if (visualContainer != null)
        {
          // Iterate through all visible rows using ScrollRows
          foreach (var row in visualContainer.ScrollRows.GetVisibleLines())
          {
            DataGrid.UpdateDataRow(row.LineIndex);
          }
        }
      }
    }
    base.OnPropertyChanged(e);
  }

  /// <summary>
  /// Adjusts the height of a row in a <see cref="SfDataGrid"/> based on the content of long text columns.
  /// </summary>
  /// <remarks>This method calculates the required height for a row by evaluating its text height</remarks>
  /// <param name="sender">The source of the event, typically an instance of <see cref="SfDataGrid"/>.</param>
  /// <param name="e">The event data containing information about the row height query.</param>
  public static void OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    if (sender is SfDataGrid dataGrid && e.RowIndex > 0 && e.RowIndex <= dataGrid.View.Records.Count)
    {
      var longTextColumns = dataGrid.Columns.OfType<LongTextColumn>();
      var maxRowHeight = 0.0;
      foreach (var column in longTextColumns)
      {
        //Debug.WriteLine($"Found long text column: {longTextColumn.MappingName}");
        var longText = GetCellText(column, dataGrid.View.Records[e.RowIndex - 1].Data);
        if (string.IsNullOrEmpty(longText))
          return;
        var cellHeight = column.EvaluateTextHeight(longText) + 12; // Add some padding;
        if (cellHeight > maxRowHeight)
          maxRowHeight = cellHeight;
      }
      e.Height = maxRowHeight;
      e.Handled = true;
    }
  }

  /// <summary>
  /// Evaluates the height of a given long text string based on a specified maximum width.
  /// </summary>
  /// <param name="longText">Text to evaluate</param>
  /// <returns></returns>
  public double EvaluateTextHeight(string longText)
  {
    //Note: We assume that the font name and size of the column are the same as the whole grid has.
    var dataGrid = this.DataGrid;
    string fontName = dataGrid.FontFamily.Source;
    double fontSize = dataGrid.FontSize;
    var formattedText = new FormattedText(
      longText,
      System.Globalization.CultureInfo.CurrentCulture,
      System.Windows.FlowDirection.LeftToRight,
      new Typeface(fontName),
      fontSize,
      Brushes.Black,
      new NumberSubstitution(),
      1);
    formattedText.MaxTextWidth = ActualWidth - 6; // Subtract some padding
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
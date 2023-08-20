namespace Qhta.WPF.Utils;

/// <summary>
/// Utility class that contains extension methods for DataGridColumn.
/// </summary>
public static class DataGridColumnUtils
{

  /// <summary>
  /// Gets a header text of the column. 
  /// Header text can be declared directly as string, as a text of TextBlock or as string content of Label.
  /// </summary>
  /// <param name="column"></param>
  /// <returns></returns>
  public static string? GetHeaderText(this DataGridColumn column)
  {
    var header = column.Header;
    if (header is string headerString)
    {
      if (!string.IsNullOrEmpty(headerString))
        return headerString;
    }
    else
    if (header is TextBlock textBlock)
    {
      headerString = textBlock.Text;
      if (!string.IsNullOrEmpty(headerString))
        return headerString;
    }
    else
    if (header is Label label)
    {
      headerString = label.Content as string ?? "";
      if (!string.IsNullOrEmpty(headerString))
        return headerString;
    }
    return null;
  }

  /// <summary>
  /// Gets binding from DataGridBoundColumn and DataGridComboBoxColumn.
  /// </summary>
  /// <param name="column"></param>
  /// <returns></returns>
  public static BindingBase? GetBinding(this DataGridColumn column)
  {
    if (column is DataGridBoundColumn boundColumn)
      return boundColumn.Binding;
    else
    if (column is DataGridComboBoxColumn comboBoxColumn)
      return comboBoxColumn.SelectedItemBinding ?? comboBoxColumn.SelectedValueBinding
        ?? comboBoxColumn.TextBinding ?? comboBoxColumn.ClipboardContentBinding;
    return null;
  }
}

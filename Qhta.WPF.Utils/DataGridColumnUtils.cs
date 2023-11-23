using System.Diagnostics.CodeAnalysis;

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
  public static Binding? GetBinding(this DataGridColumn column)
  {
    return GetBindingBase(column) as Binding;
  }

  /// <summary>
  /// Gets binding base from DataGridBoundColumn and DataGridComboBoxColumn.
  /// </summary>
  /// <param name="column"></param>
  /// <returns></returns>
  public static BindingBase? GetBindingBase(this DataGridColumn column)
  {
    if (column is DataGridBoundColumn boundColumn)
      return boundColumn.Binding;
    else
    if (column is DataGridComboBoxColumn comboBoxColumn)
      return comboBoxColumn.SelectedItemBinding ?? comboBoxColumn.SelectedValueBinding
        ?? comboBoxColumn.TextBinding ?? comboBoxColumn.ClipboardContentBinding;
    return null;
  }

  /// <summary>
  /// Tries to gets a property path to the DataGridColumn.
  /// </summary>
  /// <param name="column"></param>
  /// <param name="itemType"></param>
  /// <param name="propPath">created PropPath</param>
  /// <returns>true on success </returns>
  public static bool TryGetFilterablePropertyPath(this DataGridColumn column, Type itemType, 
    [NotNullWhen(true)] out PropPath? propPath)
  {
    var path = column.GetBinding()?.Path?.Path;
    if (path != null)
    {
      propPath = new PropPath(itemType, path);
      var propertyInfo = propPath.Last();
      var valueType = propertyInfo.GetType();
      if (valueType.IsClass && valueType != typeof(string) && column.SortMemberPath != null && itemType != null)
      {
        valueType = itemType;
        path = column.SortMemberPath;
        var pathStrs = path.Split('.');
        propPath.Clear();
        foreach (var pathStr in pathStrs)
        {
          propertyInfo = valueType.GetProperty(pathStr);
          if (propertyInfo != null)
          {
            propPath.Add(propertyInfo);
            valueType = propertyInfo.PropertyType;
          }
        }
      }
      return true;
    }
    propPath = null;
    return false;
  }
}

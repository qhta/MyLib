using System.Collections;
using System.Diagnostics;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

/// <summary>
/// Controller class for managing SfDataGrid operations.
/// </summary>
public static partial class Controller
{

  private static Type? GetRowDataType(SfDataGrid grid)
  {
    var itemsSource = grid.View?.SourceCollection;
    if (itemsSource == null)
    {
      Debug.WriteLine("ItemsSource is null.");
      return null;
    }
    var itemType = TypeUtils.TypeUtils.GetElementType(itemsSource.GetType());
    return itemType;
  }

  private static GridColumnInfo?[] GetGridColumnInfos(GridColumn[] columns, Type rowDataType, bool write)
  {
    var columnInfos = columns.Select(column =>
    {
      var mappingPropertyInfo = column.GetType().GetProperty("MappingName");
      if (mappingPropertyInfo == null)
      {
        Debug.WriteLine($"Property 'MappingName' not found in column type '{column.GetType().Name}'.");
        return null;
      }
      var mappingName = (string?)mappingPropertyInfo.GetValue(column);
      if (mappingName == null)
      {
        Debug.WriteLine($"Mapping name is null for column '{column.HeaderText ?? column.MappingName}'.");
        return null;
      }
      var valuePropertyInfo = rowDataType.GetProperty(mappingName);
      if (valuePropertyInfo == null)
      {
        Debug.WriteLine($"Property '{mappingName} not found in column type '{rowDataType}'.");
        return null;
      }
      GridColumnInfo gridColumnInfo = new GridColumnInfo(column, mappingName, valuePropertyInfo);
      if (column is GridComboBoxColumn comboBoxColumn)
      {
        var displayMemberPath = comboBoxColumn.DisplayMemberPath;
        if (!string.IsNullOrEmpty(displayMemberPath))
        {
          var displayPropertyInfo = valuePropertyInfo.PropertyType.GetProperty(displayMemberPath);
          if (displayPropertyInfo == null)
          {
            Debug.WriteLine($"Display property '{displayMemberPath}' not found in type '{valuePropertyInfo.PropertyType.Name}'.");
            return null;
          }
          gridColumnInfo.DisplayPropertyInfo = displayPropertyInfo;
        }
        if (write)
        {
          gridColumnInfo.ItemsSource = comboBoxColumn.ItemsSource;
        }
      }


      return gridColumnInfo;
    }).Where(info => info != null).ToArray();
    return columnInfos;
  }

  private static string[] GetHeaders(SfDataGrid grid, GridColumn[] columns)
  {
    List<string> result = new();
    foreach (var column in columns)
    {
      if (column.HeaderText != null)
        result.Add(column.HeaderText);
      else
        result.Add(column.MappingName);
    }
    return result.ToArray();
  }

  private static string? GetCellData(GridCellInfo cellInfo, GridColumnInfo columnInfo)
  {
    var rowData = cellInfo.RowData;
    if (rowData == null)
    {
      Debug.WriteLine("Row data is null.");
      return null;
    }
    var column = columnInfo.Column;
    var rowDataType = rowData.GetType();
    var propertyInfo = columnInfo.ValuePropertyInfo;

    //if (columnInfo.MappingName=="Category")
    //  Debug.Assert(true);
    var str = "";
    var cellValue = propertyInfo.GetValue(rowData);
    if (columnInfo.DisplayPropertyInfo != null)
    {
      if (cellValue != null)
      {
        var val = columnInfo.DisplayPropertyInfo.GetValue(cellValue);
        str = (val is string str1) ? str1 : val?.ToString() ?? string.Empty;
      }
    }
    else str = cellValue?.ToString() ?? string.Empty;
    //Debug.Write($"{columnInfo.MappingName} = {str}");
    return str;
  }

  private static void SetCellData(GridCellInfo cellInfo, GridColumnInfo columnInfo, string? str)
  {
    var rowData = cellInfo.RowData;
    if (rowData == null)
    {
      Debug.WriteLine("Row data is null.");
      return;
    }
    var column = columnInfo.Column;
    var rowDataType = rowData.GetType();
    var propertyInfo = columnInfo.ValuePropertyInfo;
    if (!propertyInfo.CanWrite)
      return;
    object? value = str;
    if (columnInfo.ItemsSource!=null && !String.IsNullOrEmpty(str))
    {
      if (columnInfo.DisplayPropertyInfo != null)
        value = columnInfo.ItemsSource.Cast<object>()
                  .FirstOrDefault(item => columnInfo.DisplayPropertyInfo.GetValue(item)?.ToString() == str);
      else
        // If no display property, use the ToString() method of the item
        value = columnInfo.ItemsSource.Cast<object>()
          .FirstOrDefault(item => item.ToString() == str);
      if (value==null)
      {
        Debug.WriteLine($"Value '{str}' not found in items source for column '{columnInfo.MappingName}'.");
        return;
      }
    }
    if (value==null && propertyInfo.PropertyType.IsValueType)
    {
      // If the property is a value type, we cannot set it to null
      // Set it to the default value of the type
      value = Activator.CreateInstance(propertyInfo.PropertyType);
    }
    else if (value == null && propertyInfo.PropertyType != typeof(string))
    {
      // If the property is not a string, and we have a null value, we cannot set it
      Debug.WriteLine($"Cannot set null value for non-string property '{columnInfo.MappingName}'.");
      return;
    }
    if (value != null && !propertyInfo.PropertyType.IsInstanceOfType(value))
    {
      // If the value is not of the correct type, we need to convert it
      try
      {
        value = Convert.ChangeType(value, propertyInfo.PropertyType);
      }
      catch (InvalidCastException ex)
      {
        Debug.WriteLine($"Cannot convert value '{value}' to type '{propertyInfo.PropertyType.Name}': {ex.Message}");
        return;
      }
    }
    propertyInfo.SetValue(rowData, value);
  }
}
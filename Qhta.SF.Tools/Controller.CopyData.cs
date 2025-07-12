using System.Diagnostics;
using System.Reflection;
using System.Windows;

using Qhta.SF.Tools;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.ST.Tools;

public static class Controller
{

  private record GridColumnInfo(GridColumn Column, string MappingName, PropertyInfo ValuePropertyInfo, PropertyInfo? DisplayPropertyInfo)
  {
    public GridColumn Column { get; } = Column;
    public string MappingName { get; } = MappingName;
    public PropertyInfo ValuePropertyInfo { get; } = ValuePropertyInfo;
    public PropertyInfo? DisplayPropertyInfo { get; } = DisplayPropertyInfo;
  }

  public static void CopyData(SfDataGrid grid)
  {
    try
    {
      var selectedCells = grid.GetSelectedCells().ToArray();
      GridColumn[] columnsToCopy;
      if (selectedCells.Length != 0)
        columnsToCopy = selectedCells.Select(cell => cell.Column).Distinct().ToArray();
      else
        columnsToCopy = grid.Columns.Where(SfDataGridColumnBehavior.GetIsSelected).ToArray();
      if (!columnsToCopy.Any())
      {
        columnsToCopy = grid.Columns.ToArray();
      }

      object[] rowsToCopy;
      if (selectedCells.Length != 0)
        rowsToCopy = selectedCells.Select(cell => cell.RowData).Distinct().ToArray();
      else
        rowsToCopy = grid.SelectionController.SelectedRows.Select(row => row.RowData).ToArray();
      if (!rowsToCopy.Any())
      {
        rowsToCopy = grid.View.Records.Select(record => record.Data).ToArray();
      }
      var rowDataType = rowsToCopy.FirstOrDefault()?.GetType();


      GridColumnInfo?[]? columnInfos = null;
      if (rowDataType != null)
      {
        columnInfos = GetGridColumnInfos(columnsToCopy, rowDataType);
      }
      var content = new List<string>();

      var headers = GetHeaders(grid, columnsToCopy);
      var headerLine = string.Join("\t", headers);
      content.Add(headerLine);
      ;
      //Debug.WriteLine($"{headerLine}'.");
      Clipboard.SetText(headerLine);

      if (columnInfos != null && columnInfos.Any())
      {
        //await Task.Factory.StartNew(() =>
        //{

        foreach (var row in rowsToCopy)
        {
          var cellValues = columnsToCopy.Select(column =>
          {
            var cellInfo = new GridCellInfo(column, row, null, -1, false);
            var columnInfo = columnInfos?.FirstOrDefault(info => info?.MappingName == column.MappingName);
            var cellData = columnInfo != null ? GetCellData(cellInfo, columnInfo) : null;
            return cellData?.ToString() ?? string.Empty;
          }).ToArray();
          var line = string.Join("\t", cellValues);
          //Debug.WriteLine($"{line}");
          content.Add(line);

        }
        Clipboard.Clear();
        var text = string.Join(Environment.NewLine, content);
        Clipboard.SetText(text, TextDataFormat.Text);
        //});
        Debug.WriteLine($"Copy data completed");
      }
    } catch (Exception e)
    {
      Console.WriteLine(e);
    }
  }

  private static GridColumnInfo?[] GetGridColumnInfos(GridColumn[] columnsToCopy, Type rowDataType)
  {
    GridColumnInfo?[] columnInfos;
    columnInfos = columnsToCopy.Select(column =>
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
      PropertyInfo? displayPropertyInfo = null;
      if (column is GridComboBoxColumn comboBoxColumn)
      {
        var displayMemberPath = comboBoxColumn.DisplayMemberPath;
        if (!string.IsNullOrEmpty(displayMemberPath))
        {
          displayPropertyInfo = valuePropertyInfo.PropertyType.GetProperty(displayMemberPath);
          if (displayPropertyInfo == null)
          {
            Debug.WriteLine($"Display property '{displayMemberPath}' not found in type '{valuePropertyInfo.PropertyType.Name}'.");
            return null;
          }
          return new GridColumnInfo(column, mappingName, valuePropertyInfo, displayPropertyInfo);
        }
      }
      return new GridColumnInfo(column, mappingName, valuePropertyInfo, displayPropertyInfo);
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

  private static object? GetCellData(GridCellInfo cellInfo, GridColumnInfo columnInfo)
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
}
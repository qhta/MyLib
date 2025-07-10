using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Threading;

using Qhta.MVVM;
using Qhta.UnicodeBuild.Helpers;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{

  public void CopyData(SfDataGrid dataGrid)
  {
    //var selectedCells = dataGrid.GetSelectedCells();
    //if (selectedCells.Count == 0)
    //  dataGrid.SelectionController.SelectAll();

    dataGrid.GridCopyPaste.Copy();

    IDataObject? dataObject = null; 
     for (int i = 0; i < 1000; i++)
     {
       try
       {
         dataObject = Clipboard.GetDataObject();
       }
       catch (Exception ex)
       {
         Debug.WriteLine($"Error getting clipboard data object: {ex.Message}");
         // Wait a bit and try again
         System.Threading.Thread.Sleep(100);
      } 
    }
    if (dataObject == null)
    {
      Debug.WriteLine("Clipboard data object is null.");
      return;
    }
    var formats = dataObject.GetFormats();
    if (formats == null || formats.Length == 0)
    {
      Debug.WriteLine("Clipboard data object has no formats.");
      return;
    }
    var unicodeTextFormat = formats.FirstOrDefault(f => f == DataFormats.UnicodeText);
    if (unicodeTextFormat != null)
    {
      var text = dataObject.GetData(unicodeTextFormat) as string;
      Debug.WriteLine($"Unicode text is:");
      Debug.WriteLine(text);
      Clipboard.Clear();
      if (text == null)
      {
        Debug.WriteLine("Unicode text is null.");
        return;
      }
      Clipboard.SetText(text, TextDataFormat.UnicodeText);
    }
  }

  public object? GetCellData(GridCellInfo cellInfo)
  {
    var rowData = cellInfo.RowData;
    if (rowData == null)
    {
      Debug.WriteLine("Row data is null.");
      return null;
    }
    var column = cellInfo.Column;
    if (column == null)
    {
      Debug.WriteLine("Column is null.");
      return null;
    }
    var rowDataType = rowData.GetType();
    var propertyInfo = rowDataType.GetProperty(column.MappingName);
    if (propertyInfo == null)
    {
      Debug.WriteLine($"Property '{column.MappingName}' not found in row data type '{rowDataType.Name}'.");
      return null;
    }
    var cellValue = propertyInfo.GetValue(rowData);
    if (cellValue == null)
    {
      Debug.WriteLine("Cell value is null.");
      return null;
    }
    if (column is GridComboBoxColumn comboBoxColumn)
    {
      var displayMemberPath = comboBoxColumn.DisplayMemberPath;
      if (!String.IsNullOrEmpty(displayMemberPath))
      {
        var displayPropertyInfo = cellValue.GetType().GetProperty(displayMemberPath);
        if (displayPropertyInfo != null)
        {
          cellValue = displayPropertyInfo.GetValue(cellValue);
          if (cellValue == null)
          {
            Debug.WriteLine($"Display property '{displayMemberPath}' is null.");
            return null;
          }
        }
        else
        {
          Debug.WriteLine($"Display property '{displayMemberPath}' not found in row data type '{rowDataType.Name}'.");
          return null;
        }
      }
    }
    return cellValue?.ToString();
    throw new NotImplementedException("This method is not fully implemented yet. Please implement the logic to retrieve the cell data based on the property info.");
  }
}
using System.Collections;
using System.Diagnostics;

using Qhta.MVVM;
using Qhta.SF.Tools;
using Qhta.UnicodeBuild.Views;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  /// <summary>
  /// Command to fill the current column in the data grid with a selected value.
  /// </summary>
  public static IRelayCommand FillColumnCommand { [DebuggerStepThrough] get; } = new RelayCommand<object>(FillColumnCommandExecute, FillColumnCommandCanExecute);

  public static bool FillColumnCommandCanExecute(object? sender)
  {
    Debug.WriteLine($"FillColumnCommandCanExecute({sender})");
    if (sender is not SfDataGrid dataGrid)
    {
      Debug.WriteLine("FillColumnCommandCanExecute: Sender is not a SfDataGrid.");
      return false;
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns,
      out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      Debug.WriteLine("FillColumnCommand: No column selected or multiple columns selected.");
      return false;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var firstItem = selectedRows.FirstOrDefault();
      if (firstItem == null)
      {
        Debug.WriteLine("FillColumnCommand: No rows selected.");
        return false;
      }
      Debug.WriteLine($"FillColumnCommandCanExecute: Column {column.MappingName} is selected.");
      return true;
    }
    Debug.WriteLine("FillColumnCommandCanExecute: No column selected.");
    return false;
  }

  public static void FillColumnCommandExecute(object? sender)
  {
    if (sender is SfDataGrid dataGrid)
    {
      var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
      if (allColumnsSelected || selectedColumns.Length != 1)
      {
        Debug.WriteLine("FillColumnCommand: No column selected or multiple columns selected.");
        return;
      }
      var column = selectedColumns.FirstOrDefault();
      if (column != null)
      {
        var firstItem = selectedRows.FirstOrDefault();
        if (firstItem == null)
        {
          Debug.WriteLine("FillColumnCommand: No rows selected.");
          return;
        }
        if (column is GridComboBoxColumn comboBoxColumn)
        {
          var mappingName = comboBoxColumn.MappingName;
          var property = firstItem.GetType().GetProperty(mappingName);
          if (property == null) return;
          var propertyType = property.PropertyType;
          var itemsSource = comboBoxColumn.ItemsSource;
          var selectValueWindow = new SelectValueWindow
          {
            Prompt = String.Format(Resources.Strings.SelectValueTitle, column.HeaderText),
            ItemsSource = itemsSource
          };
          if (selectValueWindow.ShowDialog() == true)
          {
            var selectedValue = selectValueWindow.SelectedItem;
            var emptyCellsOnly = selectValueWindow.EmptyCellsOnly;
            if (selectedValue != null)
            {
              //Debug.WriteLine($"Setting column: {mappingName}, Selected Value: {selectedValue}");
              foreach (var record in selectedRows)
              {
                if (emptyCellsOnly)
                {
                  var currentValue = property.GetValue(record);
                  if (currentValue == null)
                    property.SetValue(record, selectedValue);
                }
                else
                {
                  property.SetValue(record, selectedValue);
                }
              }
            }
          }
        }
      }
    }
  }
}
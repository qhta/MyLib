using System.Collections;
using System.Diagnostics;
using System.Windows.Input;

using Qhta.MVVM;
using Qhta.SF.Tools.Resources;
using Qhta.SF.Tools.Views;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;


/// <summary>
/// Command to fill the current column in the data grid with a value selected by user.
/// </summary>
public class FillColumnCommand : Command
{
  /// <summary>
  /// Checks whether the command can be executed basing on the current selection in the data grid.
  /// </summary>
  /// <param name="parameter"></param>
  /// <returns></returns>
  public override bool CanExecute(object? parameter)
  {
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FillColumnCommand: No column selected or multiple columns selected.");
      return false;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var firstItem = selectedRows.FirstOrDefault();
      if (firstItem == null)
      {
        //Debug.WriteLine("FillColumnCommand: No rows selected.");
        return false;
      }
      //Debug.WriteLine($"FillColumnCommandCanExecute: Column {column.MappingName} is selected.");
      return true;
    }
    //Debug.WriteLine("FillColumnCommandCanExecute: No column selected.");
    return false;

  }

  /// <summary>
  /// Executes the command basing on the current selection in the data grid.
  /// </summary>
  /// <param name="parameter"></param>
  public override void Execute(object? parameter)
  {
    if (!CanExecute(parameter))
    {
      //Debug.WriteLine("FillColumnCommand: Cannot execute command.");
      return;
    }
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FillColumnCommand: No column selected or multiple columns selected.");
      return;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var firstItem = selectedRows.FirstOrDefault();
      if (firstItem == null)
      {
        //Debug.WriteLine("FillColumnCommand: No rows selected.");
        return;
      }
      if (column is GridComboBoxColumn comboBoxColumn)
      {
        var mappingName = comboBoxColumn.MappingName;
        var property = firstItem.GetType().GetProperty(mappingName);
        if (property == null) return;
        var propertyType = property.PropertyType;
        var itemsSource = comboBoxColumn.ItemsSource;
        var selectValueWindow = new SpecificValueWindow
        {
          Prompt = String.Format(Strings.SelectValueForField, column.HeaderText),
          ItemsSource = itemsSource,
          WindowMode = SpecificWindowMode.Fill,
        };
        if (selectValueWindow.ShowDialog() == true)
        {
          var selectedValue = selectValueWindow.SelectedItem;
          var OverwriteNonEmptyCells = selectValueWindow.OverwriteNonEmptyCells;
          if (selectedValue != null)
          {
            //Debug.WriteLine($"Setting column: {mappingName}, Selected Value: {selectedValue}");
            foreach (var record in selectedRows)
            {
              if (OverwriteNonEmptyCells)
              {
                property.SetValue(record, selectedValue);
              }
              {
                var currentValue = property.GetValue(record);
                if (currentValue == null || currentValue is string str && String.IsNullOrEmpty(str))
                  property.SetValue(record, selectedValue);
              }
            }
          }
        }
      }
    }
  }
}


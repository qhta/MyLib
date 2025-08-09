using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Qhta.MVVM;
using Qhta.SF.Tools;
using Qhta.UnicodeBuild.Resources;
using Qhta.UnicodeBuild.Views;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;

namespace Qhta.UnicodeBuild.Commands;


/// <summary>
/// Command to find a value selected by the user int the current column in the data grid.
/// </summary>
public class FindCommand : Command
{
  /// <summary>
  /// Specifies the last data grid where the find operation was performed.
  /// </summary>
  public SfDataGrid? LastDataGrid { get; private set; }
  /// <summary>
  /// Specifies the last column where the find operation was performed.
  /// </summary>
  public GridColumn? LastColumn { get; private set; }
  /// <summary>
  /// Value of the last search value in the column.
  /// </summary>
  public object? LastValue { get; private set; }

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
      //Debug.WriteLine("FindInColumnCommand: No column selected or multiple columns selected.");
      return false;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var firstItem = selectedRows.FirstOrDefault();
      if (firstItem == null)
      {
        //Debug.WriteLine("FindInColumnCommand: No rows selected.");
        return false;
      }
      //Debug.WriteLine($"FindInColumnCommandCanExecute: Column {column.MappingName} is selected.");
      return true;
    }
    //Debug.WriteLine("FindInColumnCommandCanExecute: No column selected.");
    return false;
  }

  /// <summary>
  /// Checks whether the FindNext command can be executed basing on the current selection in the data grid.
  /// </summary>
  /// <param name="parameter"></param>
  /// <returns></returns>
  public bool CanExecuteFindNext(object? parameter)
  {
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    if (LastDataGrid != dataGrid)
    {
      //Debug.WriteLine("FindInColumnCommand: LastDataGrid does not match the current data grid.");
      return false;
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FindInColumnCommand: No column selected or multiple columns selected.");
      return false;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      if (LastColumn != column)
      {
        //Debug.WriteLine("FindInColumnCommand: LastColumn does not match the current column.");
        return false;
      }
      var firstItem = selectedRows.FirstOrDefault();
      if (firstItem == null)
      {
        //Debug.WriteLine("FindInColumnCommand: No rows selected.");
        return false;
      }
      //Debug.WriteLine($"FindInColumnCommandCanExecute: Column {column.MappingName} is selected.");
      return true;
    }
    //Debug.WriteLine("FindInColumnCommandCanExecute: No column selected.");
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
      //Debug.WriteLine("FindInColumnCommand: Cannot execute command.");
      return;
    }
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FindInColumnCommand: No column selected or multiple columns selected.");
      return;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var columnIndex = dataGrid.Columns.IndexOf(column);
      if (column is GridComboBoxColumn comboBoxColumn)
      {
        var mappingName = comboBoxColumn.MappingName;
        var property = Controller.GetRowDataType(dataGrid)?.GetProperty(mappingName);
        if (property == null) return;
        var propertyType = property.PropertyType;
        var itemsSource = comboBoxColumn.ItemsSource;
        var selectValueWindow = new SelectValueWindow
        {
          Prompt = String.Format(Resources.Strings.SelectValueForField, column.HeaderText),
          ItemsSource = itemsSource,
          ShowOverwriteNonEmptyCells = false,
          ShowFindInSequence = true,
        };
        if (selectValueWindow.ShowDialog() == true)
        {
          var specifiedValue = selectValueWindow.SelectedItem;
          LastDataGrid = dataGrid;
          LastColumn = column;
          LastValue = specifiedValue;
          var findInSequence = selectValueWindow.FindInSequence;
          if (specifiedValue != null)
          {

            switch (findInSequence)
            {
              case FindInSequence.FindNext:
                var selectedRow = dataGrid.CurrentItem;
                if (selectedRow == null)
                  goto FindFirst;
                if (!column.FindNextValue(specifiedValue, selectedRow))
                  MessageBox.Show(Strings.NotMoreValueFound);
                break;
              case FindInSequence.FindFirst:
              FindFirst:
                if (!column.FindFirstValue(specifiedValue))
                  MessageBox.Show(Strings.ValueNotFound);
                break;
              case FindInSequence.FindAll:
                if (!column.FindFirstValue(specifiedValue))
                  MessageBox.Show(Strings.ValueNotFound);
                break;

            }
          }
        }
      }
    }
  }

  /// <summary>
  /// Executes the FindNext command basing on the current selection in the data grid.
  /// </summary>
  /// <param name="parameter"></param>
  public void ExecuteFindNext(object? parameter)
  {
    if (!CanExecuteFindNext(parameter))
    {
      //Debug.WriteLine("FindInColumnCommand: Cannot execute command.");
      return;
    }
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    var cells = dataGrid.GetSelectedRowsAndColumns(out var allColumnsSelected, out var selectedColumns, out var allRowsSelected, out var selectedRows);
    if (allColumnsSelected || selectedColumns.Length != 1)
    {
      //Debug.WriteLine("FindInColumnCommand: No column selected or multiple columns selected.");
      return;
    }
    var column = selectedColumns.FirstOrDefault();
    if (column != null)
    {
      var columnIndex = dataGrid.Columns.IndexOf(column);
      if (column is GridComboBoxColumn comboBoxColumn)
      {
        var mappingName = comboBoxColumn.MappingName;
        var property = Controller.GetRowDataType(dataGrid)?.GetProperty(mappingName);
        if (property == null) return;
        var propertyType = property.PropertyType;
        var itemsSource = comboBoxColumn.ItemsSource;
        var selectValueWindow = new SelectValueWindow
        {
          Prompt = String.Format(Resources.Strings.SelectValueForField, column.HeaderText),
          ItemsSource = itemsSource,
          ShowOverwriteNonEmptyCells = false,
          ShowFindInSequence = true,
        };
        var specifiedValue = LastValue;
        var selectedRow = selectedRows.FirstOrDefault();
        if (!column.FindNextValue(specifiedValue, selectedRow))
          MessageBox.Show(Strings.NotMoreValueFound);
      }
    }
  }
}

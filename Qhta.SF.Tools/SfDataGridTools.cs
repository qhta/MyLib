using System.Windows;
using System.Windows.Input;

using Qhta.WPF.Utils;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

/// <summary>
/// Provides utility methods for handling mouse interactions with the header and row header cells in a <see
/// cref="SfDataGrid"/>.
/// </summary>
/// <remarks>This class contains methods that manage the selection behavior of columns and rows when interacting
/// with the grid's header cells. It supports features such as toggling column selection and clearing or setting
/// selections based on user input.</remarks>
public partial class SfDataGridTools : ResourceDictionary
{

  /// <summary>
  /// Handles the mouse left button down event on a <see cref="GridHeaderCellControl"/>. This method toggles the
  /// IsSelected state of the column when the header cell is clicked. It also manages the selection of multiple
  /// columns based on the Shift and Control keys pressed during the click event.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  /// <remarks>If the column enables filtering or sorting, then selection is possible only if the mouse click
  /// position omits filter icon or sorting icon on the right of the header control. This condition allows the method
  /// to be handled on th PreviewMouseLeftButtonDown event of the header cell control, ensuring that the sorting
  /// functionality is not triggered when the user intends to select the column instead.
  /// </remarks>
  private void GridHeaderCellControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    //Debug.WriteLine($"Grid_OnPreviewMouseLeftButtonDown({sender})");



    if (sender is GridHeaderCellControl headerCellControl)
    {
      var column = headerCellControl.Column;
      if (column == null) return;
      var grid = headerCellControl.FindParent<SfDataGrid>();
      if (grid == null) return;
      if (column.AllowFiltering || column.AllowSorting)
      {
        int limit = 0;
        if (column.AllowFiltering)
        {
          // If the column allows filtering, we can check if the mouse is on the filter icon
          limit += 20; // Assuming the filter icon is 20px wide
        }
        if (column.AllowSorting)
        {
          // If the column allows sorting, we can check if the mouse is on the sort icon
          limit += 20; // Assuming the sort icon is 20px wide
        }
        var mousePosition = e.GetPosition(headerCellControl);
        if (mousePosition.X >= headerCellControl.ActualWidth - limit)
        {
          // If mouse is on the filter icon, do open filter popup instead of selecting the column
          return;
        }
      }

      var isSelected = SfDataGridColumnBehavior.GetIsSelected(column);
      isSelected = !isSelected;

      if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
      {
        // Clear selection if Shift or Control is not pressed
        foreach (var col in grid.Columns)
        {
          if (col != column) SfDataGridColumnBehavior.SetIsSelected(col, false);
        }
      }
      if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && isSelected)
      {
        int selectedColumnIndex = grid.Columns.IndexOf(column);
        int? lastPreviousSelectedColumnIndex = null;
        int? firstNextSelectedColumnIndex = null;
        for (int i = 0; i < grid.Columns.Count; i++)
        {
          var col = grid.Columns[i];
          if (SfDataGridColumnBehavior.GetIsSelected(col))
          {
            if (i < selectedColumnIndex) lastPreviousSelectedColumnIndex = i;
            else if (i > selectedColumnIndex) firstNextSelectedColumnIndex = i;
          }
        }
        if (lastPreviousSelectedColumnIndex != null || firstNextSelectedColumnIndex != null)
        {
          if (lastPreviousSelectedColumnIndex != null && (firstNextSelectedColumnIndex == null ||
                                                          firstNextSelectedColumnIndex - selectedColumnIndex >=
                                                          selectedColumnIndex - lastPreviousSelectedColumnIndex))
          {
            // Select all columns from last previous selected to current
            for (int i = lastPreviousSelectedColumnIndex.Value + 1; i < selectedColumnIndex; i++)
            {
              var col = grid.Columns[i];
              if (col != column) SfDataGridColumnBehavior.SetIsSelected(col, isSelected);
            }
          }
          if (firstNextSelectedColumnIndex != null && (lastPreviousSelectedColumnIndex == null ||
                                                       firstNextSelectedColumnIndex - selectedColumnIndex >=
                                                       selectedColumnIndex - lastPreviousSelectedColumnIndex))
          {
            // Select all columns from current to first next selected
            for (int i = selectedColumnIndex + 1; i < firstNextSelectedColumnIndex.Value; i++)
            {
              var col = grid.Columns[i];
              if (col != column) SfDataGridColumnBehavior.SetIsSelected(col, isSelected);
            }
          }
        }
      }
      SfDataGridColumnConverter.LogIt = true;

      //Debug.WriteLine($"GridColumnBehavior.IsSelected: {isSelected} for column: {column.MappingName}");
      SfDataGridColumnBehavior.SetIsSelected(column, isSelected);
      e.Handled = true;
    }
  }

  /// <summary>
  /// Handles the left mouse button down event on a grid row header indent cell to toggle the selection of the whole grid.
  /// </summary>
  /// <remarks>This method toggles the selection state of the grid's columns. If any cells are currently
  /// selected, it clears the selection. Otherwise, it selects or deselects all columns based on their current
  /// state.</remarks>
  /// <param name="sender">The source of the event, expected to be a <see cref="GridRowHeaderIndentCell"/>.</param>
  /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
  private void GridRowHeaderIndentCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    //Debug.WriteLine($"Grid_OnPreviewMouseLeftButtonDown({sender})");
    e.Handled = true;

    if (sender is GridRowHeaderIndentCell indentCell)
    {
      var grid = indentCell.FindParent<SfDataGrid>();
      if (grid == null) return;

      var isSelected = grid.GetSelectedCells().Any();
      if (isSelected)
      {
        grid.SelectionController.ClearSelections(false);
      }
      else
      {
        isSelected = grid.Columns.FirstOrDefault(SfDataGridColumnBehavior.GetIsSelected) is not null;
        isSelected = !isSelected;
        foreach (var column in grid.Columns)
        {
          SfDataGridColumnBehavior.SetIsSelected(column, isSelected);
        }
      }
    }
  }

}
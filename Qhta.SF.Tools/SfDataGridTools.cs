using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Qhta.WPF.Utils;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Controls.Cells;

namespace Qhta.SF.Tools;

public partial class SfDataGridTools : ResourceDictionary
{

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
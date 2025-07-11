using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Controls.Cells;

namespace Qhta.UnicodeBuild.Helpers;

public partial class SfDataGridTools : ResourceDictionary
{

  private void GridHeaderCellControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    //Debug.WriteLine($"Grid_OnPreviewMouseLeftButtonDown({sender})");
    e.Handled = true;

    if (sender is GridHeaderCellControl headerCellControl)
    {
      var column = headerCellControl.Column;
      if (column == null) return;
      var grid = headerCellControl.FindParent<SfDataGrid>();
      if (grid == null) return;

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
    }
  }

}
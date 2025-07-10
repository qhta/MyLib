using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Qhta.UnicodeBuild.ViewModels;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Controls.Cells;

namespace Qhta.UnicodeBuild.Helpers;

public partial class SfDataGridTools : ResourceDictionary
{

  private void Grid_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    Debug.WriteLine($"Grid_OnPreviewMouseLeftButtonDown({sender})");
    e.Handled = true;

    if (sender is GridHeaderCellControl headerCellControl)
    {
      var column = headerCellControl.Column;
      if (column == null) return;
      var grid = headerCellControl.FindParent<SfDataGrid>();
      if (grid == null) return;
      // Clear selection if no Shift or Ctrl key is pressed
      if (Keyboard.Modifiers == ModifierKeys.None)
        grid.SelectionController.ClearSelections(false);
      _ViewModels.Instance.SelectColumn(grid, column);
    }
  }

}
using System.Diagnostics;
using System.Reflection;
using System.Windows;

using Qhta.SF.Tools;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{

  public static bool CanCutData(SfDataGrid grid)
  {
    try
    {
      var selectedCells = grid.GetSelectedCells().ToArray();
      GridColumn[] selectedColumns;
      if (selectedCells.Length != 0)
        selectedColumns = selectedCells.Select(cell => cell.Column).Distinct().ToArray();
      else
        selectedColumns = grid.Columns.Where(SfDataGridColumnBehavior.GetIsSelected).ToArray();
      if (!selectedColumns.Any())
      {
        selectedColumns = grid.Columns.ToArray();
      }
      return !selectedColumns.Any(column => column.AllowEditing && column.IsReadOnly);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
    return false;
  }

  public static void CutData(SfDataGrid grid) => CutCopyData(grid, true);

}
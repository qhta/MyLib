using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{
  /// <summary>
  /// Determines whether data can be cut from the specified data grid.
  /// This method checks if all selected columns are editable and not read-only.
  /// </summary>
  /// <param name="grid"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Performs a cut operation on the data in the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="grid"></param>
  public static void CutData(SfDataGrid grid) => CutCopyData(grid, true);

}
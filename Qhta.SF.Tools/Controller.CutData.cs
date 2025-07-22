using System.Windows.Controls;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{
  /// <summary>
  /// Determines whether data can be cut from the specified data grid.
  /// This method checks if all selected columns are editable and not read-only.
  /// </summary>
  /// <param name="dataGrid"></param>
  /// <returns></returns>
  public static bool CanCutData(SfDataGrid dataGrid) => CanExecuteDataOp(dataGrid, DataOp.Cut);

  /// <summary>
  /// Performs a cut operation on the data in the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="dataGrid"></param>
  public static void CutData(SfDataGrid dataGrid) => ExecuteDataOp(dataGrid, DataOp.Cut);

}
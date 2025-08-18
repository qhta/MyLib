using System.Collections;
using System.Diagnostics;
using System.Windows;

using Qhta.UndoManager;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

public static partial class SfDataGridCommander
{

  /// <summary>
  /// Determines whether data can be deleted from the specified data grid.
  /// This method checks if all selected columns are editable and not read-only.
  /// </summary>
  /// <param name="dataGrid"></param>
  /// <returns></returns>
  public static bool CanDeleteData(SfDataGrid dataGrid)=>CanExecuteDataOp(dataGrid, DataOp.Delete);

  /// <summary>
  /// Performs a delete operation on the data in the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="dataGrid"></param>
  public static void DeleteData(SfDataGrid dataGrid) => ExecuteDataOp(dataGrid, DataOp.Delete);


}
using System.Diagnostics;
using System.Windows;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

public static partial class SfDataGridCommander
{
  /// <summary>
  /// Determines whether data can be copied from the specified data grid.
  /// </summary>
  /// <param name="dataGrid">The <see cref="SfDataGrid"/> instance to check for copy capability.</param>
  /// <returns><see langword="true"/> if data can be copied from the specified grid; otherwise, <see langword="false"/>.</returns>
  public static bool CanCopyData(SfDataGrid dataGrid) => CanExecuteDataOp(dataGrid, DataOp.Copy);

  /// <summary>
  /// Performs a copy operation on the data in the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="dataGrid"></param>
  public static void CopyData(SfDataGrid dataGrid) => ExecuteDataOp(dataGrid, DataOp.Copy);

}
using System.Diagnostics;
using System.Reflection;
using System.Windows;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{

  /// <summary>
  /// Determines whether data can be pasted into the specified data grid.
  /// </summary>
  /// <param name="grid"></param>
  /// <returns></returns>
  public static bool CanPasteData(SfDataGrid grid) => Clipboard.ContainsText();

  /// <summary>
  /// Performs a paste operation on the data in the specified <see cref="SfDataGrid"/>.
  /// </summary>
  /// <param name="grid"></param>
  public static void PasteData(SfDataGrid grid)
  {
    throw new NotImplementedException("PasteData is not implemented yet.");
  }

}
using System.Diagnostics;
using System.Windows;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{

  public static bool CanCopyData(SfDataGrid grid) => true;

  public static void CopyData(SfDataGrid grid) => CutCopyData(grid, false);

}
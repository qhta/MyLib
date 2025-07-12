using System.Diagnostics;
using System.Reflection;
using System.Windows;

using Qhta.SF.Tools;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

public static partial class Controller
{

  public static bool CanPasteData(SfDataGrid grid) => Clipboard.ContainsText();

  public static void PasteData(SfDataGrid grid)
  {
  }

}
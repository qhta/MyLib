using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyLib.WpfUtils
{
  public class DataGridCopyToClipboardCommand: Command
  {
    public string Name { get { return "Copy"; } }

    public override void Execute(object parameter)
    {
      DataGrid dg = parameter as DataGrid;
      dg.SelectAllCells();
      dg.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
      ApplicationCommands.Copy.Execute(null, dg);
      dg.UnselectAllCells();

    }

  }
}

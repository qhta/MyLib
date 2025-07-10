using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Qhta.MVVM;
using Qhta.TypeUtils;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Views;
using Qhta.WPF.Utils;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  public void SelectColumn(SfDataGrid grid, GridColumn column)
  {
    var worker = new BackgroundWorker();
    grid.Cursor = Cursors.Wait;
    worker.DoWork += SelectColumnWorker_DoWork;
    worker.RunWorkerCompleted += (s, e) =>
    {
      grid.Cursor = Cursors.Arrow;
    };
    worker.RunWorkerAsync((grid, column));
  }

  BackgroundWorker SelectColumnWorker = new BackgroundWorker
  {
    WorkerReportsProgress = true,
    WorkerSupportsCancellation = false
  };

  private void SelectColumnWorker_DoWork(object? sender, DoWorkEventArgs e)
  {
    (SfDataGrid grid, GridColumn column) = ((SfDataGrid, GridColumn))e.Argument!;
    var startRowData = grid.View.Records.FirstOrDefault()?.Data;
    var endRowData = grid.View.Records.LastOrDefault()?.Data;
    if (startRowData == null || endRowData == null) return;

    for (int i = 0; i < grid.View.Records.Count; i++)
    {
      var rowData = grid.View.Records[i].Data;
      if (rowData == null) continue;
      //Debug.WriteLine($"Selecting cell: {rowData}, column: {column.MappingName}");
      grid.SelectCell(rowData, column);
      if (i % 1000 == 0)
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
        }, DispatcherPriority.Background);
      }

    }

  }

  private void LoadData_ProgressChanged(object? sender, ProgressChangedEventArgs e)
  {
    //Debug.WriteLine($"LoadData_ProgressChanged({e.ProgressPercentage})");
    UcdCodePoints.ProgressValue = e.ProgressPercentage;
    if (codePointsArray == null || codePointsArray.Length == 0 || e.ProgressPercentage == 0)
    {
      return;
    }
    var start = (e.ProgressPercentage - 1) * codePointsArray.Count() / 100;
    var end = (e.ProgressPercentage == 100) ? codePointsArray.Count() : (e.ProgressPercentage) * codePointsArray.Count() / 100;
    for (int i = start; i < end; i++)
    {
      var cp = codePointsArray[i];
      UcdCodePoints.Add(cp);
    }
  }
}   
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

using Qhta.MVVM;
using Qhta.Unicode.Models;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  public BackgroundWorker LoadDataBackgroundWorker = new BackgroundWorker
  {
    
    WorkerReportsProgress = true,
    WorkerSupportsCancellation = true
  };

  private void LoadData_DoWork(object? sender, DoWorkEventArgs e)
  {
    var worker = sender as BackgroundWorker;
    if (worker == null) return;
    UcdCodePoints.IsBusy = true;
    LoadData(worker, e);
  }

  private UcdCodePoint[]? codePointsArray = null;

  private void LoadData_ProgressChanged(object? sender, ProgressChangedEventArgs e)
  {
    //Debug.WriteLine($"LoadData_ProgressChanged({e.ProgressPercentage})");
    UcdCodePoints.ProgressValue = e.ProgressPercentage;
    if (codePointsArray == null || codePointsArray.Length == 0 || e.ProgressPercentage==0)
    {
      return;
    }
    var start = (e.ProgressPercentage - 1) * codePointsArray.Count() / 100;
    var end = (e.ProgressPercentage==100) ? codePointsArray.Count() : (e.ProgressPercentage) * codePointsArray.Count() / 100;
    for (int i = start; i < end; i++)
    {
      var cp = codePointsArray[i];
      UcdCodePoints.Add(cp);
    }

  }

  private void LoadData(BackgroundWorker worker, DoWorkEventArgs e)
  {
    codePointsArray = DBContext.CodePoints.ToArray();

    var n = codePointsArray.Length;
    UcdCodePoints.StatusMessage = String.Format(Resources.Strings.Loading, nameof(UcdCodePoints));
    for (int i = 0; i <= 100; i++)
    {
      worker.ReportProgress(i);
      //if (i == 100) break;
      //Debug.WriteLine($"Processing line {i}");
      if (worker.CancellationPending)
      {
        e.Cancel = true; // Indicate that the operation was canceled
        return;
      }
      //Thread.Sleep(10);
      // Allow the GUI thread to process messages
      Application.Current.Dispatcher.Invoke(() => { }, DispatcherPriority.Background);
    }
    UcdCodePoints.IsLoaded = true;
  }

  private void LoadData_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
  {
    //UcdCodePoints.StatusMessage = null;
    if (e.Cancelled)
    {
      MessageBox.Show(Resources.Strings.OperationCancelled, Resources.Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    else if (e.Error != null)
    {
      MessageBox.Show(String.Format(Resources.Strings.ErrorOcurred, e.Error.Message), Resources.Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
    }
    //else
    //{
    //  MessageBox.Show(Resources.Strings.OperationCompleted, Resources.Strings.Info, MessageBoxButton.OK, MessageBoxImage.Information);
    //}
    UcdCodePoints.IsBusy = false;
    UcdCodePoints.ProgressValue = 0;
    UcdCodePoints.StatusMessage = null;
  }

}
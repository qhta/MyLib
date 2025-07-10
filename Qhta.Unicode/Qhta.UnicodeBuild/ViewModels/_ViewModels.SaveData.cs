using System.Diagnostics;
using System.Windows.Threading;

using Qhta.MVVM;

namespace Qhta.UnicodeBuild.ViewModels;

public partial class _ViewModels
{
  public RelayCommand SaveDataCommand { get; }

  public bool SaveCommandCanExecute()
  {
    return DbContext.ChangeTracker.HasChanges();
  }

  public void SaveCommandExecute()
  {
    DbContext.SaveChanges();
    Debug.WriteLine("Data changes saved");
  }

  public void RefreshSaveDataCanExecute()
  {
    // Trigger CanExecute reevaluation
    SaveDataCommand.NotifyCanExecuteChanged();
  }

  private DispatcherTimer? _saveDataTimer;

  public void InitSaveDataTimer()
  {
    // Initialize the timer
    _saveDataTimer = new DispatcherTimer
    {
      Interval = TimeSpan.FromSeconds(1)
    };
    _saveDataTimer.Tick += (s, e) => RefreshSaveDataCanExecute();
    _saveDataTimer.Start();
  }
}
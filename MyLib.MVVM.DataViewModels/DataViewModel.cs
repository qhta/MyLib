using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public abstract class DataViewModel: ViewModel
  {
    public ViewModel ParentViewModel { get; set; }

    public bool IsOpened
    {
      get => _IsOpened;
      set { if (_IsOpened!=value) { _IsOpened = value; NotifyPropertyChanged(nameof(IsOpened)); } }
    }
    private bool _IsOpened;
    private bool _IsOpening;

    public virtual void OpenDataSet()
    {
      if (IsOpened)
        return;
      if (_IsOpening)
        return;
      _IsOpening=true;
      DoOpenDataSet();
      IsOpened = true;
    }

    public virtual Task OpenDataSetAsync()
    {
      if (IsOpened)
        return Task.CompletedTask;
      if (_IsOpening)
        return Task.CompletedTask;
      _IsOpening=true;
      return DoOpenDataSetAsync().ContinueWith(task=>{ IsOpened = true; });      
    }

    protected Task DoOpenDataSetAsync()
    {
      return Task.Run(() => new Action(DoOpenDataSet).Invoke());
    }

    protected abstract void DoOpenDataSet();
  }
}

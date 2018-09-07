using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MyLib.MVVM
{
  public class VisibleViewModel<ItemType> : VisibleViewModel, IVisible, IExpandable, ISelectable
  {
    public VisibleViewModel()
    {
    }

    public VisibleViewModel(IViewModel parentViewModel)
    {
      ParentViewModel = parentViewModel;
    }

    public IViewModel ParentViewModel { get; private set; }

    public ItemType Model
    {
      get => _Model;
      set
      {
        if (!value.Equals(_Model))
        {
          _Model = value;
          if (_Model is INotifyPropertyChanged observableModel)
            observableModel.PropertyChanged+=ObservableModel_PropertyChanged;
        }
      }
    }

    private void ObservableModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      NotifyPropertyChanged(e.PropertyName);
    }

    private ItemType _Model;

    public bool IsWaiting
    {
      get => _WaitCount>0;
    }
    int _WaitCount;

    public virtual void StartWaiting()
    {
      _WaitCount++;
      NotifyPropertyChanged(nameof(IsWaiting));
    }
    public virtual void StopWaiting()
    {
      if (_WaitCount>0)
      {
        _WaitCount--;
        NotifyPropertyChanged(nameof(IsWaiting));
      }
    }
  }
}

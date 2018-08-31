using System;
using System.Collections.Generic;
using System.Text;

namespace MyLib.MVVM
{
  public abstract class LazyLoadViewModel<ItemType>: VisibleViewModel<ItemType>, ILazyLoad
  {
    public bool LazyLoad
    {
      get => _LazyLoad;
      set
      {
        if (_LazyLoad!=value)
        {
          _LazyLoad=true;
          NotifyPropertyChanged(nameof(LazyLoad));
          if (value)
            StartLoading();
        }
      }
    }
    bool _LazyLoad;

    public bool IsLoading
    {
      get => _IsLoading;
      protected set
      {
        if (_IsLoading!=value)
        {
          _IsLoading=value;
          NotifyPropertyChanged(nameof(IsLoading));
        }
      }
    }
    bool _IsLoading;

    public bool IsLoaded
    {
      get => _IsLoaded;
      protected set
      {
        if (_IsLoaded!=value)
        {
          _IsLoaded=value;
          NotifyPropertyChanged(nameof(IsLoaded));
        }
      }
    }
    bool _IsLoaded;

    public abstract void StartLoading();

  }
}

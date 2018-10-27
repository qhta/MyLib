using System.Threading.Tasks;

namespace Qhta.MVVM
{
  public abstract class LazyLoadViewModel<ItemType>: VisibleViewModel<ItemType>, ILazyLoad, IExpandable
  {

    public bool LoadOnInit
    {
      get => _LoadOnInit;
      set
      {
        if (_LoadOnInit!=value)
        {
          _LoadOnInit=value;
          NotifyPropertyChanged(nameof(LoadOnInit));
          if (value)
            StartLoading();
        }
      }
    }
    bool _LoadOnInit;

    public bool LoadOnExpand
    {
      get => _LoadOnExpand;
      set
      {
        if (_LoadOnExpand!=value)
        {
          _LoadOnExpand=true;
          NotifyPropertyChanged(nameof(LoadOnExpand));
          if (value)
            PrepareLoadOnExpand();
        }
      }
    }
    bool _LoadOnExpand;

    public bool IsLoading
    {
      get => _IsLoading;
      protected set
      {
        if (_IsLoading!=value)
        {
          _IsLoading=value;
          NotifyPropertyChanged(nameof(IsLoading));
          if (value)
            StartWaiting();
          else
            StopWaiting();
        }
      }
    }
    bool _IsLoading;

    public bool IsLoaded
    {
      get => _IsLoaded;
      set
      {
        if (_IsLoaded!=value)
        {
          _IsLoaded=value;
          NotifyPropertyChanged(nameof(IsLoaded));
        }
      }
    }
    bool _IsLoaded;

    public override bool IsExpanded
    {
      get => base.IsExpanded;
      set
      {
        if (value==true && base.IsExpanded==false && 
          LoadOnExpand && !_IsLoaded && !IsLoading)
        {
          StartLoading().ContinueWith((task) => { base.IsExpanded=true; });
        }
        else
          base.IsExpanded = value;
      }
    }

    protected abstract void PrepareLoadOnExpand();

    public abstract Task StartLoading();

  }
}

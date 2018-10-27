using System.Collections.Generic;

namespace Qhta.MVVM
{
  public abstract class ListViewModel : VisibleViewModel, IOrientable, IVisible
  {
    public ListViewModel() { }

    public ListViewModel(IViewModel parentViewModel)
    {
      ParentViewModel = parentViewModel;
    }

    public abstract IEnumerable<object> GetItems();
    public IViewModel ParentViewModel { get; protected set; }

    public OrientationType Orientation
    {
      get { return _Orientation; }
      set
      {
        if (_Orientation!=value)
        {
          _Orientation=value;
          NotifyPropertyChanged(nameof(Orientation));
        }
      }
    }
    private OrientationType _Orientation;

    //public string SortedBy
    //{
    //  get { return _SortedBy; }
    //  set
    //  {
    //    if (_SortedBy!=value)
    //    {
    //      _SortedBy=value;
    //      NotifyPropertyChanged(nameof(SortedBy));
    //    }
    //  }
    //}
    //private string _SortedBy;

    //public virtual void FindFirstItem(object pattern, IEnumerable<string> propNames) { }

    //public virtual void FindNextItem() { }

    //public virtual void FindFirstInvalidItem() { }

    //public virtual void FindNextInvalidItem() { }

    //public abstract IEnumerable<object> Items { get; }
  }
}

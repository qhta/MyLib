using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public abstract class ListViewModel : VisibleViewModel, IOrientable, IVisible
  {
    public ListViewModel(ViewModel parentViewModel)
    {
      ParentViewModel = parentViewModel;
    }

    public ViewModel ParentViewModel { get; private set; }

    public abstract Type GetItemType();

    public OrientationType Orientation
    {
      get { return _Orientation; }
      set
      {
        if (_Orientation!=value)
        {
          _Orientation=value;
          NotifyPropertyChanged("Orientation");
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
    //      NotifyPropertyChanged("SortedBy");
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

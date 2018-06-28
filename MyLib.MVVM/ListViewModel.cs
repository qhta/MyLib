using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public abstract class ListViewModel : VisibleViewModel, IOrientable, IVisible
  {

    public ViewModel ParentViewModel { get; protected set; }

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

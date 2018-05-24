using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public abstract class ListViewModel : VisibleViewModel, IOrientable, IVisible
  {

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

    public virtual void FindFirstInvalidItem() { }

    public virtual void FindNextInvalidItem() { }
  }
}

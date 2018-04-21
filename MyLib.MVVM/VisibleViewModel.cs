using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.MVVM
{
  public class VisibleViewModel : ViewModel, IVisible
  {
    public bool IsVisible
    {
      get { return _IsVisible; }
      set
      {
        if (_IsVisible!=value)
        {
          _IsVisible = value;
          NotifyPropertyChanged("IsVisible");
        }
      }
    }
    private bool _IsVisible= true;

  }
}

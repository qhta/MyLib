using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MyLib.MultiThreadingObjects
{
  public class DispatchedObject: INotifyPropertyChanged
  {
    public static Dispatcher ApplicationDispatcher { get; set; }

    public virtual string DebugName { get; set; }

    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
        _PropertyChanged+=value;
      }
      remove
      {
        _PropertyChanged-=value;
      }
    }
    event PropertyChangedEventHandler _PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      if (_PropertyChanged!=null)
      {
        if (DispatchedObject.ApplicationDispatcher==null || Dispatcher.CurrentDispatcher==ApplicationDispatcher)
        {
          _PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        else
        {
          var action = new Action<string>(NotifyPropertyChanged);
          ApplicationDispatcher.Invoke(action, new object[] { propertyName });
        }
      }
    }

    public void Dispatch(Action action)
    {
      if (DispatchedObject.ApplicationDispatcher==null || Dispatcher.CurrentDispatcher==ApplicationDispatcher)
      {
        action.Invoke();
      }
      else
      {
        ApplicationDispatcher.Invoke(action, new object[] { });
      }
    }
  }
}

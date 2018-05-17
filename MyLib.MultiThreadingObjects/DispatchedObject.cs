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

    public virtual string Name { get; set; }

    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
        _initialDispatcher = Dispatcher.CurrentDispatcher;
        _PropertyChanged+=value;
      }
      remove
      {
        _PropertyChanged-=value;
      }
    }
    event PropertyChangedEventHandler _PropertyChanged;
    Dispatcher _initialDispatcher;

    public void NotifyPropertyChanged(string propertyName)
    {
      if (_PropertyChanged!=null)
      {
        if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
        {
          _PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        else if (_initialDispatcher==ApplicationDispatcher)
        {
          var action = new Action<string>(NotifyPropertyChanged);
          ApplicationDispatcher.Invoke(action, new object[] { propertyName });
        }
        else
        {
          _PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
      }
    }

    public void Dispatch(Action action)
    {
      if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
      {
        action.Invoke();
      }
      else if (_initialDispatcher==ApplicationDispatcher)
      {
        ApplicationDispatcher.Invoke(action, new object[] { });
      }
      else
      {
        action.Invoke();
      }
    }
  }
}

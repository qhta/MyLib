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
        _callerDispatcher = Dispatcher.CurrentDispatcher;
        _PropertyChanged+=value;
      }
      remove
      {
        _PropertyChanged-=value;
      }
    }
    event PropertyChangedEventHandler _PropertyChanged;
    Dispatcher _callerDispatcher;

    protected void NotifyPropertyChanged(string propertyName)
    {
      if (_PropertyChanged!=null)
      {
        //if (propertyName=="Target")
        //  Debug.WriteLine($"{Name} {propertyName} changed");
        if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
        {
          //if (propertyName.StartsWith("Target") ||propertyName.StartsWith("Source"))
          //  Debug.WriteLine($"{Name} {propertyName} invoke");
          _PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        else if (_callerDispatcher!=ApplicationDispatcher)
        {
          //if (propertyName.StartsWith("Target") ||propertyName.StartsWith("Source"))
          //  Debug.WriteLine($"{Name} {propertyName} invoke {_PropertyChanged!=null}");
          _PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        else if (ApplicationDispatcher!=null)
        {
          if (propertyName.StartsWith("Target") ||propertyName.StartsWith("Source"))
            Debug.WriteLine($"{Name} {propertyName} dispatch");
          var action = new Action<string>(NotifyPropertyChanged);
          ApplicationDispatcher.Invoke(action, new object[] { propertyName });
        }
      }
    }
  }
}

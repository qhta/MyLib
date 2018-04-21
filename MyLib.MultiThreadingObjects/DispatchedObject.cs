using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        _PropertyChanged+=value;
      }
      remove
      {
        _PropertyChanged-=value;
      }
    }
    event PropertyChangedEventHandler _PropertyChanged;

    protected void NotifyPropertyChanged(string propertyName)
    {
      if (_PropertyChanged!=null)
      {
        if (propertyName=="IsPopulated")
          Debug.WriteLine($"{Name} Populated");
        if (Dispatcher.CurrentDispatcher==ApplicationDispatcher)
        {
          _PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        else if (ApplicationDispatcher!=null)
        {
          var action = new Action<string>(NotifyPropertyChanged);
          ApplicationDispatcher.BeginInvoke(action, new object[] { propertyName });
        }
      }
    }
  }
}

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

namespace MyLib.WpfUtils
{
  public class DispatchedObject: INotifyPropertyChanged
  {
    public Dispatcher _Dispatcher = Application.Current?.Dispatcher;

    public virtual string Name { get; set; }

    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
        if (Name=="text")
          Debug.WriteLine($"{this.GetType().Name} {Name} PropertyChange set");
        _PropertyChanged+=value;
      }
      remove { _PropertyChanged-=value; }
    }
    event PropertyChangedEventHandler _PropertyChanged;

    protected void NotifyPropertyChanged(string propertyName)
    {
      //if (Name=="entity")
      //Debug.WriteLine($"{this.GetType().Name} {Name} NotifyPropertyChanged");
      if (_PropertyChanged!=null)
      {
        //if (Name=="entity")
        //  Debug.WriteLine($"{this.GetType().Name} {Name} PropertyChanged handle");
        if (Dispatcher.CurrentDispatcher==_Dispatcher)
        {
          //if (Name=="entity")
          //  Debug.WriteLine($"{this.GetType().Name} {Name} PropertyChanged invoke");
          _PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        else if (_Dispatcher!=null)
        {
          var action = new Action<string>(NotifyPropertyChanged);
          _Dispatcher.BeginInvoke(action, new object[] { propertyName });
          //if (Name=="entity")
          //  Debug.WriteLine($"{this.GetType().Name} {Name} PropertyChanged dispatched");
        }
      }
    }
  }
}

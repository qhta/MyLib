using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Qhta.DispatchedObjects
{
  /// <summary>
  /// A class that invokes <see cref="Dispatcher"/> on <see cref="PropertyChanged"/>> event or on other action.
  /// It defines a static <see cref="ApplicationDispatcher"/> property which enables a developer to setup a <see cref="Dispatcher"/> from any application
  /// (e.g. it can be Application.Current.Dispatcher in WPF applications).
  /// </summary>
  public class DispatchedObject: INotifyPropertyChanged
  {
    /// <summary>
    /// A static property which enables a developer to setup a Dispatcher from any application
    /// (e.g. it can be Application.Dispatcher in WPF applications).
    /// </summary>
    public static Dispatcher ApplicationDispatcher { get; set; }

    /// <summary>
    /// Helper name which can be used on Debugging.
    /// </summary>
    public virtual string DebugName { get; set; }

    /// <summary>
    /// Property changed event which implements <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
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
    /// <summary>
    /// A handler of <see cref="PropertyChanged"/> event which can be checked in descending classes.
    /// </summary>
    protected event PropertyChangedEventHandler _PropertyChanged;

    /// <summary>
    /// A method to notify that a property has changed. 
    /// When <see cref="ApplicationDispatcher"/> was not set
    /// or it is a CurrentDispatcher (a method is called from within the main thread), a <see cref="PropertyChanged"/> event
    /// is invoked directly, otherwise it is invoked using <see cref="ApplicationDispatcher"/>.
    /// </summary>
    /// <param name="propertyName"></param>
    public virtual void NotifyPropertyChanged(string propertyName)
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

    /// <summary>
    /// A method to invoke and action.
    /// When <see cref="ApplicationDispatcher"/> was not set
    /// or it is a CurrentDispatcher (a method is called from within the main thread), the action
    /// is invoked directly, otherwise it is invoked using <see cref="ApplicationDispatcher"/>.
    /// </summary>
    /// <param name="action"></param>
    public virtual void Dispatch(Action action)
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

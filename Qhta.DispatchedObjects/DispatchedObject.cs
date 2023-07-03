using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Qhta.DispatchedObjects
{
  /// <summary>
  /// A class that invokes <see cref="Dispatcher"/> on <see cref="PropertyChanged"/>> event or on other action.
  /// </summary>
  public class DispatchedObject: INotifyPropertyChanged
  {
    /// <summary>
    /// A static object to dispatch actions to main app thread.
    /// </summary>
    public static IDispatcherBridge? DispatcherBridge { get; set; }

    /// <summary>
    /// Property changed event which implements <see cref="INotifyPropertyChanged"/> interface.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged
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
    protected event PropertyChangedEventHandler? _PropertyChanged;

    /// <summary>
    /// A method to notify that a property has changed. 
    /// Uses <see cref="DispatcherBridge"/>, if it is set, to invoke <see cref="PropertyChanged"/> event.
    /// Otherwise the event is invoked directly.
    /// </summary>
    /// <param name="propertyName"></param>
    public virtual void NotifyPropertyChanged(string propertyName)
    {
      if (_PropertyChanged!=null)
      {
        var dispatcher = DispatcherBridge; 
        if (dispatcher != null)
          dispatcher.Invoke(()=>_PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
        else
          _PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// A method to invoke and action.
    /// Uses <see cref="DispatcherBridge"/>, if it is set, to invoke an action .
    /// Otherwise the action is invoked directly.
    /// </summary>
    /// <param name="action"></param>
    public virtual void Dispatch(Action action)
    {
      var dispatcher = DispatcherBridge;
      if (dispatcher != null) 
        dispatcher.Invoke(action);
      else
        action.Invoke();
    }
  }
}

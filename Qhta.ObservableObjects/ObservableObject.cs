using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Qhta.ObservableObjects
{
  /// <summary>
  /// Class that notifies other objects when its properties are changed.
  /// Implements interface <see cref="INotifyPropertyChanged"/>.
  /// </summary>
  public class ObservableObject : INotifyPropertyChanged, ILazyLoad
  {
    /// <summary>
    /// Common static dispatcher for notifying actions.
    /// </summary>
    public static IDispatcherBridge? CommonDispatcher { get; set; }

    /// <summary>
    /// Individual dispatcher for notifying actions. 
    /// If not set, then <see cref="CommonDispatcher"/> is used.
    /// </summary>
    public IDispatcherBridge? Dispatcher
    {
      get => _Dispatcher ?? CommonDispatcher;
      set => _Dispatcher = value;
    }
    private IDispatcherBridge? _Dispatcher;

    /// <summary>
    /// Common property that specifies 
    /// whether the notification action will be invoked asynchronously (with BeginInvoke)
    /// or synchronously (with Invoke).
    /// </summary>
    public static bool CommonBeginInvokeActionEnabled { get; set; }

    /// <summary>
    /// Specifies whether the notification action will be invoked asynchronously (with BeginInvoke)
    /// or synchronously (with Invoke). 
    /// If not set directly for this instance, then <see cref="CommonBeginInvokeActionEnabled"/> is used.
    /// </summary>
    public bool BeginInvokeActionEnabled
    {
      get => _BeginInvokeActionEnabled ?? CommonBeginInvokeActionEnabled;
      set => _BeginInvokeActionEnabled = value;
    }

    private bool? _BeginInvokeActionEnabled;

    /// <summary>
    /// An object used to synchronize access.
    /// </summary>
    public virtual object LockObject => new object();

    #region INotifyPropertyChanged

    /// <summary>
    /// An event to raise when a property is changed. Can be set by any observer.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// A method to raise the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the changed property</param>
    public void NotifyPropertyChanged(string propertyName) => NotifyPropertyChanged(this, propertyName);

    /// <summary>
    /// A method to raise the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the changed property</param>
    /// <param name="oldValue">Old value of the property</param>
    /// <param name="newValue">New value of the property</param>
    public void NotifyPropertyChanged(string propertyName, object? oldValue, object? newValue) => NotifyPropertyChanged(this, propertyName, oldValue, newValue);

    /// <summary>
    /// A method to raise the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the changed property</param>
    /// <param name="sender">The object, which property was changed.</param>
    public void NotifyPropertyChanged(object sender, string propertyName) => NotifyPropertyChanged(sender, propertyName, null, null);

    /// <summary>
    /// A method to raise the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="sender">The object, which property was changed.</param>
    /// <param name="propertyName">Name of the changed property</param>
    /// <param name="oldValue">Old value of the property</param>
    /// <param name="newValue">New value of the property</param>
    public void NotifyPropertyChanged(object sender, string propertyName, object? oldValue, object? newValue)
    {
      var propertyChangedEventHandler = PropertyChanged;

      if (propertyChangedEventHandler == null)
        return;

      var dispatcher = Dispatcher;

      foreach (PropertyChangedEventHandler handler in propertyChangedEventHandler.GetInvocationList().Cast<PropertyChangedEventHandler>())
      {

        PropertyChangedEventArgs args = (oldValue == null && newValue == null) 
          ? new PropertyChangedEventArgs(propertyName)
          : new PropertyChanged2EventArgs(propertyName, oldValue, newValue);
        if (dispatcher != null)
        {
          try
          {
            if (BeginInvokeActionEnabled)
              dispatcher.BeginInvoke(handler, sender, args);
            else
              dispatcher.Invoke(handler, sender, args);
          }
          catch (Exception ex)
          {
            Debug.WriteLine($"{ex.GetType()} thrown in {this.GetType()} NotifyPropertyChanged dispatched invoke:\n {ex.Message}");
          }
        }
        else
          try
          {
            if (BeginInvokeActionEnabled)
              handler.BeginInvoke(sender, args, null, null);
            else
              handler.Invoke(sender, args);
          }
          catch (Exception ex)
          {
            Debug.WriteLine($"{ex.GetType()} thrown in NotifyPropertyChanged direct invoke:\n {ex.Message}");
          }
      }
    }

    #endregion INotifyPropertyChanged

    #region Enable synchronization from BindingOperations
    /// <summary>
    /// Specifies whether the object is synchronized in multi-thread environment.
    /// </summary>
    public virtual bool IsSynchronized => true;

    /// <summary>
    /// Object used to synchronize in multi-thread environment.
    /// </summary>
    public virtual object SyncRoot => LockObject;

    #endregion

    /// <summary>
    /// Determines whether the object is loaded.
    /// </summary>
    public virtual bool IsLoaded
    {
      get => _IsLoaded;
      set
      {
        if (value != _IsLoaded)
        {
          _IsLoaded = value;
          NotifyPropertyChanged(nameof(IsLoaded));
        }
      }
    }

    private bool _IsLoaded;
  }
}

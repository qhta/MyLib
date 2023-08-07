using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  /// <summary>
  /// Class that notifies other objects when its properties are changed.
  /// Implements interface <see cref="INotifyPropertyChanged"/>.
  /// </summary>
  public class ObservableObject : INotifyPropertyChanged
  {
    /// <summary>
    /// Common static dispatcher for notifying actions.
    /// </summary>
    public static Dispatcher CommonDispatcher { get; set; } = null!;

    /// <summary>
    /// Individual dispatcher for notifying actions. 
    /// If not set, then <see cref="CommonDispatcher"/> is used.
    /// </summary>
    public Dispatcher Dispatcher
    {
      get => _Dispatcher ?? CommonDispatcher;
      set
      {
#if DEBUG
        {
          if (value.Thread.GetApartmentState() != ApartmentState.STA)
            Debug.Fail($"{this} should be set with STA thread dispatcher");
          if (value.Thread.Name != "VSTA_Main")
            Debug.Fail($"{this.GetType().Name} should be created with VSTA_Main dispatcher");
        }
#endif
        _Dispatcher = value;
      }
    }
    private Dispatcher? _Dispatcher;

    /// <summary>
    /// Common property that specifies 
    /// whether the notification action will be invoked asynchronously (with BeginInvoke)
    /// or synchronously (with Invoke).
    /// </summary>
    public static bool CommonBeginInvokeActionEnabled { get; set; }

    /// <summary>
    /// Specifies whether the notification action will be invoked asynchronously (with BeginInvoke)
    /// or synchronously (with Invoke). 
    /// If not set directly for this instance, than <see cref="CommonBeginInvokeActionEnabled"/> is used.
    /// </summary>
    public bool BeginInvokeActionEnabled
    {
      get => _BeginInvokeActionEnabled ?? CommonBeginInvokeActionEnabled;
      set => _BeginInvokeActionEnabled = value;
    }

    private bool? _BeginInvokeActionEnabled;

    /// <summary>
    /// An object used to sychronize access.
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
    /// <param name="sender">The object, which property was changed.</param>
    public void NotifyPropertyChanged(object sender, string propertyName)
    {
      var propertyChangedEventHandler = PropertyChanged;

      if (propertyChangedEventHandler == null)
        return;

      var dispatcher = Dispatcher;

      foreach (PropertyChangedEventHandler handler in propertyChangedEventHandler.GetInvocationList())
      {

        var args = new PropertyChangedEventArgs(propertyName);
        if (dispatcher != null)
        {
          try
          {
            if (BeginInvokeActionEnabled)
              dispatcher.BeginInvoke(DispatcherPriority.Background, handler, sender, args, null, null);
            else
              dispatcher.Invoke(DispatcherPriority.Background, handler, sender, args);
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
    /// Specifies whether the object is synchronized in multithread environment.
    /// </summary>
    public virtual bool IsSynchronized
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Object used to synchronize in multithread environment.
    /// </summary>
    public virtual object SyncRoot
    {
      get
      {
        return LockObject;
      }
    }
    #endregion

  }
}

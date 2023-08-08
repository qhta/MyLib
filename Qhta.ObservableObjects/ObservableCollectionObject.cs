using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  /// <summary>
  /// Base class for all observable collection classes.
  /// </summary>
  public abstract class ObservableCollectionObject : ObservableObject, INotifyCollectionChanged
  {
    #region INotifyCollectionChanged

    /// <summary>
    /// Handler for collection changed event.
    /// </summary>
    public virtual event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Method to handle collection changed event.
    /// If a Dispatcher is set, then it is used to begin invoke the action
    /// </summary>
    /// <param name="sender">Immutable collection which is a result of the operation</param>
    /// <param name="notifyCollectionChangedEventHandler"></param>
    /// <param name="args"></param>
    protected virtual void HandleCollectionChangedEvent(object sender, NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler,
      NotifyCollectionChangedEventArgs args)
    {
      var dispatcher = Dispatcher;
      foreach (NotifyCollectionChangedEventHandler handler in notifyCollectionChangedEventHandler.GetInvocationList())
      {
        if (dispatcher != null)
        {
          try
          {
            if (BeginInvokeActionEnabled)
              dispatcher.BeginInvoke(DispatcherPriority.Background, handler, sender, args, null, null);
            else
              dispatcher.Invoke(DispatcherPriority.Send, handler, sender, args);
          }
          catch (Exception ex)
          {
            Debug.WriteLine($"{ex.GetType()} thrown in {this.GetType()} NotifyCollectionChanged dispatched invoke:\n {ex.Message}");
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
            Debug.WriteLine($"{ex.GetType()} thrown in NotifyCollectionChanged direct invoke:\n {ex.Message}");
          }
      }
    }


    /// <summary>
    /// Invokes <see cref="HandleCollectionChangedEvent"/> handle method.
    /// </summary>
    /// <param name="sender">Immutable collection which is a result of the operation</param>
    /// <param name="args"></param>
    public virtual void NotifyCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
      if (CollectionChanged!=null)
        HandleCollectionChangedEvent(sender, CollectionChanged, args);
    }
    #endregion INotifyCollectionChanged

  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  /// <summary>
  /// Base class for all observable collection classes
  /// </summary>
  public abstract class ObservableCollectionObject : ObservableObject, INotifyCollectionChanged
  {
    //internal const string dateTimeFormat = "hh:mm:ss.fff";

    #region INotifyCollectionChanged

    /// <summary>
    /// Abstract get accessor for object which will be notify other objects on collection change.
    /// Will be implemented to get immutable collection of items.
    /// </summary>
    /// <returns></returns>
    protected abstract ICollection GetNotifyObject();

    /// <summary>
    /// Handler for collection changed event.
    /// </summary>
    public virtual event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Specifies whether collection notifies about changes.
    /// </summary>
    public bool NotifyCollectionChangedEnabled { get; set; } = true;

    /// <summary>
    /// Starts bulk change operation.
    /// </summary>
    /// <param name="action">Specifies action to invoke on end of bulk change operation.</param>
    public void BulkChangeStart(NotifyCollectionChangedAction action)
    {
      if (bulkAction != null)
        BulkChangeEnd();
      bulkAction = action;
      newItems = new List<object>();
      itemsIndex = -1;
      oldItems = new List<object>();
    }

    /// <summary>
    /// Ends bulk change operation. Invokes action set with previous <see cref="BulkChangeStart"/> method.
    /// </summary>
    public void BulkChangeEnd()
    {
      if (bulkAction != null)
      {
        var action = (NotifyCollectionChangedAction)bulkAction;
        bulkAction = null;
        if (newItems.Count > 0)
          NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItems, oldItems, itemsIndex));
        newItems = null!;
      }
    }

    /// <summary>
    /// Action to invoke on <see cref="BulkChangeEnd()"/>
    /// </summary>
    protected NotifyCollectionChangedAction? bulkAction;

    /// <summary>
    /// List of items added in bulk operation.
    /// Set up in <see cref="BulkChangeStart"/> method.
    /// </summary>
    protected List<object> newItems = null!;

    /// <summary>
    /// List of items removed in bulk operation.
    /// Set up in <see cref="BulkChangeStart"/> method.
    /// </summary>
    protected List<object> oldItems = null!;

    /// <summary>
    /// Index of items changed in bulk operation.
    /// </summary>
    protected int itemsIndex;

    /// <summary>
    /// Method invoked on each collection change.
    /// If a bulk change operation was started, 
    /// then it collects items in <see cref="newItems"/> items.
    /// Otherwise it invokes <see cref="CollectionChanged"/> events
    /// (if they are set.
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      if (args.Action == bulkAction)
      {
        if (itemsIndex == -1)
          itemsIndex = args.NewStartingIndex;
        foreach (var item in args.NewItems)
          newItems.Add(item);
        return;
      }

      if (!NotifyCollectionChangedEnabled)
        return;

      if (CollectionChanged != null)
        HandleCollectionChangedEvent(CollectionChanged, args, false);
    }

    /// <summary>
    /// Method to handle collection changed event.
    /// If a Dispatcher is set, then it is used to begin invoke the action
    /// </summary>
    /// <param name="notifyCollectionChangedEventHandler"></param>
    /// <param name="args"></param>
    /// <param name="immediately"></param>
    protected virtual void HandleCollectionChangedEvent(NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler,
      NotifyCollectionChangedEventArgs args, bool immediately)
    {
      if (notifyCollectionChangedEventHandler == null)
        return;
      //var newItemsCount = args.NewItems?.Count ?? 0;
      //var oldItemsCount = args.OldItems?.Count ?? 0;
      //if (args.Action == NotifyCollectionChangedAction.Add)
      //  Debug.WriteLine($"NotifyCollectionChanged(action={args.Action}, newItems.Count={newItemsCount}, newStartingIndex={args.NewStartingIndex})" +
      //  $" {DateTime.Now.ToString(dateTimeFormat)}");
      //else if (args.Action == NotifyCollectionChangedAction.Remove)
      //  Debug.WriteLine($"NotifyCollectionChanged(action={args.Action}, oldItems.Count={oldItemsCount}, oldStartingIndex={args.OldStartingIndex})" +
      //  $" {DateTime.Now.ToString(dateTimeFormat)}");
      //else if (newItemsCount > 0 || oldItemsCount > 0)
      //  Debug.WriteLine($"NotifyCollectionChanged(action={args.Action}," +
      //    $" newItems.Count={newItemsCount}, newStartingIndex={args.NewStartingIndex})" +
      //    $" oldItems.Count={oldItemsCount}, oldStartingIndex={args.OldStartingIndex})" +
      //  $" {DateTime.Now.ToString(dateTimeFormat)}");
      //else
      //  Debug.WriteLine($"NotifyCollectionChanged(action={args.Action})" +
      //  $" {DateTime.Now.ToString(dateTimeFormat)}");

      var dispatcher = Dispatcher;
      foreach (NotifyCollectionChangedEventHandler handler in notifyCollectionChangedEventHandler.GetInvocationList())
      {
        if (dispatcher != null)
        {
          try
          {
            if (BeginInvokeActionEnabled)
              dispatcher.BeginInvoke(DispatcherPriority.Background, handler, GetNotifyObject(), args, null, null);
            else
              dispatcher.Invoke(DispatcherPriority.Background, handler, GetNotifyObject(), args);
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
              handler.BeginInvoke(GetNotifyObject(), args, null, null);
            else
              handler.Invoke(GetNotifyObject(), args);
          }
          catch (Exception ex)
          {
            Debug.WriteLine($"{ex.GetType()} thrown in NotifyCollectionChanged direct invoke:\n {ex.Message}");
          }
      }
    }


    /// <summary>
    /// Redirects event to <see cref="OnCollectionChanged"/>
    /// </summary>
    /// <param name="args"></param>
    public virtual void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      OnCollectionChanged(args);
    }
    #endregion INotifyCollectionChanged

  }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  public class ObservableCollectionObject : ObservableObject, INotifyCollectionChanged
  {
    public const string dateTimeFormat = "hh:mm:ss.fff";

    public ObservableCollectionObject() : base() { }

    public ObservableCollectionObject(Dispatcher dispatcher) : base(dispatcher) { }

    #region INotifyCollectionChanged

    public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

    public virtual event NotifyCollectionChangedEventHandler CollectionChangedImmediately;

    public bool NotifyCollectionChangedEnabled { get; set; } = true;

    public void BulkChangeStart(NotifyCollectionChangedAction action)
    {
      if (bulkAction != null)
        BulkChangeEnd();
      bulkAction = action;
      bulkItems = new List<object>();
      bulkIndex = -1;
    }
    public void BulkChangeEnd()
    {
      if (bulkAction != null)
      {
        var action = (NotifyCollectionChangedAction)bulkAction;
        bulkAction = null;
        if (bulkItems.Count > 0)
        {
          NotifyCollectionChanged(action, bulkItems, bulkIndex);
        }
        bulkItems = null;
      }
    }

    private NotifyCollectionChangedAction? bulkAction;
    private List<object> bulkItems;
    private int bulkIndex;

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      if (args.Action == bulkAction)
      {
        if (bulkIndex == -1)
          bulkIndex = args.NewStartingIndex;
        foreach (var item in args.NewItems)
          bulkItems.Add(item);
        //Debug.WriteLine("OnCollectionChanged return as action is bulkAction");
        return;
      }

      if (!NotifyCollectionChangedEnabled)
      {
        //Debug.WriteLine("OnCollectionChanged return as NotifyCollectionChangedEnabled is false");
        return;
      }

      HandleCollectionChangedEvent(CollectionChangedImmediately, args, true);
      HandleCollectionChangedEvent(CollectionChanged, args, false);
    }
    private void HandleCollectionChangedEvent(NotifyCollectionChangedEventHandler notifyCollectionChangedEventHandler, 
      NotifyCollectionChangedEventArgs args,  bool immediately)
    { 
      if (notifyCollectionChangedEventHandler != null)
      {
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

        if (Dispatcher != null)
        {
          //dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, args);
          Dispatcher.BeginInvoke(DispatcherPriority.Background,
            new Action<object, NotifyCollectionChangedEventArgs>(NotifyCollectionChangedEventHandler), this, args);
        }
        else

          foreach (NotifyCollectionChangedEventHandler handler in notifyCollectionChangedEventHandler.GetInvocationList())
          {
            handler.Invoke(this, args);
            //handler.BeginInvoke(this, args, null, null);
          }
      }
    }


    private void NotifyCollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs args)
    {
      lock (LockObject)
      {
        var notifyCollectionChangedEventHandler = CollectionChanged;
        if (notifyCollectionChangedEventHandler == null)
          return;
        foreach (NotifyCollectionChangedEventHandler handler in notifyCollectionChangedEventHandler.GetInvocationList())
        {
          handler.Invoke(this, args);
        }
      }
    }

    protected virtual void NotifyCollectionChanged(NotifyCollectionChangedAction action)
    {
      NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action));
    }

    public virtual void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      //NotifyPropertyChanged("Count");
      //NotifyPropertyChanged("Item[]");
      OnCollectionChanged(args);
    }

    public void NotifyCollectionChanged(NotifyCollectionChangedAction action, object changedItem, int index)
    {
      NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index));
    }

    public void NotifyCollectionChanged(NotifyCollectionChangedAction action, IList changedItems, int index)
    {
      NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems, index));
    }

    #endregion INotifyCollectionChanged

  }
}

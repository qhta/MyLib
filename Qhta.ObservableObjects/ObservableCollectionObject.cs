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
    public ObservableCollectionObject(Dispatcher dispatcher) : base(dispatcher) { }

    #region INotifyCollectionChanged

    public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

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
      if (args.Action==bulkAction)
      {
        if (bulkIndex == -1)
          bulkIndex = args.NewStartingIndex;
        foreach (var item in args.NewItems)
          bulkItems.Add(item);
        return;
      }

      if (!NotifyCollectionChangedEnabled)
        return;

      var notifyCollectionChangedEventHandler = CollectionChanged;

      if (notifyCollectionChangedEventHandler == null)
        return;

      foreach (NotifyCollectionChangedEventHandler handler in notifyCollectionChangedEventHandler.GetInvocationList())
      {
        if (_dispatcher != null)
        {
          //dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, args);
          _dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, args);
        }
        else
        {
          //handler.Invoke(this, args);
          handler.BeginInvoke(this, args, null, null);
        }
      }
    }

    protected virtual void NotifyCollectionChanged(NotifyCollectionChangedAction action)
    {
      NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action));
    }

    public virtual void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      NotifyPropertyChanged("Count");
      NotifyPropertyChanged("Item[]");
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

using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Threading;

namespace ObservableImmutable
{
  public class ObservableCollectionObject : ObservableObject, INotifyCollectionChanged
  {
    #region INotifyCollectionChanged

    public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      var notifyCollectionChangedEventHandler = CollectionChanged;

      if (notifyCollectionChangedEventHandler == null)
        return;

      var dispatcher = DispatcherHelper.Dispatcher;

      foreach (NotifyCollectionChangedEventHandler handler in notifyCollectionChangedEventHandler.GetInvocationList())
      {
        if (dispatcher == null)
        {
          var dispatcherObject = handler.Target as DispatcherObject;
          if (dispatcherObject != null && !dispatcherObject.CheckAccess())
            dispatcher = dispatcherObject.Dispatcher;
        }

        if (dispatcher != null)
        {
          dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, args);
        }
        else
          try
          {
            handler.BeginInvoke(this, args, null, null);
          }
          catch(Exception ex)
          {
            //Debug.WriteLine(ex.Message);
          }
      }
    }

    protected virtual void NotifyCollectionChanged()
    {
      NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    protected virtual void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
      NotifyPropertyChanged("Count");
      NotifyPropertyChanged("Item[]");
      OnCollectionChanged(args);
    }

    public void NotifyCollectionChanged(NotifyCollectionChangedAction action, object changedItem, int index)
    {
      NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index));
    }

    #endregion INotifyCollectionChanged


    #region Constructors

    public ObservableCollectionObject() : this(LockTypeEnum.Lock) { }

    protected ObservableCollectionObject(LockTypeEnum lockType)
    {
      Helper = new DispatcherHelper(lockType);
    }
    #endregion Constructors

    protected readonly DispatcherHelper Helper =new DispatcherHelper(default(LockTypeEnum));
  }
}

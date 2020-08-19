using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  public class ObservableObject: INotifyPropertyChanged
  {
    public static bool TraceCreationWithDispatcher { get; set; } = true;

    public ObservableObject(): this(Dispatcher.CurrentDispatcher)
    {
    }

    public ObservableObject(Dispatcher dispatcher)
    {
      if (dispatcher == null)
        dispatcher = Dispatcher.FromThread(Thread.CurrentThread);
#if DEBUG
      if (TraceCreationWithDispatcher && dispatcher != null)
      {
        if (dispatcher.Thread.GetApartmentState() != ApartmentState.STA)
          Debug.WriteLine($"{this.GetType().Name} should be created with STA thread dispatcher");
        //if (dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
        //  Debug.WriteLine($"{this.GetType().Name} should be created with GUI Access dispatcher");
        if (dispatcher.Thread.Name!="VSTA_Main")
          Debug.WriteLine($"{this.GetType().Name} should be created with VSTA_Main dispatcher");
      }
#endif
      Dispatcher = dispatcher;
    }

    public Dispatcher Dispatcher { get; private set; }

    public virtual void SetDispatcher (Dispatcher dispatcher)
    {
#if DEBUG
      if (TraceCreationWithDispatcher)
        if (dispatcher.Thread.GetApartmentState() != ApartmentState.STA)
          Debug.WriteLine($"{this} should be set with STA thread dispatcher");
#endif
      Dispatcher = dispatcher;
    }

    public virtual object LockObject => this;

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName) => NotifyPropertyChanged(this, propertyName);

    public void NotifyPropertyChanged(object sender, string propertyName)
    {
      var propertyChangedEventHandler = PropertyChanged;

      if (propertyChangedEventHandler == null)
        return;


      foreach (PropertyChangedEventHandler handler in propertyChangedEventHandler.GetInvocationList())
      {

        var args = new PropertyChangedEventArgs(propertyName);
        if (Dispatcher != null)
        {
          Dispatcher.BeginInvoke(DispatcherPriority.Background, handler, sender, args);
        }
        else
          try
          {
            handler.BeginInvoke(sender, args, null, null);
          }
          catch (Exception ex)
          {
            Debug.WriteLine($"{ex.GetType().Name} thrown in ObservableObject:\n {ex.Message}");
          }
      }
   }

    #endregion INotifyPropertyChanged

    #region Enable synchronization from BindingOperations
    public virtual bool IsSynchronized
    {
      get
      {
        //Debug.WriteLine("Get IsSynchronized");
        return true;
      }
    }

    public virtual object SyncRoot
    {
      get
      {
        //Debug.WriteLine("Get LockObject");
        return LockObject;
      }
    }
    #endregion

  }
}

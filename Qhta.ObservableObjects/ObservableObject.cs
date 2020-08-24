using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  public class ObservableObject : INotifyPropertyChanged
  {
    public static Dispatcher CommonDispatcher { get; set; }

    public ObservableObject()
    {
    }

    public Dispatcher Dispatcher
    {
      get => _Dispatcher ?? CommonDispatcher;
      set
      {
#if DEBUG
        {
          if (value.Thread.GetApartmentState() != ApartmentState.STA)
            Debug.WriteLine($"{this} should be set with STA thread dispatcher");
          if (value.Thread.Name != "VSTA_Main")
            Debug.WriteLine($"{this.GetType().Name} should be created with VSTA_Main dispatcher");
        }
#endif
        Dispatcher = value;
      }
    }
    protected Dispatcher _Dispatcher;

    public virtual object LockObject => this;

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName) => NotifyPropertyChanged(this, propertyName);

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
          var handled = false;
          //if (dispatcher == Dispatcher.CurrentDispatcher)
          //  try
          //  {
          //    dispatcher.Invoke(handler, sender, args);
          //    handled = true;
          //  }
          //  catch (InvalidOperationException)
          //  {

          //  }
          if (!handled)
            dispatcher.BeginInvoke(DispatcherPriority.Background, handler, sender, args);
        }
        else
          try
          {
            handler.Invoke(sender, args);
            //handler.BeginInvoke(sender, args, null, null);
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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace Qhta.ObservableObjects
{
  public class ObservableObject: INotifyPropertyChanged
  {
    public ObservableObject(Dispatcher dispatcher)
    {
      _dispatcher = dispatcher;
    }
    protected Dispatcher _dispatcher;
    public virtual object LockObject => this;

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      var propertyChangedEventHandler = PropertyChanged;

      if (propertyChangedEventHandler == null)
        return;


      foreach (PropertyChangedEventHandler handler in propertyChangedEventHandler.GetInvocationList())
      {

        var args = new PropertyChangedEventArgs(propertyName);
        if (_dispatcher != null)
        {
          _dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, args);
        }
        else
          try
          {
            handler.BeginInvoke(this, args, null, null);
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

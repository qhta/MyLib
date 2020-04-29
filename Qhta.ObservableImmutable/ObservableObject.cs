using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace ObservableImmutable
{
  public class ObservableObject: INotifyPropertyChanged
  {
    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      var propertyChangedEventHandler = PropertyChanged;

      if (propertyChangedEventHandler == null)
        return;

      var dispatcher = DispatcherHelper.Dispatcher;

      foreach (PropertyChangedEventHandler handler in propertyChangedEventHandler.GetInvocationList())
      {
        if (dispatcher == null)
        {
          var dispatcherObject = handler.Target as DispatcherObject;
          if (dispatcherObject != null && !dispatcherObject.CheckAccess())
            dispatcher = dispatcherObject.Dispatcher;
        }

        var args = new PropertyChangedEventArgs(propertyName);
        if (dispatcher != null)
        {
          dispatcher.BeginInvoke(DispatcherPriority.DataBind, handler, this, args);
        }
        else
          try
          {
            handler.BeginInvoke(this, args, null, null);
          }
          catch (Exception ex)
          {
            //Debug.WriteLine(ex.Message);
          }
      }
   }

    #endregion INotifyPropertyChanged

  }
}

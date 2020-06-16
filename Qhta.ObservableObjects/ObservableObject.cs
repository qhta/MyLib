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
    protected object _lockObject = new object();

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
            //Debug.WriteLine(ex.Message);
          }
      }
   }

    #endregion INotifyPropertyChanged

  }
}

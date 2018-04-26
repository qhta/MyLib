using System;
using System.ComponentModel;
using MyLib.MultiThreadingObjects;

namespace MyLib.MVVM
{
  public class ViewModel : DispatchedObject, IViewModel, INotifyPropertyChanged
  {
    //public event PropertyChangedEventHandler PropertyChanged;

    public new void NotifyPropertyChanged(string propertyName)
    {
      base.NotifyPropertyChanged(propertyName);
    //  if (PropertyChanged!=null)
    //    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public virtual bool IsValid => true;

  }
}

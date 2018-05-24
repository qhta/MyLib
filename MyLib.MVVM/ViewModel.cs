using System;
using System.ComponentModel;
using MyLib.MultiThreadingObjects;

namespace MyLib.MVVM
{
  public class ViewModel : DispatchedObject, IValidated, INotifyPropertyChanged
  {
    public new void NotifyPropertyChanged(string propertyName)
    {
      base.NotifyPropertyChanged(propertyName);
    }
    public virtual bool? IsValid { get; set; }

  }
}

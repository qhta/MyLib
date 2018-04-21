using System;
using System.ComponentModel;

namespace MyLib.MVVM
{
  public class ViewModel : IViewModel, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged!=null)
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public virtual bool IsValid => true;

  }
}

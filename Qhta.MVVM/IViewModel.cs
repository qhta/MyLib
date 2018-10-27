using System.ComponentModel;

namespace Qhta.MVVM
{
  public interface IViewModel: INotifyPropertyChanged
  {
    void NotifyPropertyChanged(string propertyName);

  }
}

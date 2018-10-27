using System.ComponentModel;
using Qhta.DispatchedObjects;

namespace Qhta.MVVM
{
  public class ViewModel : DispatchedObject, IValidated, INotifyPropertyChanged, IViewModel
  {
    public new void NotifyPropertyChanged(string propertyName)
    {
      base.NotifyPropertyChanged(propertyName);
    }
    public virtual bool? IsValid { get; set; }

  }
}

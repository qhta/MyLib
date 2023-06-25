using System.ComponentModel;

namespace Qhta.MVVM
{
  /// <summary>
  /// An interface expanding INotifyPropertyChanged (from System.ComponentModel) with NotifyPropertyChanged method.
  /// </summary>
  public interface IViewModel: INotifyPropertyChanged
  {
    /// <summary>
    /// A method to notify that a property value has changed.
    /// </summary>
    /// <param name="propertyName"></param>
    void NotifyPropertyChanged(string propertyName);
  }
}

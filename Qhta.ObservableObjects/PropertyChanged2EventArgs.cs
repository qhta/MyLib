using System.ComponentModel;

namespace Qhta.ObservableObjects;

/// <summary>
///   Event args for property changed event that includes old and new values.
/// </summary>
public class PropertyChanged2EventArgs: PropertyChangedEventArgs
{
  /// <summary>
  /// Old value of the property.
  /// </summary>
  public object? OldValue { get; }

  /// <summary>
  /// New value of the property.
  /// </summary>
  public object? NewValue { get; }

  /// <summary>
  /// Constructor for PropertyChanged2EventArgs with property name, old value and new value.
  /// </summary>
  /// <param name="propertyName"></param>
  /// <param name="oldValue"></param>
  /// <param name="newValue"></param>
  public PropertyChanged2EventArgs(string propertyName, object? oldValue, object? newValue)
    : base(propertyName)
  {
    OldValue = oldValue;
    NewValue = newValue;
  }
}
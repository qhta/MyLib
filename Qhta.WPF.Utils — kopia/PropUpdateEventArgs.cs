namespace Qhta.WPF.Utils;

/// <summary>
/// CancelEventArgs that holds information on updating a property.
/// </summary>
public class PropUpdateEventArgs: CancelEventArgs
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="targetObject"></param>
  /// <param name="property"></param>
  /// <param name="newValue"></param>
  /// <param name="oldValue"></param>
  public PropUpdateEventArgs(object targetObject, PropertyInfo property, object? newValue, object? oldValue)
  {
    TargetObject = targetObject;
    Property = property;
    NewValue = newValue;
    OldValue = oldValue;
  }

  /// <summary>
  /// Updated object
  /// </summary>
  public object TargetObject { get; set; }

  /// <summary>
  /// Updated property info.
  /// </summary>
  public PropertyInfo Property { get; private set; }

  /// <summary>
  /// New value of the property.
  /// </summary>
  public object? NewValue { get; private set; }

  /// <summary>
  /// Old value of the property.
  /// </summary>
  public object? OldValue { get; private set; }
}

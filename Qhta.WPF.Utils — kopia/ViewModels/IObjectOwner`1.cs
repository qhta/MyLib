namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// Interface of object that owns components of the specific type and can change a reference of it to new instance.
/// </summary>
public interface IObjectOwner<T>
{
  /// <summary>
  /// Gets a component referenced by a specific property.
  /// </summary>
  public object? GetComponent(string propName);

  /// <summary>
  /// Changes a component referenced by a specific property to other component.
  /// Returns true if the change was successfull.
  /// </summary>
  public bool ChangeComponent(string propName, object? newComponent);

  /// <summary>
  /// Changes a specific component property to other component.
  /// Returns true if the change was successfull.
  /// </summary>
  public bool ChangeComponent(object? oldComponent, object? newComponent);

}

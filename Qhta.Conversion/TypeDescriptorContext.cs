namespace Qhta.Conversion;

/// <summary>
/// Class implementing ITypeDescriptorContext interface (defined in System.ComponentModel).
/// Parameter of this interface type is used in standard TypeConverter methods.
/// </summary>
public class TypeDescriptorContext : ITypeDescriptorContext
{
  /// <summary>
  /// Initializing constructor
  /// </summary>
  /// <param name="instance"></param>
  /// <param name="propertyInfo"></param>
  public TypeDescriptorContext(object instance, PropertyInfo? propertyInfo = null)
  {
    Instance = instance;
    if (instance is IContainer container)
      Container = container;
    if (propertyInfo != null)
      PropertyDescriptor = new PropertyInfoDescriptor(propertyInfo);
  }

  /// <inheritdoc/>
  public object? GetService(Type serviceType)
  {
    return null;
  }

  /// <inheritdoc/>
  public void OnComponentChanged()
  {
  }

  /// <inheritdoc/>
  public bool OnComponentChanging()
  {
    return false;
  }

  /// <inheritdoc/>
  public IContainer Container { get; } = null!;

  /// <inheritdoc/>
  public object Instance { get; }

  /// <inheritdoc/>
  public PropertyDescriptor PropertyDescriptor { get; } = null!;
}
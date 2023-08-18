namespace Qhta.Conversion;

/// <summary>
/// Extends PropertyDescriptor class (declared in System.ComponentModel) with PropertyInfo data.
/// </summary>
public class PropertyInfoDescriptor : PropertyDescriptor
{
  /// <summary>
  /// Access property to PropertyInfo.
  /// </summary>
  public PropertyInfo PropertyInfo { get; private set; } = null!;

  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="info"></param>
  public PropertyInfoDescriptor(PropertyInfo info) : base(info.Name, info.GetCustomAttributes().ToArray())
  {
    PropertyInfo = info;
  }

  /// <summary>
  /// Gets PropertyInfo declaring type.
  /// </summary>
  public override Type ComponentType => PropertyInfo.DeclaringType ?? typeof(object);

  /// <summary>
  /// Informs if a property is read only.
  /// </summary>
  public override bool IsReadOnly => !PropertyInfo.CanWrite;

  /// <summary>
  /// Gets PropertyInfo property type.
  /// </summary>
  public override Type PropertyType => PropertyInfo.PropertyType;


  /// <summary>
  /// Sets a value to the component using PropertyInfo.
  /// </summary>
  /// <param name="component"></param>
  /// <param name="value"></param>
  public override void SetValue(object? component, object? value)
  {
    PropertyInfo.SetValue(component, value);
  }

  /// <summary>
  /// Gets a value from the component using PropertyInfo.
  /// </summary>
  /// <param name="component"></param>
  /// <returns></returns>
  public override object? GetValue(object? component)
  {
    return PropertyInfo.GetValue(component);
  }


  /// <summary>
  /// Checks if the PropertyInfo can write.
  /// </summary>
  /// <param name="component"></param>
  /// <returns></returns>
  public override bool CanResetValue(object component)
  {
    return PropertyInfo.CanWrite;
  }

  /// <summary>
  /// If a PropertyType is not value type then sets null to component using PropertyInfo.
  /// Otherwise sets 0 (zero).
  /// </summary>
  /// <param name="component"></param>
  public override void ResetValue(object component)
  {
    if (!PropertyType.IsValueType)
      PropertyInfo.SetValue(component, null);
    else
      PropertyInfo.SetValue(component, 0);
  }

  /// <summary>
  /// Checks whether the component type contains ShouldSerializeXXXXX method (where XXXXX is the property name).
  /// If so, it invokes this method on the component and returns the boolean result.
  /// Otherwise it returns true.
  /// </summary>
  /// <param name="component"></param>
  /// <returns></returns>
  public override bool ShouldSerializeValue(object component)
  {
    var methodInfo = ComponentType.GetMethod("ShouldSerialize" + PropertyInfo.Name);
    if (methodInfo != null)
    {
      try
      {
        var result = (methodInfo.Invoke(component, null));
        if (result is bool bv)
          return bv;
      }
      catch { }
    }
    return true;
  }
}
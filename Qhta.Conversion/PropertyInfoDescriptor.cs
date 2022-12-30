using System.ComponentModel;
using System.Reflection;

namespace Qhta.Conversion;

public class PropertyInfoDescriptor: PropertyDescriptor
{
  public PropertyInfoDescriptor(PropertyInfo info) : base(info.Name, info.GetCustomAttributes().ToArray())
  {
    PropertyInfo = info;
  }

  public PropertyInfo PropertyInfo = null!;


  public override void SetValue(object? component, object? value)
  {
    PropertyInfo.SetValue(component, value);
  }

  public override object? GetValue(object? component)
  {
    return PropertyInfo.GetValue(component);
  }


  public override bool CanResetValue(object component)
  {
    return false;
  }

  public override void ResetValue(object component)
  {
  }

  public override bool ShouldSerializeValue(object component)
  {
    return false;
  }

  public override Type ComponentType => PropertyInfo.DeclaringType ?? typeof(object);

  public override bool IsReadOnly  => false;

  public override Type PropertyType => PropertyInfo.PropertyType;
}
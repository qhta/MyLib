﻿using System.ComponentModel;
using System.Reflection;

namespace Qhta.Conversion;

public class TypeDescriptorContext : ITypeDescriptorContext
{
  public TypeDescriptorContext(object instance, PropertyInfo? propertyInfo = null)
  {
    Instance = instance;
    if (instance is IContainer container)
      Container = container;
    if (propertyInfo != null)
      PropertyDescriptor = new PropertyInfoDescriptor(propertyInfo);
  }

  public object? GetService(Type serviceType)
  {
    return null;
  }

  public void OnComponentChanged()
  {
  }

  public bool OnComponentChanging()
  {
    return false;
  }

  public IContainer Container { get; } = null!;

  public object Instance { get; }

  public PropertyDescriptor PropertyDescriptor { get; } = null!;
}
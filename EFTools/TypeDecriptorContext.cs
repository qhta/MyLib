using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTools
{
  public class TypeDescriptorContext : ITypeDescriptorContext
  {

    public TypeDescriptorContext(Type componentType, string propertyName)
    {
      PropertyDescriptor = TypeDescriptor.GetProperties(componentType)[propertyName];
    }

    public TypeDescriptorContext(object instance, string propertyName)
    {
      Instance = instance;
      PropertyDescriptor = TypeDescriptor.GetProperties(instance)[propertyName];
    }

    public object Instance { get; private set; }
    public PropertyDescriptor PropertyDescriptor { get; private set; }
    public IContainer Container { get; private set; }

    public void OnComponentChanged()
    {
    }

    public bool OnComponentChanging()
    {
      return true;
    }

    public object GetService(Type serviceType)
    {
      return null;
    }
  }
}

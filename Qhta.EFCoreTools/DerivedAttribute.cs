using System;

namespace Qhta.EFTools
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple =false, Inherited =false)]
  public class DerivedAttribute: Attribute
  {
    public string Property { get; set; }

    public string GetMethod { get; set; }

    public Type TypeConverter { get; set; }
  }
}

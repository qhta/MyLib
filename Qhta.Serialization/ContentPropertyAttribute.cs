using System;

namespace Qhta.Serialization
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public class ContentPropertyAttribute: Attribute
  {
    public ContentPropertyAttribute(string name)
    {
      Name = name;
    }
    public string Name { get; init; }
  }
}

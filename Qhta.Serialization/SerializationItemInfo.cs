using System;
using System.Reflection;

namespace Qhta.Serialization
{
  public class SerializationItemInfo: SerializationTypeInfo
  {

    public SerializationItemInfo(string elementName, PropertyInfo info, Type type): base(elementName, type)
    {
      PropInfo = info;
    }

    public PropertyInfo PropInfo { get; init; }

  }

}

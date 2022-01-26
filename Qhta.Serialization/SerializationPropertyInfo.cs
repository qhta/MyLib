using System.Reflection;

namespace Qhta.Serialization
{
  public class SerializationPropertyInfo
  {

    public SerializationPropertyInfo(string name, PropertyInfo info, int order)
    {
      Name = name;
      PropInfo = info;
      Order = order;
    }

    public int Order { get; init; }
    public string Name { get; init; }
    public PropertyInfo PropInfo { get; init; }

  }

}

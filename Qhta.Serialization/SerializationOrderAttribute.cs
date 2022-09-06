namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SerializationOrderAttribute: System.Attribute
{
  public SerializationOrderAttribute() : base() { }

  public SerializationOrderAttribute(int order) : base() { Order = order; }

  public int Order { get; set; }
}
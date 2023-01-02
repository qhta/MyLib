namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct 
                | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class QXmlElementTypeAttribute: XmlArrayItemAttribute
{
  public QXmlElementTypeAttribute(): base()
  {
  }

  public QXmlElementTypeAttribute(string? elementName): base (elementName)
  {
  }

  public QXmlElementTypeAttribute(Type? type): base(type)
  {
  }

  public QXmlElementTypeAttribute(string? elementName, Type? type): base(elementName, type)
  {
  }

  public object? Value { get; set; }

  public Type? ConverterType { get; set; }

}
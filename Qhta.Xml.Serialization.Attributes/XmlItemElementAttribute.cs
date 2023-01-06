namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
public class XmlItemElementAttribute : XmlArrayItemAttribute
{
  public XmlItemElementAttribute()
  {
  }

  public XmlItemElementAttribute(string? elementName) : base(elementName)
  {
  }

  public XmlItemElementAttribute(string? elementName, Type? type) : base(elementName, type)
  {
  }

  public XmlItemElementAttribute(Type? type) : base(type)
  {
  }

  public string? AddMethod { get; set; }

  public object? Value { get; set; }

  public Type? ConverterType { get; set; }
}
namespace Qhta.Xml.Serialization;

/// <summary>
/// Overrides attribute usage of the original XmlArrayItemAttribute
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class QXmlArrayItemAttribute: XmlArrayItemAttribute
{
  public QXmlArrayItemAttribute(): base()
  {
  }

  public QXmlArrayItemAttribute(string? elementName): base (elementName)
  {
  }

  public QXmlArrayItemAttribute(Type? type): base(type)
  {
  }

  public QXmlArrayItemAttribute(string? elementName, Type? type): base(elementName, type)
  {
  }

  public object? Value { get; set; }
}
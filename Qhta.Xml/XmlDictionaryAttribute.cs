namespace Qhta.Xml;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class XmlDictionaryAttribute : XmlCollectionAttribute
{
  public XmlDictionaryAttribute()
  {
  }

  public XmlDictionaryAttribute(string? elementName) : base(elementName)
  {
  }

  public Type? KeyType { get; set; }

  public string? KeyName { get; set; }

  public Type? ValueType { get; set; }

  public bool AttributesAreKeys { get; set; }

  public bool ElementsAreKeys { get; set; }
}
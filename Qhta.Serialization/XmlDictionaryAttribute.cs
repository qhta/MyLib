namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
public class XmlDictionaryAttribute : XmlCollectionAttribute
{
  public XmlDictionaryAttribute() : base() { }

  public XmlDictionaryAttribute(string? elementName) : base(elementName) { }

  public Type? KeyType { get; set;}

  public string? KeyName { get; set; }

  public Type? ValueType { get; set; }

  public bool AttributesAreKeys { get; set; }

  public bool ElementsAreKeys { get; set; }
}
namespace Qhta.Xml;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class XmlCollectionAttribute : XmlArrayAttribute
{
  public XmlCollectionAttribute()
  {
  }

  public XmlCollectionAttribute(string? elementName, Type? collectionType = null)
  {
    ElementName = elementName;
    CollectionType = collectionType;
  }

  public Type? CollectionType { get; }

  //public Type? ItemType { get;}

  public string? AddMethod { get; set; }

  public Type? XmlConverter { get; set; }
}
using System;
using System.Xml.Serialization;

namespace Qhta.Xml.Serialization
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
  public class XmlCollectionAttribute : XmlArrayAttribute
  {

    public XmlCollectionAttribute(string? elementName, Type? collectionType = null)
    { 
      base.ElementName = elementName;
      CollectionType = collectionType;
    }

    public Type? CollectionType { get; init; }

    //public Type? ItemType { get; init; }

    public string? AddMethod { get; set; }

    public Type? XmlConverter {get; set; }
  }
}

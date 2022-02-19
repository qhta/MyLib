using System;
#nullable enable

namespace Qhta.Xml.Serialization
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class XmlCollectionAttribute : System.Xml.Serialization.XmlArrayAttribute
  {

    public XmlCollectionAttribute(string? elementName, Type? collectionType = null) : base(elementName) 
    { 
      CollectionType = collectionType;
    }

    public Type? CollectionType { get; init; }
  }
}

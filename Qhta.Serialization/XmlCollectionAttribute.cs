﻿using System.Xml.Serialization;

namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
public class XmlCollectionAttribute : XmlArrayAttribute
{
  public XmlCollectionAttribute()
  {
  }

  public XmlCollectionAttribute(string? elementName, Type? collectionType = null)
  { 
    base.ElementName = elementName;
    CollectionType = collectionType;
  }

  public Type? CollectionType { get;}

  //public Type? ItemType { get;}

  public string? AddMethod { get; set; }

  public Type? XmlConverter {get; set; }
}
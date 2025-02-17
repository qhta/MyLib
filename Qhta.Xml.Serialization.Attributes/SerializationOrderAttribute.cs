﻿namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Property)]
public class SerializationOrderAttribute : Attribute
{
  public SerializationOrderAttribute()
  {
  }

  public SerializationOrderAttribute(int order)
  {
    Order = order;
  }

  public int Order { get; set; }
}
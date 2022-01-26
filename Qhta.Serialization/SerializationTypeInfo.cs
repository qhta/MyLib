using System;
using System.ComponentModel;
using System.Reflection;

namespace Qhta.Serialization
{
  public class SerializationTypeInfo
  {

    public SerializationTypeInfo(String elementName, Type aType)
    {
      ElementName = elementName;
      Type = aType;
    }

    /// <summary>
    /// XmlElement name for a type
    /// </summary>
    public string ElementName { get; set; }

    /// <summary>
    /// A type to serialize or deserialize
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// A public constructor to invoke while deserialization
    /// </summary>
    public ConstructorInfo? KnownConstructor { get; set; }

    /// <summary>
    /// Converter to/from string value.
    /// </summary>
    public TypeConverter? TypeConverter { get; set; }

    /// <summary>
    /// Known properties to serialize as XML attributes.
    /// </summary>
    public KnownPropertiesDictionary PropsAsAttributes { get; set; } = new KnownPropertiesDictionary();

    /// <summary>
    /// Known properties to serialize as XML elements.
    /// </summary>
    public KnownPropertiesDictionary PropsAsElements { get; set; } = new KnownPropertiesDictionary();

    /// <summary>
    /// KnownTypes for collection items.
    /// </summary>
    public KnownTypesDictionary KnownItems { get; set; } = new KnownTypesDictionary();

  }
}

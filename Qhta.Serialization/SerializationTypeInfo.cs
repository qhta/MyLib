using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

using Qhta.TestHelper;
using Qhta.TypeUtils;

namespace Qhta.Serialization
{
  public class SerializationTypeInfo: ITypeInfo
  {

    public SerializationTypeInfo(Type aType)
    {
      Type = aType;
    }

    /// <summary>
    /// XmlElement name for a type
    /// </summary>
    public string? ElementName { get; set; }

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
    /// Known property to accept content of XmlElement.
    /// </summary>
    public SerializationPropertyInfo? KnownContentProperty { get; set; }

    /// <summary>
    /// Known property to accept text content of XmlElement.
    /// </summary>
    public SerializationPropertyInfo? KnownTextProperty { get; set; }

    /// <summary>
    /// KnownTypes for collection items.
    /// </summary>
    public KnownTypesDictionary KnownItems { get; set; } = new KnownTypesDictionary();

  }
}

// Decompiled with JetBrains decompiler
// Type: System.Xml.Serialization.XmlTypeMapping
// Assembly: System.Xml.XmlSerializer, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F49AF7-B84F-4E1C-B6C6-7C7A3706639D
// Assembly location: C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.8\ref\net6.0\System.Xml.XmlSerializer.dll
// XML documentation location: C:\Program Files\dotnet\packs\Microsoft.NETCore.App.Ref\6.0.8\ref\net6.0\System.Xml.XmlSerializer.xml


#nullable enable
namespace System.Xml.Serialization
{
  /// <summary>Contains a mapping of one type to another.</summary>
  public class XmlTypeMapping : XmlMapping
  {
    internal XmlTypeMapping();

    /// <summary>The fully qualified type name that includes the namespace (or namespaces) and type.</summary>
    /// <returns>The fully qualified type name.</returns>
    public string TypeFullName { get; }

    /// <summary>Gets the type name of the mapped object.</summary>
    /// <returns>The type name of the mapped object.</returns>
    public string TypeName { get; }

    /// <summary>Gets the XML element name of the mapped object.</summary>
    /// <returns>The XML element name of the mapped object. The default is the class name of the object.</returns>
    public string? XsdTypeName { get; }

    /// <summary>Gets the XML namespace of the mapped object.</summary>
    /// <returns>The XML namespace of the mapped object. The default is an empty string ("").</returns>
    public string? XsdTypeNamespace { get; }
  }
}
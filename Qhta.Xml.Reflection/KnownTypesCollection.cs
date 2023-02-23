namespace Qhta.Xml.Reflection;

/// <summary>
/// Named collection of serialization type info
/// </summary>
public class KnownTypesCollection : TypeInfoCollection<SerializationTypeInfo>
{
  private readonly KnownNamespacesCollection KnownNamespaces;

  /// <summary>
  /// Initializes a new instance of the <see cref="KnownTypesCollection"/> class.
  /// </summary>
  /// <param name="knownNamespaces">Known namespaces collection.</param>
  public KnownTypesCollection(KnownNamespacesCollection knownNamespaces)
  {
    KnownNamespaces = knownNamespaces;
  }

  /// <summary>
  /// Dumps this instance to debug output window
  /// </summary>
  public void Dump()
  {
    Debug.WriteLine("KnownTypes:");
    Debug.Indent();
    foreach (var item in this) Dump(item);
    Debug.Unindent();
  }

  /// <summary>
  /// Dumps the specified type information to debug output window
  /// </summary>
  /// <param name="typeInfo">The type information.</param>
  public void Dump(SerializationTypeInfo typeInfo)
  {
    KnownNamespaces.XmlNamespaceToPrefix.TryGetValue(typeInfo.XmlNamespace ?? "", out var prefix);
    Debug.WriteLine($"\"{prefix}:{typeInfo.XmlName}\" = type {typeInfo.Type}");
    typeInfo.MembersAsAttributes.Dump("  Attributes:", KnownNamespaces);
    typeInfo.MembersAsElements.Dump("  Elements:", KnownNamespaces);
  }
}
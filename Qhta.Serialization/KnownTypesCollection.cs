namespace Qhta.Xml.Serialization;

public class KnownTypesCollection : TypeInfoCollection<SerializationTypeInfo>
{
  private readonly KnownNamespacesCollection KnownNamespaces;

  public KnownTypesCollection(KnownNamespacesCollection knownNamespaces)
  {
    KnownNamespaces = knownNamespaces;
  }

  public void Dump()
  {
    Debug.WriteLine("KnownTypes:");
    Debug.Indent();
    foreach (var item in this) Dump(item);
    Debug.Unindent();
  }

  public void Dump(SerializationTypeInfo typeInfo)
  {
    KnownNamespaces.XmlNamespaceToPrefix.TryGetValue(typeInfo.XmlNamespace ?? "", out var prefix);
    Debug.WriteLine($"\"{prefix}:{typeInfo.XmlName}\" = type {typeInfo.Type}");
    typeInfo.MembersAsAttributes.Dump("  Attributes:", KnownNamespaces);
    typeInfo.MembersAsElements.Dump("  Elements:", KnownNamespaces);
  }
}
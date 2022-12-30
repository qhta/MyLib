using System.Xml.Linq;

namespace Qhta.Xml.Serialization;

public class KnownTypesCollection: TypesInfoCollection<SerializationTypeInfo>
{
  public KnownTypesCollection(KnownNamespacesCollection knownNamespaces)
  {
    KnownNamespaces = knownNamespaces;
  }

  private KnownNamespacesCollection KnownNamespaces;

  public void Dump()
  {
    Debug.WriteLine("KnownTypes:");
    Debug.Indent();
    foreach (var item in this)
    {
      Dump(item);
    }
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
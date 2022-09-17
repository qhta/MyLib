using System.Xml.Linq;

namespace Qhta.Xml.Serialization;

public class KnownTypesCollection: TypesInfoCollection<SerializationTypeInfo>
{

  public void Dump()
  {
    Debug.WriteLine("KnownTypes:");
    Debug.Indent();
    foreach (var item in this)
    {
      Debug.WriteLine($"\"{item.XmlName}\" = type {item.Type}");
      item.MembersAsAttributes.Dump("  Attributes:");
      item.MembersAsElements.Dump("  Elements:");
    }
    Debug.Unindent();
  }
}
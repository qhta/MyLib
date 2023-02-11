namespace Qhta.Xml.Serialization;

public class KnownItemTypesCollection : TypeInfoCollection<SerializationItemInfo>
{
  public new void Add(SerializationItemInfo item)
  {
    base.Add(item.XmlName, item);
  }

  public new void Add(string xmlName, SerializationItemInfo item)
  {
    base.Add(xmlName, item);
  }
}
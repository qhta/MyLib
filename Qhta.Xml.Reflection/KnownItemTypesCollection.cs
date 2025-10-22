namespace Qhta.Xml.Reflection;

/// <summary>
/// Named collection of serialization item info. Needed for serialization.
/// </summary>
public class KnownItemTypesCollection : TypeInfoCollection<SerializationItemInfo>
{
  /// <summary>
  /// Adds an item to collection.
  /// </summary>
  /// <param name="item">The object to add. />.</param>
  public new void Add(SerializationItemInfo item)
  {
    if (Contains(item))
      return;
    base.Add(item);
    if (item.TypeInfo.KnownSubtypes!=null)
      foreach (var subType in item.TypeInfo.KnownSubtypes)
      {
        var subItem = new SerializationItemInfo(subType);
        base.Add(subItem);
      }
  }
}
namespace Qhta.Xml.Serialization;

public class PropOrderComparer : IComparer<SerializationPropertyInfo>
{
  public int Compare(SerializationPropertyInfo? x, SerializationPropertyInfo? y)
  {
    if (x== null && y == null)
      return 1;
    if (x == null || y == null)
      return 0;
    var result = x.Order.CompareTo(y.Order);
    if (result != 0)
      return result;
    if (x.Name.IsEmpty() && y.Name.IsEmpty())
      return 1;
    if (x.Name.IsEmpty() || y.Name.IsEmpty())
      return 0;
    return x.Name.CompareTo(y.Name);
  }
}
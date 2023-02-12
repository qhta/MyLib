namespace Qhta.Xml.Reflection;

public class PropOrderComparer : IComparer<SerializationMemberInfo>
{
  public int Compare(SerializationMemberInfo? x, SerializationMemberInfo? y)
  {
    if (x == null && y == null)
      return 1;
    if (x == null || y == null)
      return 0;
    var result = x.Order.CompareTo(y.Order);
    if (result != 0)
      return result;
    if (x.QualifiedName.IsEmpty() && y.QualifiedName.IsEmpty())
      return 1;
    if (x.QualifiedName.IsEmpty() || y.QualifiedName.IsEmpty())
      return 0;
    return x.QualifiedName.CompareTo(y.QualifiedName);
  }
}
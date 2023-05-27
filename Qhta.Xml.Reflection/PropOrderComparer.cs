namespace Qhta.Xml.Reflection;

/// <summary>
/// Class to compare order of properties for serialization.
/// </summary>
public class PropOrderComparer : IComparer<SerializationMemberInfo>
{
  /// <summary>
  /// Compares two serialization member info objecst.
  /// </summary>
  /// <param name="x">The first object to compare.</param>
  /// <param name="y">The second object to compare.</param>
  /// <returns>
  /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.
  /// <list type="table">
  /// <listheader><term> Value</term><description> Meaning</description></listheader><item>
  /// <term> Less than zero</term><description><paramref name="x" /> is less than <paramref name="y" />.</description>
  /// </item><item><term> Zero</term><description><paramref name="x" /> equals <paramref name="y" />.</description>
  /// </item><item><term> Greater than zero</term><description><paramref name="x" /> is greater than <paramref name="y" />.</description>
  /// </item></list>
  /// </returns>
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
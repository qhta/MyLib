namespace Qhta.Xml.Serialization;

/// <summary>
/// Provides utility methods for working with referenced objects, including converting object values  to their
/// corresponding reference string representations.
/// </summary>
/// <remarks>This class is designed to assist with serialization and deserialization scenarios where objects  are
/// identified by reference strings. It supports composite keys and handles null values gracefully.</remarks>
public static class ReferencedObjectHelper
{
  /// <summary>
  /// Helper method to get the reference string of an object instance based on the provided member information.
  /// </summary>
  /// <param name="idProperty"></param>
  /// <param name="instance"></param>
  /// <returns>string version of the value</returns>
  public static string? GetReferenceString(object? instance, SerializationMemberInfo idProperty)
  {
    if (instance == null)
      return null;
    var str = idProperty.Property?.GetValue(instance, [])?.ToString();
    if (idProperty.IsCompositeKey && idProperty.MoreMembers != null)
    {
      foreach (var prop in idProperty.MoreMembers)
      {
        var v = prop.GetValue(instance, [])?.ToString();
        if (v != null)
          str += "," + v;
      }
      str = "(" + str + ")";
    }
    return str;
  }
}
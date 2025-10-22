namespace Qhta.Xml.Reflection;

/// <summary>
/// Interface for KnownMembersCollection to be used as a dictionary
/// </summary>
public interface IMembersDictionary : IEnumerable<SerializationMemberInfo>
{
  /// <summary>
  /// Checks if it contains a qualified name (name with namespace) as an item key.
  /// </summary>
  /// <param name="qualifiedName">QualifiedName to search</param>
  /// <returns><c>true</c> if name found, <c>false</c> otherwise.</returns>
  public bool ContainsKey(QualifiedName qualifiedName);

  /// <summary>
  /// Checks if it contains a simple name (without namespace) as an item key.
  /// </summary>
  /// <param name="name">Simple name to search</param>
  /// <returns><c>true</c> if name found, <c>false</c> otherwise.</returns>
  public bool ContainsKey(string name);
  
  /// <summary>
  /// Tries to get an item searching its qualified name (name with namespace).
  /// </summary>
  /// <param name="qualifiedName">QualifiedName to search</param>
  /// <param name="typeInfo">FoundSerializationMemberInfo (or null when not found).</param>
  /// <returns><c>true</c> if name found, <c>false</c> otherwise.</returns>
  public bool TryGetValue(QualifiedName qualifiedName, [MaybeNullWhen(false)] out SerializationMemberInfo typeInfo);

  /// <summary>
  /// Tries to get an item searching its simple name (without namespace).
  /// </summary>
  /// <param name="name">Simple name to search</param>
  /// <param name="typeInfo">FoundSerializationMemberInfo (or null when not found).</param>
  /// <returns><c>true</c> if name found, <c>false</c> otherwise.</returns>
  public bool TryGetValue(string name, [MaybeNullWhen(false)] out SerializationMemberInfo typeInfo);

}
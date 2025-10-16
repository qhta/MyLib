namespace Qhta.Xml.Serialization;

/// <summary>
/// Collection of referenced objects during serialization/deserialization.
/// </summary>
public class ReferencedObjects
{
  private readonly Dictionary<Type, Dictionary<string, object>> _referencedObjects = new();

  /// <summary>
  /// Adds a referenced object with a specified key and type.
  /// </summary>
  /// <param name="type">The type of the referenced object.</param>
  /// <param name="key">Primary key for the referenced object. Unique in T type.</param>
  /// <param name="value">Object of class T</param>
  public void AddReference(Type type, string key, object value)
  {
    if (!_referencedObjects.ContainsKey(type))
    {
      _referencedObjects[type] = new Dictionary<string, object>();
    }
    _referencedObjects[type][key] = value;
  }

  /// <summary>
  /// Retrieves a reference of the specified type associated with the given key.
  /// </summary>
  /// <param name="type">The type of the referenced object.</param>
  /// <param name="key">Primary key for the referenced object. Unique in T type.</param>
  /// <returns>The reference of type associated with the specified key, or <see langword="null"/> if no
  /// such reference exists.</returns>
  public object? GetReference(Type type, string key)
  {
    if (_referencedObjects.TryGetValue(type, out var references))
    {
      if (references.TryGetValue(key, out var value))
      {
        return value;
      }
    }
    return null;
  }
}
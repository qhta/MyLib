namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// EnumItem is a record type that represents an item in an enumeration and its associated name.
/// </summary>
/// <typeparam name="T"></typeparam>
public record EnumItem<T> where T : struct, Enum
{
  /// <summary>
  /// Enumeration value of the item, represented as a generic type T.
  /// </summary>
  public T Id { get; set; }
  /// <summary>
  /// Name of the enumeration item, typically a string that describes the item.
  /// </summary>
  public string Name { get; set; } = null!;
}
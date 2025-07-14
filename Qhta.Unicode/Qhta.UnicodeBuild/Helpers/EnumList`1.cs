namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// List of enum items.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EnumList<T>: List<EnumItem<T>> where T : struct, Enum
{
  /// <summary>
  /// Automatically populates the list with enum values of type T.
  /// </summary>
  public EnumList()
  {
    foreach (var item in Enum.GetValues(typeof(T)).Cast<T>())
    {
      Add(new EnumItem<T>
      {
        Id = item,
        Name = item.ToString()!
      });
    }
  }

  /// <summary>
  /// Contains the enum values of type T.
  /// </summary>
  public static Array EnumValues => Enum.GetValues(typeof(T));
}
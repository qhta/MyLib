namespace Qhta.MVVM;

/// <summary>
/// Filter is a wrapper for predicate of object.
/// </summary>
public interface IFilter
{

  /// <summary>
  /// Get delegate to any function that gets an object and returns boolean.
  /// </summary>
  public Predicate<object> GetPredicate ();

  /// <summary>
  /// Checks whether the item should be accepted using a predicate.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public bool Accept(object item);
}

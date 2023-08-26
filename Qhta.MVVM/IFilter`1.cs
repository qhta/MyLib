namespace Qhta.MVVM;

/// <summary>
/// Filter of T is a wrapper for predicate of T.
/// </summary>
public interface IFilter<T>: IFilter
{
  /// <summary>
  /// Delegate to any function that gets an object and returns boolean.
  /// </summary>
  public new Predicate<T> GetPredicate();

  /// <summary>
  /// Checks whether the item should be accepted using a predicate.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public bool Accept(T item);
}

namespace Qhta.MVVM;

/// <summary>
/// Implementation of IFilter that is an expression of IFilters.
/// </summary>
public abstract class FilterExpr : IFilter
{
  /// <summary>
  /// Operands of the expression.
  /// </summary>
  public Collection<IFilter> Operands { get; private set; } = new Collection<IFilter>();

  /// <summary>
  /// Gets the predicate function basing on the operands.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public abstract Predicate<object> GetPredicate();

  /// <summary>
  /// Evaluates the predicate.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public virtual bool Accept(object item)
  {
    return GetPredicate().Invoke(item);
  }
}

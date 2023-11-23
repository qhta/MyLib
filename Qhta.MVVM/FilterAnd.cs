namespace Qhta.MVVM;

/// <summary>
/// Implementation of IFilter that is a conjuction of IFilters.
/// </summary>
public class FliterAnd : FilterExpr
{
  /// <summary>
  /// Gets a conjunction of the operands.
  /// </summary>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public override Predicate<object> GetPredicate()
  {
    return new Predicate<object>((obj) =>
    {
      foreach (var item in Operands)
        if (!item.Accept(obj))
          return false;
      return true;
    });
  }

}

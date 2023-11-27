namespace Qhta.WPF.Utils;

/// <summary>
/// Prepared filter for a column. Holds property info, other value to compare, compare function and predicate function.
/// </summary>
public class CompoundFilter : IFilter
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public CompoundFilter(BooleanOperation op)
  {
    Operation = op;
  }

  /// <summary>
  /// Component filters;
  /// </summary>
  public Collection<IFilter> Items { get; set; } = new();

  /// <summary>
  /// Predicate to evaluate for the column value.
  /// </summary>
  public BooleanOperation Operation { get; set; }

  /// <summary>
  /// Predicate to evaluate for the column value.
  /// </summary>
  public Predicate<object> Predicate
  {
    get
    {
      if (Operation == BooleanOperation.And)
        return new Predicate<object>((obj) =>
        {
          foreach (var item in Items)
            if (!item.Accept(obj)) return false;
          return true;
        });
      if (Operation == BooleanOperation.Or)
        return new Predicate<object>((obj) =>
        {
          foreach (var item in Items)
            if (item.Accept(obj)) return true;
          return false;
        });
      throw new InvalidOperationException($"Compound filter boolean operation not specified");
    }
  }

  /// <summary>
  /// Implementation of IFilter.
  /// </summary>
  /// <returns></returns>
  Predicate<object> IFilter.GetPredicate() => Predicate;

  /// <summary>
  /// Implementation of IFilter.
  /// </summary>
  /// <param name="item"></param>
  /// <returns></returns>
  public bool Accept(object item) => Predicate.Invoke(item);
}

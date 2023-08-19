namespace Qhta.WPF.Utils.ViewModels
{
  /// <summary>
  /// Enumeration of functions used to define boolean predicate.
  /// </summary>
  public enum BoolPredicateFunction
  {
    /// <summary>
    /// Is boolean a true value?
    /// </summary>
    IsTrue,

    /// <summary>
    /// Is boolean a false value?
    /// </summary>
    IsFalse,

    /// <summary>
    /// Is boolean undefined (is null)?
    /// </summary>
    IsEmpty,

    /// <summary>
    /// Is boolean defined (is not null)?
    /// </summary>
    NotEmpty,

  }
}

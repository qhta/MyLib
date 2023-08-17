namespace Qhta.WPF.Utils.ViewModels
{
  /// <summary>
  /// Enumeration of functions used to define boolean predicate.
  /// </summary>
  public enum BoolPredicateFunction
  {
    /// <summary>
    /// Is boolean a true value.
    /// </summary>
    IsTrue,

    /// <summary>
    /// Is boolean a false value.
    /// </summary>
    IsFalse,

    /// <summary>
    /// Boolean is undefined (is null).
    /// </summary>
    IsEmpty,

    /// <summary>
    /// Boolean is defined (is not null).
    /// </summary>
    NotEmpty,

  }
}

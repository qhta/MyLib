namespace Qhta.WPF.Utils.ViewModels
{
  /// <summary>
  /// Enumeration of functions used to define text predicate.
  /// </summary>
  public enum TextPredicateFunction
  {
    /// <summary>
    /// Is text equal to specific string?
    /// </summary>
    IsEqual,

    /// <summary>
    /// Is text not equal to specific string?
    /// </summary>
    NotEqual,

    /// <summary>
    /// Is text null or empty?
    /// </summary>
    IsEmpty,

    /// <summary>
    /// Is text not null and is not empty?
    /// </summary>
    NotEmpty,

    /// <summary>
    /// Does text start with specific string?
    /// </summary>
    StartsWith,

    /// <summary>
    /// Whether text does not start with specific string?
    /// </summary>
    NotStartsWith,

    /// <summary>
    /// Does text end with specific string?
    /// </summary>
    EndsWith,

    /// <summary>
    /// Whether text does not end with specific string?
    /// </summary>
    NotEndsWith,

    /// <summary>
    /// Does text contain specific string?
    /// </summary>
    Contains,

    /// <summary>
    /// Whether text does not contain specific string?
    /// </summary>
    NotContains,

    /// <summary>
    /// Is regular expression found in text?
    /// </summary>
    RegExpr,
  }
}

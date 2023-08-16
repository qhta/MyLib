namespace Qhta.WPF.Utils.ViewModels
{
  /// <summary>
  /// Function uses to define text predicate
  /// </summary>
  public enum TextPredicateFunction
  {
    /// <summary>
    /// Is text equal to specific string.
    /// </summary>
    IsEqual,

    /// <summary>
    /// Text is not equal to specific string.
    /// </summary>
    NotEqual,

    /// <summary>
    /// Text starts with specific string.
    /// </summary>
    StartsWith,

    /// <summary>
    /// Text ends with specific string.
    /// </summary>
    EndsWith,

    /// <summary>
    /// Text contains specific string.
    /// </summary>
    Contains,

    /// <summary>
    /// Regular expression is found in text.
    /// </summary>
    RegExpr,
  }
}

namespace Qhta.WPF.Utils.ViewModels
{
  /// <summary>
  /// Enumeration of functions used to define enum value predicate.
  /// </summary>
  public enum EnumPredicateFunction
  {
    /// <summary>
    /// Is enum equal to specific value?
    /// </summary>
    IsEqual,

    /// <summary>
    /// Is enum not equal to specific value?
    /// </summary>
    NotEqual,

    /// <summary>
    /// Is enum undefined (is null)?
    /// </summary>
    IsEmpty,

    /// <summary>
    /// Is enum defined (is not null)?
    /// </summary>
    NotEmpty,

  }
}

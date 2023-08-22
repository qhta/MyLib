namespace Qhta.WPF.Utils.ViewModels
{
  /// <summary>
  /// Enumeration of functions used to define numeric value predicate.
  /// </summary>
  public enum NumPredicateFunction
  {
    /// <summary>
    /// Is number equal to specific value?
    /// </summary>
    IsEqual,

    /// <summary>
    /// Is number not equal to specific value?
    /// </summary>
    NotEqual,

    /// <summary>
    /// Is number undefined (is null)?
    /// </summary>
    IsEmpty,

    /// <summary>
    /// Is number defined (is not null)?
    /// </summary>
    NotEmpty,

    /// <summary>
    /// Is number greater than specific value?
    /// </summary>
    IsGreater,

    /// <summary>
    /// Is number not greater than specific value (is less or equal)?
    /// </summary>
    NotGreater,

    /// <summary>
    /// Is number less than specific value?
    /// </summary>
    IsLess,

    /// <summary>
    /// Is number not less than specific value (is greater or equal)?
    /// </summary>
    NotLess,

  }
}

namespace Qhta.MVVM
{
  /// <summary>
  /// Interface for an object that cant be validated.
  /// </summary>
  public interface IValidated
  {
    /// <summary>
    /// Specifies whether the object is valid.
    /// </summary>
    bool? IsValid { get; }
  }
}

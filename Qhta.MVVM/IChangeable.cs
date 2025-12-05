namespace Qhta.MVVM;

/// <summary>
/// Interface for changeable objects.
/// </summary>
public interface IChangeable
{
  /// <summary>
  /// Specifies if the object has been changed.
  /// </summary>
  public bool? IsChanged { get; set; }

}
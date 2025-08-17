namespace Qhta.SF.Tools;

/// <summary>
/// Singleton class representing a non-empty value in the application.
/// </summary>
public class NonEmptyValue
{
  private NonEmptyValue()
  {
    // Private constructor to prevent instantiation from outside the class.
  }

  /// <summary>
  /// Static singleton instance of the NonEmptyValue class.
  /// </summary>
  public static NonEmptyValue Instance { get; } = new NonEmptyValue();
}
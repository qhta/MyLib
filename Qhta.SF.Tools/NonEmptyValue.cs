using Qhta.SF.Tools.Resources;

namespace Qhta.SF.Tools;

/// <summary>
/// Object representing non-empty value.
/// </summary>
public class NonEmptyValue
{
  /// <summary>
  /// Returns a <see cref="NonEmptyValue"/>.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return DataStrings.EmptyValue;
  }
}
using Qhta.SF.Tools.Resources;

namespace Qhta.SF.Tools;

/// <summary>
/// Object representing an empty value.
/// </summary>
public class EmptyValue
{
  /// <summary>
  /// Returns a <see cref="EmptyValue"/>.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return DataStrings.EmptyValue;
  }
}
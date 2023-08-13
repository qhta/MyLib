namespace Qhta.WPF.Utils;

/// <summary>
/// Interface that defines GetError method.
/// </summary>
public interface IProposedValueErrorInfo
{
  /// <summary>
  /// Gets an error for some value proposed for a property.
  /// </summary>
  /// <param name="propertyName"></param>
  /// <param name="value"></param>
  /// <param name="cultureInfo"></param>
  /// <returns></returns>
  object GetError(string propertyName, object value, CultureInfo cultureInfo);
}

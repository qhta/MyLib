namespace Qhta.Xml;

/// <summary>
///   Interface for a type converter with unit property.
/// </summary>
public interface IRealTypeConverter
{
  /// <summary>
  /// Defines string shortcut of unit.
  /// </summary>
  public string? Unit { get; set; }
}
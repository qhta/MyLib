namespace Qhta.Conversion;

/// <summary>
///  Defines numeric length restrictions and value restrictions.
/// </summary>
public interface INumberRestrictions
{
  /// <summary>
  /// Specifies total number of digits
  /// </summary>
  public int? TotalDigits { get; set; }

  /// <summary>
  /// Specifies fractional number of digits
  /// </summary>
  public int? FractionDigits { get; set; }

  /// <summary>
  /// Specifies minimum (double) value outside an accepted range.
  /// </summary>
  public double? MinExclusive { get; set; }

  /// <summary>
  /// Specifies maximum (double) value outside an accepted range.
  /// </summary>
  public double? MaxExclusive { get; set; }

  /// <summary>
  /// Specifies minimum (double) value inside an accepted range.
  /// </summary>
  public double? MinInclusive { get; set; }

  /// <summary>
  /// Specifies maximum (double) value inside an accepted range.
  /// </summary>
  public double? MaxInclusive { get; set; }


}
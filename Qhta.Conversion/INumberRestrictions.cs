namespace Qhta.Conversion;

public interface INumberRestrictions
{
  public int? TotalDigits { get; set; }

  public int? FractionDigits { get; set; }

  public double? MinExclusive { get; set; }

  public double? MaxExclusive { get; set; }

  public double? MinInclusive { get; set; }

  public double? MaxInclusive { get; set; }
}
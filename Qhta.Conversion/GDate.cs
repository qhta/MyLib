namespace Qhta.Conversion;

/// <summary>
/// Generic date structure that specifies Year, Month, Day and Zone components.
/// A 0 (zero) value for a specific component means it is unimportant.
/// So we can have Year-only, Year-month, Year-Month-Day or Year-Month-Date-Zone values.
/// </summary>
public struct GDate
{

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
  public ushort Year { get; set; }
  public byte Month { get; set; }
  public byte Day { get; set; }
  public sbyte Zone { get; set; }

  public GDate(int year, int month = 0, int day = 0, int zone = 0)
  {
    Year = (ushort)year;
    Month = (byte)month;
    Day = (byte)day;
    Zone = (sbyte)zone;
  }

  public override string ToString()
  {
    var str = $"{Year:D4}-{Month:D2}-{Day:D2}";
    if (Zone > 0)
      str += "+" + Zone.ToString("D2");
    else if (Zone < 0)
      str += Zone.ToString("D2");
    return str;
  }
}
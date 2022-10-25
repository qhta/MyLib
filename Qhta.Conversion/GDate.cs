namespace Qhta.Conversion;

public struct GDate
{
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
    else
    if (Zone < 0)
      str += Zone.ToString("D2");
    return str;
  }
}
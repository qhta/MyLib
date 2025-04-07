using System.Globalization;

namespace Qhta.UnicodeBuild.Helpers;

public record RangeModel(): IComparable<RangeModel>
{
  public int Start { get; set; }
  public int? End { get; set; }

  public int CompareTo(RangeModel? other)
  {
    return this.Start.CompareTo(other?.Start);
  }

  public override string ToString()
  {
    var result = Start.ToString("X4");
    if (End.HasValue)
    {
      result += ".." + ((int)End).ToString("X4");
    }
    return result;
  }

  public static RangeModel Parse(string str)
  {
    str = str.Trim();
    var k = str.IndexOf("..");
    if (k != -1)
    {
      var s1 = str.Substring(0, k);
      var s2 = str.Substring(k+2);
      var n1 = int.Parse(s1, NumberStyles.HexNumber);
      var n2 = int.Parse(s2, NumberStyles.HexNumber);
      return new RangeModel { Start = n1, End = n2 };
    }
    else
    {
      var n = int.Parse(str, NumberStyles.HexNumber);
      return new RangeModel { Start = n };
    }
  }

  public static implicit operator RangeModel? (string? value) => String.IsNullOrEmpty(value) ? null :RangeModel.Parse(value);

  public static implicit operator string? (RangeModel? value) => value?.ToString();


}
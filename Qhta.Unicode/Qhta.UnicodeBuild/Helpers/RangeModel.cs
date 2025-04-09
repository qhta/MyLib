using System.Diagnostics;
using System.Globalization;

namespace Qhta.UnicodeBuild.Helpers;

public record RangeModel() : IComparable<RangeModel>
{
  public int Start { get; set; }
  public int? End { get; set; } // if null, it means the range is a single value
  public string? Separator { get; set; } // ".." or "." or null

  public int CompareTo(RangeModel? other)
  {
    return this.Start.CompareTo(other?.Start);
  }

  public override string ToString()
  {
    var result = Start.ToString("X4");
    if (End.HasValue)
    {
      var end = End.Value;
      result += (Separator ?? "..") + end.ToString("X4");
    }
    else if (Separator != null)
    {
      result += Separator;
    }
    return result;
  }
  public static RangeModel Parse(string str)
  {
    if (TryParse(str, out var result))
      return result!;
    throw new FormatException($"Invalid range format: {str}");
  }

  public static bool TryParse(string str, out RangeModel? result)
  {
    result = null;
    str = str.Trim();
    if (string.IsNullOrEmpty(str))
      return false;
    var k = str.IndexOf("..");
    if (k != -1)
    {
      var s1 = str.Substring(0, k);
      var ok1 = int.TryParse(s1, NumberStyles.HexNumber, null, out var n1);
      if (!ok1)
        return false;
      if (k + 2 < str.Length)
      {
        var s2 = str.Substring(k + 2);

        var ok2 = int.TryParse(s2, NumberStyles.HexNumber, null, out var n2);
        if (!ok2)
          return false;
        result = new RangeModel { Start = n1, End = n2 };
        return true;
      }
      result = new RangeModel { Start = n1, End = null, Separator=".." };
      return true;
    }
    if ((k = str.IndexOf(".")) != -1)
    {
      var s1 = str.Substring(0, k);
      var ok1 = int.TryParse(s1, NumberStyles.HexNumber, null, out var n1);
      if (!ok1)
        return false;
      if (k + 1 < str.Length)
      {
        var s2 = str.Substring(k + 2);

        var ok2 = int.TryParse(s2, NumberStyles.HexNumber, null, out var n2);
        if (!ok2)
          return false;
        result = new RangeModel { Start = n1, End = n2 };
        return true;
      }
      result = new RangeModel { Start = n1, Separator = "." };
      return true;
    }

    else
    {
      var ok1 = int.TryParse(str, NumberStyles.HexNumber, null, out var n1);
      if (!ok1)
        return false;
      result = new RangeModel { Start = n1 };
    }
    return true;
  }

  public static implicit operator RangeModel?(string? value) => String.IsNullOrEmpty(value) ? null : RangeModel.Parse(value);

  public static implicit operator string?(RangeModel? value) => value?.ToString();


}
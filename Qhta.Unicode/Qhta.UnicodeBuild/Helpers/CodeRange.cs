using System.Diagnostics;
using System.Globalization;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Represents a range of codes, defined by a start and an optional end value, with an optional separator.
/// </summary>
/// <remarks>The <see cref="CodeRange"/> can represent a single value or a range of values. If the <see
/// cref="End"/> property is null, the range is considered a single value. The <see cref="Separator"/> property can be
/// used to specify a custom separator between the start and end values when converting the range to a string.</remarks>
public record CodeRange() : IComparable<CodeRange>, IComparable
{
  /// <summary>
  /// Start of the code range, represented as an integer.
  /// </summary>
  public int Start { get; set; }
  /// <summary>
  /// End of the code range, represented as an integer. If null, it means the range is a single value.
  /// </summary>
  public int? End { get; set; }
  /// <summary>
  /// Separator used when converting the range to a string. If null, the default separator ".." is used.
  /// </summary>
  public string? Separator { get; set; }

  /// <summary>
  /// Compares this code range with another code range based on the start value.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public int CompareTo(CodeRange? other)
  {
    return this.Start.CompareTo(other?.Start);
  }

  /// <summary>
  /// Compares this code range with another object. If the object is not a <see cref="CodeRange"/>, an exception is thrown.
  /// Implemented for compatibility with IComparable interface.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  /// <exception cref="InvalidCastException"></exception>
  public int CompareTo(object? obj)
  {
    if (obj is CodeRange other)
      return CompareTo(other);
    throw new InvalidCastException();
  }

  /// <summary>
  /// Converts the code range to its string representation in hexadecimal format. If the <see cref="End"/> is
  /// null, it returns only the start value. If the <see cref="Separator"/> is null, it uses the default separator "..".
  /// </summary>
  /// <returns></returns>
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

  /// <summary>
  /// Converts a string representation of a code range in hexadecimal format to a <see cref="CodeRange"/> object.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  /// <exception cref="FormatException"></exception>
  public static CodeRange Parse(string str)
  {
    if (TryParse(str, out var result))
      return result!;
    throw new FormatException(String.Format(Resources.Strings.InvalidCodeRangeFormat, str));
  }

  /// <summary>
  /// Safe parsing method for a string representation of a code range in hexadecimal format.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="result"></param>
  /// <returns></returns>
  public static bool TryParse(string str, out CodeRange? result)
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
        result = new CodeRange { Start = n1, End = n2 };
        return true;
      }
      result = new CodeRange { Start = n1, Separator=".." };
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
        var s2 = str.Substring(k + 1);

        var ok2 = int.TryParse(s2, NumberStyles.HexNumber, null, out var n2);
        if (!ok2)
          return false;
        result = new CodeRange { Start = n1, End = n2 };
        return true;
      }
      result = new CodeRange { Start = n1, Separator = "." };
      return true;
    }

    else
    {
      var ok1 = int.TryParse(str, NumberStyles.HexNumber, null, out var n1);
      if (!ok1)
        return false;
      result = new CodeRange { Start = n1 };
    }
    return true;
  }

  /// <summary>
  /// Converts a string representation of a code range to a <see cref="CodeRange"/> object.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator CodeRange?(string? value) => String.IsNullOrEmpty(value) ? null : CodeRange.Parse(value);

  /// <summary>
  /// Converts a <see cref="CodeRange"/> object to its string representation in hexadecimal format.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator string?(CodeRange? value) => value?.ToString();

  /// <summary>
  /// Checks if the code range contains a specific code.
  /// </summary>
  /// <param name="code"></param>
  /// <returns></returns>
  public bool Contains(int code)
  {
    if (End.HasValue)
      return code >= Start && code <= End.Value;
    return code == Start;
  }

}
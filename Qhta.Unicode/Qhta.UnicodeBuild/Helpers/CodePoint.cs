using System.Diagnostics;
using System.Globalization;

namespace Qhta.UnicodeBuild.Helpers;

public record CodePoint() : IComparable, IComparable<CodePoint>
{
  public int Value { get; set; }

  public int CompareTo(CodePoint? other)
  {
    return this.Value.CompareTo(other?.Value);
  }

  public override string ToString()
  {
    var result = Value.ToString("X4");
    return result;
  }

  public int CompareTo(object? obj)
  {
    if (obj is CodePoint other)
      return CompareTo(other);
    throw new NotImplementedException();
  }

  public static CodePoint Parse(string str)
  {
    if (TryParse(str, out var result))
      return result!;
    throw new FormatException($"Invalid code format: {str}");
  }

  public static bool TryParse(string str, out CodePoint? result)
  {
    result = null;
    str = str.Trim();
    if (string.IsNullOrEmpty(str))
      return false;
    var ok1 = int.TryParse(str, NumberStyles.HexNumber, null, out var n1);
    if (!ok1)
      return false;
    result = new CodePoint { Value = n1 };
    return true;
  }

  public static implicit operator CodePoint?(string? value) => String.IsNullOrEmpty(value) ? null : CodePoint.Parse(value);

  public static implicit operator string?(CodePoint? value) => value?.ToString();


  public static implicit operator CodePoint?(int? value) => (value is null) ? null : new CodePoint {  Value =(int)value };

  public static implicit operator int?(CodePoint? value) => value?.Value;
  public static implicit operator int(CodePoint value) => value.Value;
}
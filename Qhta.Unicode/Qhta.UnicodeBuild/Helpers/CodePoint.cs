using System.Diagnostics;
using System.Globalization;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// Value object representing a Unicode code point.
/// Internal representation is an integer, and external representation is a hexadecimal string.
/// </summary>
public record CodePoint() : IComparable, IComparable<CodePoint>
{
  /// <summary>
  /// Internal value
  /// </summary>
  public int Value { get; set; }

  /// <summary>
  /// Compares this code point with another code point using their internal values.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public int CompareTo(CodePoint? other)
  {
    return this.Value.CompareTo(other?.Value);
  }

  /// <summary>
  /// Compares this code point with another object. If the object is not a <see cref="CodePoint"/>, an exception is thrown.
  /// Implemented for compatibility with IComparable interface.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  /// <exception cref="NotImplementedException"></exception>
  public int CompareTo(object? obj)
  {
    if (obj is CodePoint other)
      return CompareTo(other);
    throw new InvalidCastException();
  }

  /// <summary>
  /// Converts the code point to its hexadecimal string representation with at least 4 digits.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    var result = Value.ToString("X4");
    return result;
  }

  /// <summary>
  /// Parses a string representation of a code point in hexadecimal format.
  /// </summary>
  /// <param name="str"></param>
  /// <returns></returns>
  /// <exception cref="FormatException"></exception>
  public static CodePoint Parse(string str)
  {
    if (TryParse(str, out var result))
      return result!;
    throw new FormatException($"Invalid code format: {str}");
  }

  /// <summary>
  /// Safely tries to parse a string representation of a code point in hexadecimal format.
  /// </summary>
  /// <param name="str"></param>
  /// <param name="result"></param>
  /// <returns></returns>
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

  /// <summary>
  /// Converts a string representation of a code point to a CodePoint object.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator CodePoint?(string? value) => String.IsNullOrEmpty(value) ? null : CodePoint.Parse(value);

  /// <summary>
  /// Converts a CodePoint object to its string representation in hexadecimal format.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator string?(CodePoint? value) => value?.ToString();

  /// <summary>
  /// Converts an integer to a CodePoint object.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator CodePoint?(int? value) => (value is null) ? null : new CodePoint {  Value =(int)value };

  /// <summary>
  /// Converts a CodePoint object to an integer. Allows for null values.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator int?(CodePoint? value) => value?.Value;

  /// <summary>
  /// Converts a CodePoint to an integer implicitly.
  /// </summary>
  /// <param name="value">The <see cref="CodePoint"/> instance to convert.</param>
  public static implicit operator int(CodePoint value) => value.Value;
}
namespace Qhta.Unicode;

/// <summary>
/// A Unicode code point.
/// Contains a single integer value.
/// Converts to and from hexadecimal string.
/// </summary>
public readonly struct CodePoint: IComparable
{
  private readonly int Value;

  // ReSharper disable once ConvertToPrimaryConstructor
  /// <summary>
  /// Create a code point from an integer value.
  /// </summary>
  /// <param name="value"></param>
  public CodePoint(int value) : this()
  {
    Value = value;
  }

  /// <summary>
  /// Create a code point from a hexadecimal string.
  /// </summary>
  /// <param name="str"></param>
  public CodePoint(string str) : this()
  {
    Value = int.Parse(str, System.Globalization.NumberStyles.HexNumber); ;
  }

  /// <summary>
  /// Explicit conversion from CodePoint to hexadecimal string (at least four digits).
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return Value.ToString("X4");
  }

  /// <summary>
  /// Explicit conversion from CodePoint to string with given format.
  /// </summary>
  /// <returns></returns>
  public string ToString(string format)
  {
    return Value.ToString(format);
  }

  /// <summary>
  /// Compare two code points.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public int CompareTo(object? obj)
  {
    if (obj is CodePoint point)
      return Value.CompareTo(point.Value);
    return Value.CompareTo(obj);
  }

  #region implicit operators
  /// <summary>
  /// Implicit conversion from CodePoint to hexadecimal string (at least four digits).
  /// </summary>
  /// <param name="codePoint"></param>
  public static implicit operator string(CodePoint codePoint)
  {
    return codePoint.Value.ToString("X4");
  }

  /// <summary>
  /// Implicit conversion from hexadecimal string to CodePoint.
  /// </summary>
  /// <param name="str"></param>
  public static implicit operator CodePoint(string str)
  {
    return new CodePoint(str);
  }

  /// <summary>
  /// Implicit conversion from CodePoint to integer.
  /// </summary>
  /// <param name="codePoint"></param>
  public static implicit operator int(CodePoint codePoint)
  {
    return codePoint.Value;
  }

  /// <summary>
  /// Implicit conversion from integer to CodePoint.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator CodePoint(int value)
  {
    return new CodePoint(value);
  }
  #endregion
}
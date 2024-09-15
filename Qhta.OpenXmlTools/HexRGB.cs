using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Represents RGB integer color code.
/// </summary>
public struct HexRGB
{

  /// <summary>
  /// Stored value.
  /// </summary>
  public int Value { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="HexRGB"/> struct with the integer value.
  /// </summary>
  /// <param name="value"></param>
  public HexRGB(int value)
  {
    Value = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="HexRGB"/> struct with the string value.
  /// </summary>
  /// <param name="value"></param>
  public HexRGB(string value)
  {
    Value = Int32.Parse(value, NumberStyles.HexNumber);
  }

  /// <summary>
  /// Converts the integer to the <see cref="HexRGB"/> struct.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator HexRGB(int value)
  {
    return new HexRGB(value);
  }

  /// <summary>
  /// Converts the <see cref="HexRGB"/> struct to the integer.
  /// </summary>
  /// <param name="hexInt"></param>
  public static implicit operator int(HexRGB hexInt)
  {
    return hexInt.Value;
  }

  /// <summary>
  /// Converts the string value to the <see cref="HexRGB"/> struct.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator HexRGB?(string? value)
  {
    if (value == null)
      return null;
    return new HexRGB(value);
  }

  /// <summary>
  /// Converts the <see cref="HexRGB"/> struct to the string.
  /// </summary>
  /// <param name="hexInt"></param>
  public static implicit operator string?(HexRGB? hexInt)
  {
    return hexInt.ToString();
  }

  /// <summary>
  /// Converts the <see cref="HexRGB"/> struct to the string.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return Value.ToString("X6");
  }
}

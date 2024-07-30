using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Represents a hexadecimal integer.
/// </summary>
public struct HexInt
{

  /// <summary>
  /// Stored value.
  /// </summary>
  public int Value { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="HexInt"/> struct with the integer value.
  /// </summary>
  /// <param name="value"></param>
  public HexInt(int value)
  {
    Value = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="HexInt"/> struct with the string value.
  /// </summary>
  /// <param name="value"></param>
  public HexInt(string value)
  {
    Value = Int32.Parse(value, NumberStyles.HexNumber);
  }

  /// <summary>
  /// Converts the integer to the <see cref="HexInt"/> struct.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator HexInt(int value)
  {
    return new HexInt(value);
  }

  /// <summary>
  /// Converts the <see cref="HexInt"/> struct to the integer.
  /// </summary>
  /// <param name="hexInt"></param>
  public static implicit operator int(HexInt hexInt)
  {
    return hexInt.Value;
  }

  /// <summary>
  /// Converts the string value to the <see cref="HexInt"/> struct.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator HexInt?(string? value)
  {
    if (value == null)
      return null;
    return new HexInt(value);
  }

  /// <summary>
  /// Converts the <see cref="HexInt"/> struct to the string.
  /// </summary>
  /// <param name="hexInt"></param>
  public static implicit operator string?(HexInt? hexInt)
  {
    return hexInt.ToString();
  }

  /// <summary>
  /// Converts the <see cref="HexInt"/> struct to the string.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return Value.ToString("X8");
  }
}

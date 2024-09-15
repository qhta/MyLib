using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Represents HexByte integer value.
/// </summary>
public struct HexByte
{

  /// <summary>
  /// Stored value.
  /// </summary>
  public int Value { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="HexByte"/> struct with the integer value.
  /// </summary>
  /// <param name="value"></param>
  public HexByte(int value)
  {
    Value = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="HexByte"/> struct with the string value.
  /// </summary>
  /// <param name="value"></param>
  public HexByte(string value)
  {
    Value = Int32.Parse(value, NumberStyles.HexNumber);
  }

  /// <summary>
  /// Converts the integer to the <see cref="HexByte"/> struct.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator HexByte(int value)
  {
    return new HexByte(value);
  }

  /// <summary>
  /// Converts the <see cref="HexByte"/> struct to the integer.
  /// </summary>
  /// <param name="hexInt"></param>
  public static implicit operator int(HexByte hexInt)
  {
    return hexInt.Value;
  }

  /// <summary>
  /// Converts the string value to the <see cref="HexByte"/> struct.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator HexByte?(string? value)
  {
    if (value == null)
      return null;
    return new HexByte(value);
  }

  /// <summary>
  /// Converts the <see cref="HexByte"/> struct to the string.
  /// </summary>
  /// <param name="hexInt"></param>
  public static implicit operator string?(HexByte? hexInt)
  {
    return hexInt.ToString();
  }

  /// <summary>
  /// Converts the <see cref="HexByte"/> struct to the string.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return Value.ToString("X2");
  }
}

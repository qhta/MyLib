using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Represents a twips measure integer.
/// </summary>
public struct Twips
{

  /// <summary>
  /// Stored value.
  /// </summary>
  public int Value { get; set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Twips"/> struct with the integer value.
  /// </summary>
  /// <param name="value"></param>
  public Twips(int value)
  {
    Value = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Twips"/> struct with the string value.
  /// </summary>
  /// <param name="value"></param>
  public Twips(string value)
  {
    Value = Int32.Parse(value);
  }

  /// <summary>
  /// Converts the integer to the <see cref="Twips"/> struct.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator Twips?(Int32? value)
  {
    if (value == null)
      return null;
    return new Twips((int)value);
  }

  /// <summary>
  /// Converts the <see cref="Twips"/> struct to the integer.
  /// </summary>
  /// <param name="Twips"></param>
  public static implicit operator int?(Twips?Twips)
  {
    return Twips?.Value;
  }


  /// <summary>
  /// Converts the unsigned integer to the <see cref="Twips"/> struct.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator Twips?(UInt32? value)
  {
    if (value == null)
      return null;
    return new Twips((int)value);
  }

  /// <summary>
  /// Converts the <see cref="Twips"/> struct to the unsigned integer.
  /// </summary>
  /// <param name="Twips"></param>
  public static implicit operator uint?(Twips? Twips)
  {
    return (uint?)Twips?.Value;
  }
  /// <summary>
  /// Converts the string value to the <see cref="Twips"/> struct.
  /// </summary>
  /// <param name="value"></param>
  public static implicit operator Twips?(string? value)
  {
    if (value == null)
      return null;
    return new Twips(value);
  }

  /// <summary>
  /// Converts the <see cref="Twips"/> struct to the string.
  /// </summary>
  /// <param name="Twips"></param>
  public static implicit operator string?(Twips? Twips)
  {
    return Twips.ToString();
  }

  /// <summary>
  /// Converts the <see cref="Twips"/> struct to the string.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return Value.ToString();
  }
}

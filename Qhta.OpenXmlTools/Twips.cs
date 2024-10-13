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
  /// Initializes a new instance of the <see cref="Twips"/> struct with the unsigned integer value.
  /// </summary>
  /// <param name="value"></param>
  public Twips(uint value)
  {
    Value = (int)value;
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
  /// Converts string to the <see cref="Twips"/> struct.
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static Twips FromString(string value)
  {
    return new Twips(value);
  }

  /// <summary>
  /// Converts the <see cref="Twips"/> struct to the string.
  /// </summary>
  /// <returns></returns>
  public override string ToString()
  {
    return Value.ToString();
  }

  /// <summary>
  /// Equality comparison operator.
  /// </summary>
  /// <param name="a"></param>
  /// <param name="b"></param>
  /// <returns></returns>
  public static bool operator ==(Twips? a, Twips? b)
  {
    if (a is null)
      return b is null;
    return a.Equals(b);
  }

  /// <summary>
  /// Inequality comparison operator.
  /// </summary>
  /// <param name="a"></param>
  /// <param name="b"></param>
  /// <returns></returns>
  public static bool operator !=(Twips? a, Twips? b)
  {
    return !(a == b);
  }

  /// <summary>
  /// Converts the <see cref="Twips"/> value to points.
  /// </summary>
  /// <returns></returns>
  public double ToPoints()
  {
    return this.Value / 20.0;
  }

  /// <summary>
  /// Converts the points value to the <see cref="Twips"/> value.
  /// </summary>
  /// <param name="points"></param>
  /// <returns></returns>
  public static Twips FromPoints(double points)
  {
    return new Twips((int)(points * 20));
  }

  /// <summary>
  /// Compares the <see cref="Twips"/> value to the other.
  /// </summary>
  /// <param name="other"></param>
  /// <returns></returns>
  public bool Equals(Twips other)
  {
    return Value == other.Value;
  }

  /// <summary>
  /// Compares the <see cref="Twips"/> value to the other object.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public override bool Equals(object? obj)
  {
    return obj is Twips other && Equals(other);
  }

  /// <summary>
  /// Gets the hash code of the <see cref="Twips"/> value
  /// </summary>
  /// <returns></returns>
  public override int GetHashCode()
  {
    return Value;
  }
}

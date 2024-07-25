using System;
using DocumentFormat.OpenXml;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml EMU values.
/// </summary>
public static class EMUTools
{
  /// <summary>
  /// Convert an integer value in EMU to millimeters.
  /// </summary>
  /// <param name="x">Source integer value</param>
  /// <param name="precision">Number of decimal digits to round to (default 0)</param>
  /// <returns>Double result</returns>
  public static double ToMM(this Int32Value x, int precision=0)
  {
    return Math.Round(x / 56.6952380952381, precision);
  }

  /// <summary>
  /// Convert an unsigned integer value in EMU to millimeters.
  /// </summary>
  /// <param name="x">Source integer value</param>
  /// <param name="precision">Number of decimal digits to round to (default 0)</param>
  /// <returns>Double result</returns>
  public static double ToMM(this UInt32Value x, int precision=0)
  {
    return Math.Round(x / 56.6952380952381, precision);
  }

  /// <summary>
  /// Convert an integer value in EMU to millimeters string with "mm" suffix.
  /// </summary>
  /// <param name="x">Source integer value</param>
  /// <param name="precision">Number of decimal digits to round to (default 0)</param>
  /// <returns>String result in invariant culture format</returns>
  public static string ToStringMM(this Int32Value x, int precision=0)
  {
    var mm = Math.Round(x / 56.6952380952381, precision);
    var mmStr = mm.ToString(CultureInfo.InvariantCulture);
    return mmStr+"mm";
  }

  /// <summary>
  /// Convert an unsigned integer value in EMU to millimeters string with "mm" suffix.
  /// </summary>
  /// <param name="x">Source integer value</param>
  /// <param name="precision">Number of decimal digits to round to (default 0)</param>
  /// <returns>String result in invariant culture format</returns>
  public static string ToStringMM(this UInt32Value x, int precision=0)
  {
    var mm = Math.Round(x / 56.6952380952381, precision);
    var mmStr = mm.ToString(CultureInfo.InvariantCulture);
    return mmStr + "mm";
  }
}
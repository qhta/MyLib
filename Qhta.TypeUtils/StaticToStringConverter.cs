using System;

namespace Qhta.TypeUtils;

/// <summary>
///   A static class that converts object to string
/// </summary>
public static class StaticToStringConverter
{
  /// <summary>
  ///   Converting character value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? CharToString(Char value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting boolean value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? BoolToString(bool value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting signed byte value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? SByteToString(SByte value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting int16 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? Int16ToString(Int16 value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting int32 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? Int32ToString(Int32 value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting int64 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? Int64ToString(Int64 value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting byte value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? ByteToString(Byte value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting unsigned int16 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? UInt16ToString(UInt16 value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting unsigned int32 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? UInt32ToString(UInt32 value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting unsigned int64 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? UInt64ToString(UInt64 value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting float value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? FloatToString(float value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting double value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? DoubleToString(double value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting DateTime value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? DateTimeToString(DateTime value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting TimeSpan value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? TimeSpanToString(TimeSpan value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting enum value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? EnumToString(object? value)
  {
    if (value == null)
      return null;
    var result = value.ToString() ?? "";
    result = result.Replace(", ", "+");
    return result;
  }

  /// <summary>
  ///   Converting nullable char value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableCharToString(Char? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable bool value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableBoolToString(bool? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable signed byte value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableSByteToString(SByte? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable int16 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableInt16ToString(Int16? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable int32 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableInt32ToString(Int32? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable int64 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableInt64ToString(Int64? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable byte value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableByteToString(Byte? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable unsigned int16 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableUInt16ToString(UInt16? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable unsigned int32 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableUInt32ToString(UInt32? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable unsigned int64 value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableUInt64ToString(UInt64? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable float value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableFloatToString(float? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable double value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableDoubleToString(double? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable DateTime value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableDateTimeToString(DateTime? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable TimeSpan value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableTimeSpanToString(TimeSpan? value)
  {
    return value.ToString();
  }

  /// <summary>
  ///   Converting nullable enum value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? NullableEnumToString(object? value)
  {
    if (value == null)
      return null;
    var result = value.ToString() ?? "";
    result = result.Replace(", ", "+");
    return result;
  }

  /// <summary>
  ///   Converting class value to string
  /// </summary>
  /// <param name="value"></param>
  /// <returns></returns>
  public static string? ClassToString(object? value)
  {
    return value?.ToString();
  }
}